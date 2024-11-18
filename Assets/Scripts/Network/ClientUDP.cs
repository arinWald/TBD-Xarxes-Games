﻿using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;
using System;

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
    
    bool gameStarted = false;
    bool goToClientGame = false;

    public SceneChanger sceneChangerScript;

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
            // Change scene could not be done inside a thread
            if(goToClientGame)
            {
                sceneChangerScript.GoToClientGame();
                goToClientGame = false;

                Thread inGameReceive = new Thread(InGameReceive);
                inGameReceive.Start();
            }

            // Recieve Data
            //Debug.Log("Game Started");

            // Send Data
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

        while (true)
        {
            byte[] data = new byte[1024];
            int recv = socket.ReceiveFrom(data, ref Remote);

            clientText = "Message received from {0}: " + Remote.ToString();
            string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);

            //Debug.Log("MSG: " + receivedMessage);

            if (receivedMessage == "Ping UDP")
            {
                clientText = "Successfully connected to server";

                connectionSuccess = true;
            }
            else if (receivedMessage == "StartGame")
            {
                gameStarted = true;
                Debug.Log("Changing scene");

                // bool to call change scene outisde thread (look Update)
                goToClientGame = true;

                break;
            }
        }
    }

    void InGameReceive()
    {
        while (true)
        {
            // Allocate a buffer to receive data
            byte[] data = new byte[1024];
            int recv = socket.ReceiveFrom(data, ref Remote); // Receive data from the remote endpoint

            // Trim the byte array to match the received data length
            byte[] trimmedData = new byte[recv];
            Array.Copy(data, trimmedData, recv);

            // Deserialize the received data into a struct
            GameStateManager.PlayerInputData receivedMessage = GameStateManager.BytesToStruct<GameStateManager.PlayerInputData>(trimmedData);

            // Log the received data
            Debug.Log($"Received Data: \n" +
                      $"Horizontal Input: {receivedMessage.send_horizontalInput}, " +
                      $"Vertical Input: {receivedMessage.send_verticalInput}, " +
                      $"Space Input: {receivedMessage.send_spaceInput}, " +
                      $"Space Input Up: {receivedMessage.send_spaceInputUp}, " +
                      $"Space Input Down: {receivedMessage.send_spaceInputDown}, " +
                      $"Shift Input: {receivedMessage.send_shiftInput}, " +
                      $"Rotation Angle: {receivedMessage.send_rotationAngle}");
        }
    }

}
