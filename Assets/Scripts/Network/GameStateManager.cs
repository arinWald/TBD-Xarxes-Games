using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class GameStateManager : MonoBehaviour
{
    private float send_horizontalInput = 0f;
    private float send_verticalInput = 0f;
    private bool send_spaceInput = false;
    private bool send_spaceInputUp = false;
    public bool send_spaceInputDown = false;
    private bool send_shiftInput = false;

    [HideInInspector]
    public ClientUDP clientUDPScript;
    [HideInInspector]
    public ServerUDP serverUDPScript;

    public PlayerMovement localPlayer;
    public PlayerMovement remotePlayer;


    private void Start()
    {

    }

    private void Update()
    {

    }

    // Assign the received controls
    void SetPlayerControls()
    {
        remotePlayer.horizontalInput = send_horizontalInput;
        remotePlayer.verticalInput = send_verticalInput;
    }

    // Send current controls
    void SendPlayerControls()
    {
        
    }

    // Listen for controls
    void ReceivePlayerControls()
    {

    }
}