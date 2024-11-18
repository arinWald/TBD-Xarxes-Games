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


    }

    private void Update()
    {
        SetPlayerControls();

        Thread sendPlayerControlsThread = new Thread(SendPlayerControls);
        sendPlayerControlsThread.Start();

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

        }
        else if(serverUDPScript != null)
        {
            Debug.Log("Controls sending to client");
            serverUDPScript.socket.SendTo(buffer, SocketFlags.None, serverUDPScript.Remote);
        }
        
    }

    // Listen for controls
    void ReceivePlayerControls()
    {

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