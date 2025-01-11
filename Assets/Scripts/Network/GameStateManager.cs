using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;
using System;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using static GameStateManager;

public class GameStateManager : MonoBehaviour
{
    [Serializable]
    public struct PlayerInputData
    {
        // Bytes = 
        public float send_horizontalInput;
        public float send_verticalInput;
        public float send_rotationAngle;
        public bool send_spaceInput;
        public bool send_spaceInputUp;
        public bool send_spaceInputDown;
        public bool send_shiftInputDown;

        public PlayerInputData(float h, float v, bool space, bool spaceUp, bool spaceDown, bool shiftDown, float rotAng)
        {
            send_horizontalInput = h;
            send_verticalInput = v;
            send_rotationAngle = rotAng;
            send_spaceInput = space;
            send_spaceInputUp = spaceUp;
            send_spaceInputDown = spaceDown;
            send_shiftInputDown = shiftDown;
        }
    };

    public enum playerID
    {
        NULL,
        SERVER,
        CLIENT
    };

    [Serializable]
    public struct WorldState
    {
        public Vector3 _serverPos;
        public Vector3 _clientPos;
        public Vector3 _ballPos;
        public Vector3 _ballRot;
        public Vector3 _ballVel;
        public playerID _playerID;

        public WorldState(Vector3 spos, Vector3 cpos, Vector3 bpos, Vector3 brot, Vector3 bVel, playerID pID)
        {
            _serverPos = spos;
            _clientPos = cpos;
            _ballPos = bpos;
            _ballRot = brot;
            _ballVel = bVel;
            _playerID = pID;
    }
    };

    [HideInInspector]
    public ClientUDP clientUDPScript;
    [HideInInspector]
    public ServerUDP serverUDPScript;

    public PlayerMovement localPlayer;
    public PlayerMovement remotePlayer;
    
    public float maxReplicationTime;
    private float replicationTimer;

    public bool server;

    PlayerInputData incomingData;
    PlayerInputData pendingToSendData;

    WorldState worldStateData;
    bool setWorldData;

    bool IAmClient;



    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Server")
        {
            IAmClient = false;
            serverUDPScript = GameObject.Find("ScriptContainer").GetComponent<ServerUDP>();
        }
        else if (SceneManager.GetActiveScene().name == "Client")
        {
            IAmClient = true;
            clientUDPScript = GameObject.Find("ScriptContainer").GetComponent<ClientUDP>();
        }

        Thread inGameReceive = new Thread(ReceivePlayerControls);
        inGameReceive.Start();

