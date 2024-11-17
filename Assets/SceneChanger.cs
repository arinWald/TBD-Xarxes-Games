using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public ServerUDP serverUDPscript;
    public ClientUDP clientUDPScript;
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

    public void GoToServerGame()
    {

        SceneManager.LoadScene("Server");
    }

    public void GoToClientGame()
    {
        SceneManager.LoadScene("Client");
    }

}
