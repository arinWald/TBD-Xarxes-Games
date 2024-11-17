using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class ClientUDP : MonoBehaviour
{
    [HideInInspector]
    public Socket socket;
    [HideInInspector]
    public IPEndPoint ipep;
    [HideInInspector]
    public EndPoint Remote;

    public GameObject UItextObj;
    public TMP_InputField IPInputField;
    TextMeshProUGUI UItext;
    string clientText;
    string serverIP = "192.168.1.53";

    [HideInInspector]
    public bool connectionSuccess = false;
    [HideInInspector]
    bool gameStarted = false;

    SceneChanger sceneChangerScript;

    void Start()
    {
        DontDestroyOnLoad(this);
        UItext = UItextObj.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        UItext.text = clientText;

        if (connectionSuccess && gameStarted)
        {
            // Recieve Data
            Debug.Log("Game Started");
            // Send Data
        }
        else if (connectionSuccess)
        {
            byte[] data = new byte[1024];
            int recv = socket.ReceiveFrom(data, ref Remote);

            string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);

            if(receivedMessage == "StartGame")
            {
                gameStarted = true;
                sceneChangerScript.GoToClientGame();
            }
        }
    }


    public void StartClient()
    {
       
        if (!string.IsNullOrEmpty(IPInputField.text))
        {
            serverIP = IPInputField.text;
        }

        Thread connectionStablisher = new Thread(Send);
        connectionStablisher.Start();
    }

    void Send()
    {
        ipep = new IPEndPoint(IPAddress.Parse(serverIP), 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //byte[] data = Encoding.ASCII.GetBytes("Player 1 Joined with IP: " + GetLocalIPAddress());
        //socket.SendTo(data, SocketFlags.None, ipep);

        byte[] data = Encoding.ASCII.GetBytes("Connected");
        socket.SendTo(data, SocketFlags.None, ipep);

        Thread receive = new Thread(Receive);
        receive.Start();
    }

    void Receive()
    {
        Remote = (EndPoint)(ipep);

        byte[] data = new byte[1024];
        int recv = socket.ReceiveFrom(data, ref Remote);

        clientText = "Message received from {0}: " + Remote.ToString();
        string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);

        Debug.Log("MSG: " + receivedMessage);

        if (receivedMessage == "Ping UDP")
        {
            clientText = "Successfully connected to server";

            connectionSuccess = true;
        }
    }
}
