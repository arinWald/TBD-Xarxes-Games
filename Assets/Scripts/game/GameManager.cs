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
}
