using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public GameObject serverPlayer;
    public GameObject clientPlayer;

    private PlayerMovement serverControls;
    private PlayerMovement clientControls;

    private GameObject playerInPossesion;

    private Rigidbody ballRb;

    void Start()
    {
        playerInPossesion = null;
        ballRb = gameObject.GetComponent<Rigidbody>();

        serverControls = serverPlayer.GetComponent<PlayerMovement>();
        clientControls = clientPlayer.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInPossesion != null)
        {
            gameObject.transform.parent = playerInPossesion.transform.Find("BallSocket");
            transform.localPosition = Vector3.zero;
            ballRb.velocity = Vector3.zero;
        }
        else
        {
            gameObject.transform.parent = null;
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
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "client" && other.name == "SnatchHitbox" && clientControls.snatch)
        {
            //Debug.Log("YEP");
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