        Thread inGameSend = new Thread(SendPlayerControls);
        inGameSend.Start();

    }

    private void Update()
    {
        SetPlayerControls();


        if (server)
        {
            if (replicationTimer >= maxReplicationTime)
            {
                replicationTimer = 0;

                SendWorldState();
            }
            replicationTimer += Time.deltaTime;
        }

        //Debug.Log("Set world Data? " + setWorldData);
        if(setWorldData && IAmClient)
        {
            setWorldData = false;
            SetWorldData();
        }

       //Debug.Log("timer: " + replicationTimer);

    }

    // Assign the received controls
    void SetPlayerControls()
    {
        remotePlayer.horizontalInput = incomingData.send_horizontalInput;
        remotePlayer.verticalInput = incomingData.send_verticalInput;
        remotePlayer.spaceInput = incomingData.send_spaceInput;
        remotePlayer.spaceInputUp = incomingData.send_spaceInputUp;
        remotePlayer.spaceInputDown = incomingData.send_spaceInputDown;
        remotePlayer.shiftInputDown = incomingData.send_shiftInputDown;
        remotePlayer.rotationAngle = incomingData.send_rotationAngle;

    }

    // Send current controls
    void SendPlayerControls()
    {
        while(true)
        {
            PlayerInputData data = new PlayerInputData(
            localPlayer.horizontalInput,
            localPlayer.verticalInput,
            localPlayer.spaceInput,
            localPlayer.spaceInputUp,
            localPlayer.spaceInputDown,
            localPlayer.shiftInputDown,
            localPlayer.rotationAngle
            );

            byte[] buffer = StructToBytes(data);


            if (IAmClient && clientUDPScript != null)
            {
                //Debug.Log("Controls sending to client");
                clientUDPScript.socket.SendTo(buffer, SocketFlags.None, clientUDPScript.Remote);
            }
            else if (serverUDPScript != null)
            {
                //Debug.Log("Controls sending to client");
                serverUDPScript.socket.SendTo(buffer, SocketFlags.None, serverUDPScript.Remote);
            }
        }
        
    }

    // Listen for controls
    void ReceivePlayerControls()
    {
        if (IAmClient && clientUDPScript != null)
        {
            while (true)
            {
                // Allocate a buffer to receive data
                byte[] data = new byte[1024];
                int recv = clientUDPScript.socket.ReceiveFrom(data, ref clientUDPScript.Remote);

                // Trim the byte array to match the received data length
                byte[] trimmedData = new byte[recv];
                Array.Copy(data, trimmedData, recv);

                // Check the package size to differentiate
                if (trimmedData.Length == Marshal.SizeOf(typeof(WorldState)))
                {
                    worldStateData = BytesToStruct<WorldState>(trimmedData);

                    if (!worldStateData.Equals(default(WorldState)))
                    {
                        setWorldData = true;
                        //Debug.Log("Receiving World Data as a Client");
                    }
                }
                else if(trimmedData.Length == Marshal.SizeOf(typeof(PlayerInputData)))
                {
                    // Deserialize the received data into a struct
                    incomingData = BytesToStruct<PlayerInputData>(trimmedData);
                }                
            }
        }
        else if (serverUDPScript != null)
        {
            while (true)
            {
                byte[] data = new byte[1024];
                int recv = serverUDPScript.socket.ReceiveFrom(data, ref serverUDPScript.Remote); // Receive data from the remote endpoint

                byte[] trimmedData = new byte[recv];
                Array.Copy(data, trimmedData, recv);

                incomingData = BytesToStruct<GameStateManager.PlayerInputData>(trimmedData);
            }
        }
    }

    public static byte[] StructToBytes<T>(T data) where T : struct
    {
        int size = Marshal.SizeOf(data);
        byte[] bytes = new byte[size];

        IntPtr ptr = Marshal.AllocHGlobal(size); 
        Marshal.StructureToPtr(data, ptr, false);
        Marshal.Copy(ptr, bytes, 0, size);
        Marshal.FreeHGlobal(ptr);
        return bytes;
    }

    public static T BytesToStruct<T>(byte[] bytes) where T : struct
    {
        int size = Marshal.SizeOf(typeof(T));
        if (bytes.Length != size)
        {
            Debug.LogError($"Received bytes length ({bytes.Length}) does not match struct size ({Marshal.SizeOf(typeof(T))}).");
            throw new ArgumentException("Byte array size does not match struct size.");
        }

        IntPtr ptr = Marshal.AllocHGlobal(size); 
        Marshal.Copy(bytes, 0, ptr, size);       
        T data = Marshal.PtrToStructure<T>(ptr); 
        Marshal.FreeHGlobal(ptr);               

        return data;
    }

    public void SendWorldState()
    {
        Debug.Log("Send world state");

        playerID whatPlayerhasBall = playerID.NULL;
        GameObject temp = localPlayer.ball.GetComponent<Ball>().playerInPossesion;


        if (temp != null)
        {
            if(temp.name == "ClientPlayer")
            {
                whatPlayerhasBall = playerID.CLIENT;
            }
            else if(temp.name == "ServerPlayer")
            {
                whatPlayerhasBall = playerID.SERVER;
            }
        }

        WorldState data = new WorldState(localPlayer.gameObject.transform.position,
                                        remotePlayer.gameObject.transform.position,
                                        localPlayer.ball.gameObject.transform.position,
                                        localPlayer.ball.gameObject.transform.eulerAngles,
                                        localPlayer.ball.gameObject.GetComponent<Rigidbody>().velocity,
                                        whatPlayerhasBall);

        byte[] buffer = StructToBytes(data);


        if (serverUDPScript != null)
        {
            Debug.Log("WorldState sending to client");
            serverUDPScript.socket.SendTo(buffer, SocketFlags.None, serverUDPScript.Remote);
        }
    }

    private void SetWorldData()
    {
        localPlayer.gameObject.transform.position = worldStateData._clientPos;
        localPlayer.ball.gameObject.transform.position = worldStateData._ballPos;
        localPlayer.ball.gameObject.transform.eulerAngles = worldStateData._ballRot;
        localPlayer.ball.gameObject.GetComponent<Rigidbody>().velocity = worldStateData._ballVel;
        remotePlayer.gameObject.transform.position = worldStateData._serverPos;

        // POV from Client (remote is Server)

        switch (worldStateData._playerID)
        {
            case playerID.NULL:
                localPlayer.ball.GetComponent<Ball>().playerInPossesion = null;
                break;
            case playerID.SERVER:
                localPlayer.ball.GetComponent<Ball>().playerInPossesion = remotePlayer.gameObject;
                break;
            case playerID.CLIENT:
                localPlayer.ball.GetComponent<Ball>().playerInPossesion = localPlayer.gameObject;
                break;
        }

        Debug.Log("Setting World Data: \n" + worldStateData._serverPos + "\n" +
            worldStateData._ballPos + "\n" + worldStateData._clientPos + "\n" +
            worldStateData._playerID.ToString());
    }


}
