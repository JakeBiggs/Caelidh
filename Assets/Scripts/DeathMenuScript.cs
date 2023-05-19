using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class DeathMenuScript : MonoBehaviour
{
    public GameControllerScript gc;
    public int levelsCleared;
    public float timeSurvived;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI timeText;
    public GlobalControl GlobalControl;
    // Start is called before the first frame update
    void Start()
    {
        //gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        levelsCleared = GlobalControl.Instance.levelsCleared;
        timeSurvived = GlobalControl.Instance.timePassed;
        DisplayStats(levelsCleared, timeSurvived); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DisplayStats(int level, float time)
    {
        Debug.Log("Time: " + time.ToString());
        Debug.Log("Level: " + level.ToString());
        levelText.text = "Levels Cleared: " + level.ToString();
        timeText.text = "Time Survived: " + time.ToString()+"s";
    }
    public void RestartGame()
    {
        GameControllerScript gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        gc.currentLevel = 0;
        gc.chaptersPassed = 0;
        SceneManager.LoadScene("HubScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
