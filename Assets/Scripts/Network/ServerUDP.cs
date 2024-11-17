using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class ServerUDP : MonoBehaviour
{
    Socket socket;

    public GameObject UItextObj;
    TextMeshProUGUI UItext;

    [HideInInspector]
    public string serverText;
    [HideInInspector]
    public IPEndPoint ipep;
    [HideInInspector]
    public EndPoint Remote;


    void Start()
    {
        DontDestroyOnLoad(this);
        UItext = UItextObj.GetComponent<TextMeshProUGUI>();

        startServer();
    }

    public void startServer()
    {
        serverText = "Waiting for players...";

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipep = new IPEndPoint(IPAddress.Any, 9050);
        socket.Bind(ipep);

        Thread newConnection = new Thread(Receive);
        newConnection.Start();
    }

    void Update()
    {
        UItext.text = serverText;

    }


    public void Receive()
    {
        int recv;
        byte[] data = new byte[1024];

        serverText = "Waiting for new Client...";

        Remote = (EndPoint)(ipep);

        while (true)
        {
            recv = socket.ReceiveFrom(data, ref Remote);
            serverText = Encoding.ASCII.GetString(data, 0, recv);

            Thread answer = new Thread(() => SendPing(Remote));
            answer.Start();
            
            if (serverText == "Connected")
            {
                serverText = "Player 1 Joined with IP: " + Remote.ToString();
                break;
            }
        }
    }

    public void SendPing(EndPoint Remote)
    {
        byte[] data = new byte[1024];
        string welcome = "Ping UDP";

        data = Encoding.ASCII.GetBytes(welcome);
        socket.SendTo(data, SocketFlags.None, Remote);

    }

    void GameStarter()
    {
        Debug.Log("Starting Game");
        byte[] data = new byte[1024];
        string welcome = "StartGame";

        data = Encoding.ASCII.GetBytes(welcome);
        socket.SendTo(data, SocketFlags.None, Remote);
    }
}

