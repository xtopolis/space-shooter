using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InterfaceKeyboardEvents : MonoBehaviour
{
    private bool controlsEnabled = false;
    private void Start()
    {
        Player.playerDied += ControlsEnabled;
    }
    // Update is called once per frame
    void Update()
    {
        if(controlsEnabled && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(1); // Game Scene
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void ControlsEnabled()
    {
        controlsEnabled = true;
    }
}
