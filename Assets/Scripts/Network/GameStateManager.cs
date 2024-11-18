using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class GameStateManager : MonoBehaviour
{
    public struct PlayerInputData
    {
        public float send_horizontalInput;
        public float send_verticalInput;
        public bool send_spaceInput;
        public bool send_spaceInputUp;
        public bool send_spaceInputDown;
        public bool send_shiftInput;

        public PlayerInputData(float h, float v, bool space, bool spaceUp, bool spaceDown, bool shift)
        {
            send_horizontalInput = h;
            send_verticalInput = v;
            send_spaceInput = space;
            send_spaceInputUp = spaceUp;
            send_spaceInputDown = spaceDown;
            send_shiftInput = shift;
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


    private void Start()
    {

    }

    private void Update()
    {

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