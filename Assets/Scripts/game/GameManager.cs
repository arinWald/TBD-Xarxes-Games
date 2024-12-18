using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public int serverScore = 0;
    public int clientScore = 0;

    public GameObject serverTextObj;
    TextMeshProUGUI serverText;

    public GameObject clientTextObj;
    TextMeshProUGUI clientText;

    public GameObject winTextGameObject;
    TextMeshProUGUI winText;


    public GameObject ServerGO;
    public GameObject ClientGO;
    public GameObject BallGO;

    public Vector3 ServerResetPos;
    public Vector3 ClientResetPos;

    public Vector3 BallResetPos;

    private float countdownTime;
    public float countdownMaxTime;


    // Start is called before the first frame update
    void Start()
    {
        serverText = serverTextObj.GetComponent<TextMeshProUGUI>();
        clientText = clientTextObj.GetComponent<TextMeshProUGUI>();
        winText = winTextGameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        clientText.text = clientScore.ToString();
        serverText.text = serverScore.ToString();

        if (serverScore >= 5)
        {
            winTextGameObject.SetActive(true);
            winText.text = "Right Player Wins!";
        }
        else if (clientScore >= 5)
        {
            winTextGameObject.SetActive(true);
            winText.text = "Left Player Wins!";
        }
    }

    public void GoalReset()
    {
        ResetPos();
    }

    private void ResetPos()
    {
        ServerGO.transform.position = ServerResetPos;
        ClientGO.transform.position = ClientResetPos;
        BallGO.transform.position = BallResetPos;
        BallGO.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
