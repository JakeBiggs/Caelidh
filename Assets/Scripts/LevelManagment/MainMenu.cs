using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("HubScene");
        Debug.Log("Play game");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
