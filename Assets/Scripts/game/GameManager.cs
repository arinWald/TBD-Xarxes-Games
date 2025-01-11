using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


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

    public float countdownTime;
    public int countdownMaxTime = 4;
    private bool isCountdown = false;

    public TextMeshProUGUI countdownText;


    void Start()
    {
        serverText = serverTextObj.GetComponent<TextMeshProUGUI>();
        clientText = clientTextObj.GetComponent<TextMeshProUGUI>();
        winText = winTextGameObject.GetComponent<TextMeshProUGUI>();
    }

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
        if (serverScore >= 5 || clientScore >= 5)
        {
            return;
        }
        StartCoroutine(WaitAndReset());
    }

    private IEnumerator WaitAndReset()
    {
        ResetPos();

        Time.timeScale = 0.0f;

        // Display countdown
        for (int i = countdownMaxTime; i > 0; i--)
        {
            countdownText.text = (i-1).ToString();
            if(i == 1)
            {
                countdownText.text = "GO!";
            }

            countdownText.gameObject.transform.parent.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(1f);
        }

        countdownText.gameObject.transform.parent.gameObject.SetActive(false);

        Time.timeScale = 1.0f;
    }

    private void ResetPos()
    {

        ServerGO.transform.position = ServerResetPos;
        ClientGO.transform.position = ClientResetPos;

        BallGO.transform.SetParent(null);
        BallGO.GetComponent<Rigidbody>().velocity = Vector3.zero;
        BallGO.transform.position = BallResetPos;
    }
}
