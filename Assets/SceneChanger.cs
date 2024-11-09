using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneChanger : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToClientScene()
    {
        SceneManager.LoadScene("ClientConnect");
    }

    public void GoToServerScene()
    {
        SceneManager.LoadScene("ServerConnect");
    }

    public void GoToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }


}
