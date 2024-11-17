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
    private bool send_shiftInput = false;

    [HideInInspector]
    public ClientUDP clientUDPScript;
    [HideInInspector]
    public ServerUDP serverUDPScript;

    private GameObject player;

    private void Start()
    {
        player = this.gameObject;
}

    private void Update()
    {

    }
}
