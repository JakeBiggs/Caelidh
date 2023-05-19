using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [Tooltip("GameObject array for the prefabs of the Easy Regular Levels")]
    [SerializeField] private GameObject[] easyRegularLevels;
    [Tooltip("GameObject array for the prefabs of the Medium Regular Levels")]
    [SerializeField] private GameObject[] mediumRegularLevels;
    [Tooltip("GameObject array for the prefabs of the Hard Regular Levels")]
    [SerializeField] private GameObject[] hardRegularLevels;
    [Tooltip("GameObject Variable for the prefab of the Act Boss Level")]
    [SerializeField] private GameObject actBossLevel;
    [Tooltip("GameObject Variable for the prefab of the Chapter Boss Level")]
    [SerializeField] private GameObject chapterBossLevel;
    [Tooltip("GameObject Variable for the prefab of the Last Boss Level in the game")]
    [SerializeField] private GameObject endOfGameBossLevel;

    private GameControllerScript gc;

    public GameObject currentLevel;
    // Start is called before the first frame update
    void Start()
    {
        InitialiseVariables();
        SpawnLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void InitialiseVariables()
    {
        gc = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        
    }
    /// <summary>
    /// Spawns the correct level depending on what the current level number is.
    /// </summary>
    private void SpawnLevel()
    {
 
        // If the current level is the last level in the game, then spawn the last boss level
        if (gc.currentLevel == gc.totalLevels)
        {
            //print("SPAWN LAST BOSS");
            currentLevel = Instantiate(endOfGameBossLevel);
            currentLevel.transform.position = Vector3.zero;
            return;
        }
        // If the current level number can divide into levelPerChapter with no remainders (Meaning it is the last level of that chapter aka the boss level) spawn boss level
        if (gc.isChapterBossLevel())
        {
            // Spawn End of Chapter Boss
            //print("CHAPTER BOSS");
            currentLevel = Instantiate(chapterBossLevel);
            currentLevel.transform.position = Vector3.zero;
            gc.chaptersPassed++;
            return;
        }
        // If the current level number (converted to first chapter level numbers because it keeps maths easier)
        // can divide into number of Regular Levels per Act + 1 so it is the one after the last regular level
        if (gc.isActBossLevel())
        {
            // Spawn End of Act Boss
            //print("ACT BOSS");
            currentLevel = Instantiate(actBossLevel);
            currentLevel.transform.position = Vector3.zero;
            return;
        }
        // If all other checks come back false, then it must be a regular level and is spawned as such
        else
        {
            // Spawn regular Level
            //print("REGULAR");
            if(gc.levelDifficulty == GameControllerScript.LevelDifficulties.Easy)
            {
                print("Spawned easy level");
                currentLevel = Instantiate(easyRegularLevels[Random.Range(0, easyRegularLevels.Length)]);
                currentLevel.transform.position = Vector3.zero;
            }
            else if (gc.levelDifficulty == GameControllerScript.LevelDifficulties.Medium)
            {
                print("Spawned medium level");
                currentLevel = Instantiate(mediumRegularLevels[Random.Range(0, mediumRegularLevels.Length)]);
                currentLevel.transform.position = Vector3.zero;
            }
            else if(gc.levelDifficulty == GameControllerScript.LevelDifficulties.Hard)
            {
                print("Spawned hard level");
                currentLevel = Instantiate(hardRegularLevels[Random.Range(0, hardRegularLevels.Length)]);
                currentLevel.transform.position = Vector3.zero;
            }
        }

    }
}
