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

    public GameObject UItextObj;          // Per mostrar missatges
    public TMP_InputField IPInputField;   // Camp de text per introduir IP
    TextMeshProUGUI UItext;
    string clientText;
    string serverIP = "192.168.1.53";        // IP per defecte

    SceneChanger sceneChangerScript;

    void Start()
    {
        DontDestroyOnLoad(this);
        UItext = UItextObj.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        UItext.text = clientText;
    }

    // Funció cridada pel botó "Connect To Server"
    public void StartClient()
    {
        // Obtenim la IP del InputField si ha estat introduïda
        if (!string.IsNullOrEmpty(IPInputField.text))
        {
            serverIP = IPInputField.text;  // Agafem la IP del camp de text
        }

        // Iniciem el procés de connexió en un nou fil
        Thread mainThread = new Thread(Send);
        mainThread.Start();
    }

    void Send()
    {
        // Establim la comunicació amb l'endpoint del servidor
        ipep = new IPEndPoint(IPAddress.Parse(serverIP), 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Enviem un "Hello World UDP" per establir la connexió amb el servidor
        byte[] data = Encoding.ASCII.GetBytes("Player 1 Joined with IP: " + serverIP);
        socket.SendTo(data, SocketFlags.None, ipep);

        // Comencem a rebre la resposta del servidor
        Thread receive = new Thread(Receive);
        receive.Start();
    }

    void Receive()
    {
        Remote = (EndPoint)(ipep);

        byte[] data = new byte[1024];
        int recv = socket.ReceiveFrom(data, ref Remote);

        // Mostrem "Ping Received" quan rebem la resposta del servidor
        clientText = "Message received from {0}: " + Remote.ToString();
        string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);

        Debug.Log("MSG: " + receivedMessage);

        // Si el missatge rebut és el "Ping UDP", mostrem "Ping Received" a la consola
        if (receivedMessage == "Ping UDP")
        {
            clientText = "Successfully connected to server";
        }
        else if(receivedMessage == "StartGame")
        {
            sceneChangerScript.GoToClientGame();
        }
    }
}
