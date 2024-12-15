using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public GameManager manager;

    public GameObject serverPlayer;
    public GameObject clientPlayer;

    private PlayerMovement serverControls;
    private PlayerMovement clientControls;

    private GameObject playerInPossesion;

    private Rigidbody ballRb;

    private Material ballMat;
    
    void Start()
    {
        playerInPossesion = null;
        ballRb = gameObject.GetComponent<Rigidbody>();
        ballMat = gameObject.GetComponent<Renderer>().material;

        serverControls = serverPlayer.GetComponent<PlayerMovement>();
        clientControls = clientPlayer.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (playerInPossesion != null)
        {
            gameObject.transform.parent = playerInPossesion.transform.Find("BallSocket");
            transform.localPosition = Vector3.zero;
            ballRb.velocity = Vector3.zero;

            if (playerInPossesion.name == "ClientPlayer")
            {
                ballMat.SetColor("_EmissionColor", new Color(0,0.71f,1f, 1f) * 2.5f);
            }
            else if (playerInPossesion.name == "ServerPlayer")
            {
                ballMat.SetColor("_EmissionColor", new Color(1f, 0.65f, 0, 1f) * 2.5f);
            }
        }
        else
        {
            gameObject.transform.parent = null;
            ballMat.SetColor("_EmissionColor", Color.Lerp(ballMat.GetColor("_EmissionColor"), Color.white * 2.5f, 1 * Time.deltaTime));
        }
    }

    public void ChangePossesion(GameObject player)
    {
        playerInPossesion = player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerInPossesion == null)
        {
            if (other.tag == "client" && other.name == "BallSensor")
            {
                playerInPossesion = clientPlayer;
                clientControls.ballPossesion = true;
            }

            if (other.tag == "server" && other.name == "BallSensor")
            {
                playerInPossesion = serverPlayer;
                serverControls.ballPossesion = true;
            }
        }
        if (other.name == "Goal" && other.tag=="client")
        {
            manager.serverScore++;
        }
        if (other.name == "Goal" && other.tag == "server")
        {
            manager.clientScore++;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "client" && other.name == "SnatchHitbox" && clientControls.snatch)
        {
            playerInPossesion = clientPlayer;
            clientControls.ballPossesion = true;
        }

        if (other.tag == "server" && other.name == "SnatchHitbox" && serverControls.snatch)
        {
            playerInPossesion = serverPlayer;
            serverControls.ballPossesion = true;
        }
    }
}

