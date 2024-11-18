using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToScenes : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Server" || SceneManager.GetActiveScene().name == "Client")
        {
            if (audioSource != null && audioSource.enabled)
            {
                audioSource.enabled = false;
            }
        }
        else
        {
            if (audioSource != null && !audioSource.enabled)
            {
                audioSource.enabled = true;
            }
        }
    }
}
