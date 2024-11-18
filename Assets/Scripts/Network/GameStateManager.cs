using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;
using System;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    [Serializable]
    public struct PlayerInputData
    {
        public float send_horizontalInput;
        public float send_verticalInput;
        public bool send_spaceInput;
        public bool send_spaceInputUp;
        public bool send_spaceInputDown;
        public bool send_shiftInput;
        public float send_rotationAngle;

        public PlayerInputData(float h, float v, bool space, bool spaceUp, bool spaceDown, bool shift, float rotAng)
        {
            send_horizontalInput = h;
            send_verticalInput = v;
            send_spaceInput = space;
            send_spaceInputUp = spaceUp;
            send_spaceInputDown = spaceDown;
            send_shiftInput = shift;
            send_rotationAngle = rotAng;
        }
    };


    [HideInInspector]
    public ClientUDP clientUDPScript;
    [HideInInspector]
    public ServerUDP serverUDPScript;

    public PlayerMovement localPlayer;
    public PlayerMovement remotePlayer;

    PlayerInputData incomingData;
    PlayerInputData pendingToSendData;

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
    }

    // Assign the received controls
    void SetPlayerControls()
    {
        remotePlayer.horizontalInput = incomingData.send_horizontalInput;
        remotePlayer.verticalInput = incomingData.send_verticalInput;
        remotePlayer.spaceInput = incomingData.send_spaceInput;
        remotePlayer.spaceInputUp = incomingData.send_spaceInputUp;
        remotePlayer.spaceInputDown = incomingData.send_spaceInputDown;
        remotePlayer.shiftInput = incomingData.send_shiftInput;
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
            localPlayer.shiftInput,
            localPlayer.rotationAngle
            );

            byte[] buffer = StructToBytes(data);


            if (IAmClient && clientUDPScript != null)
            {
                Debug.Log("Controls sending to client");
                clientUDPScript.socket.SendTo(buffer, SocketFlags.None, clientUDPScript.Remote);
            }
            else if (serverUDPScript != null)
            {
                Debug.Log("Controls sending to client");
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
                int recv = clientUDPScript.socket.ReceiveFrom(data, ref clientUDPScript.Remote); // Receive data from the remote endpoint

                // Trim the byte array to match the received data length
                byte[] trimmedData = new byte[recv];
                Array.Copy(data, trimmedData, recv);

                // Deserialize the received data into a struct
                incomingData = GameStateManager.BytesToStruct<GameStateManager.PlayerInputData>(trimmedData);
            }
        }
        else if (serverUDPScript != null)
        {
            while (true)
            {
                // Allocate a buffer to receive data
                byte[] data = new byte[1024];
                int recv = serverUDPScript.socket.ReceiveFrom(data, ref serverUDPScript.Remote); // Receive data from the remote endpoint

                // Trim the byte array to match the received data length
                byte[] trimmedData = new byte[recv];
                Array.Copy(data, trimmedData, recv);

                // Deserialize the received data into a struct
                incomingData = BytesToStruct<GameStateManager.PlayerInputData>(trimmedData);

                //Debug.Log($"Received Data: \n" +
                //          $"Horizontal Input: {incomingData.send_horizontalInput}, " +
                //          $"Vertical Input: {incomingData.send_verticalInput}, " +
                //          $"Space Input: {incomingData.send_spaceInput}, " +
                //          $"Space Input Up: {incomingData.send_spaceInputUp}, " +
                //          $"Space Input Down: {incomingData.send_spaceInputDown}, " +
                //          $"Shift Input: {incomingData.send_shiftInput}, " +
                //          $"Rotation Angle: {incomingData.send_rotationAngle}");
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
            throw new ArgumentException("Byte array size does not match struct size.");

        IntPtr ptr = Marshal.AllocHGlobal(size); 
        Marshal.Copy(bytes, 0, ptr, size);       
        T data = Marshal.PtrToStructure<T>(ptr); 
        Marshal.FreeHGlobal(ptr);               

        return data;
    }

}