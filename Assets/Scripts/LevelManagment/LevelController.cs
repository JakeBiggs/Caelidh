using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelController : MonoBehaviour, ILevel
{
    public List<GameObject> enemies;
    private GameControllerScript gc;
    public List<GameObject> doors;
    private List<GameControllerScript.LevelDifficulties> unAssignedDifficulties;

    private int faeToSpawn;
    private int piskiesToSpawn;
    private int trowsToSpawn;
    //Start is called before the first frame update
    private void Awake()
    {

    }
    void Start()
    {
        InitialiseVariables();
        AssignDoorDifficulties();
        SetupLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void InitialiseVariables()
    {
        gc = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        enemies = new List<GameObject>();
        doors = FindDoors();
        unAssignedDifficulties = new List<GameControllerScript.LevelDifficulties>();
        foreach(GameControllerScript.LevelDifficulties difficulty in Enum.GetValues(typeof(GameControllerScript.LevelDifficulties)))
        {
            unAssignedDifficulties.Add(difficulty);
        }
    }
    public void SetupLevel()
    {
        Invoke(nameof(SpawnEnemies), 0.1f);
    }
    private void SpawnEnemies()
    {
        EnemiesToSpawn();
        List<GameObject> spawnPoints = new List<GameObject>();
        if (GameObject.Find("EnemySpawns") == null || GameObject.Find("EnemySpawns").transform.childCount == 0)
        {
            return;
        }
        for (int i = 0; i < GameObject.Find("EnemySpawns").transform.childCount; i++)
        {
            spawnPoints.Add(GameObject.Find("EnemySpawns").transform.GetChild(i).gameObject);
        }
        for(int i = 0; i < trowsToSpawn; i++)
        {
            GameObject enemy = Instantiate(gc.Trow);
            enemies.Add(enemy);
            int rand = UnityEngine.Random.Range(0, spawnPoints.Count);
            enemy.transform.position = spawnPoints[rand].transform.position;
            spawnPoints.RemoveAt(rand);
            enemy.GetComponent<IEnemyType>().Initialise(0.5f * ((int)gc.levelDifficulty), (gc.currentLevel * 0.1f) + (((int)gc.levelDifficulty + 1) * 0.1f));
        }
        for (int i = 0; i < piskiesToSpawn; i++)
        {
            GameObject enemy = Instantiate(gc.Piskie);
            for(int x = 0; x < enemy.transform.childCount; x++)
            {
                enemies.Add(enemy.transform.GetChild(x).gameObject);
            }
            int rand = UnityEngine.Random.Range(0, spawnPoints.Count);
            enemy.transform.position = spawnPoints[rand].transform.position;
            spawnPoints.RemoveAt(rand);
            for(int x = 0; x < enemy.transform.childCount; x++)
            {
                enemy.transform.GetChild(x).GetComponent<IEnemyType>().Initialise(0, (gc.currentLevel * 0.1f) + (((int)gc.levelDifficulty + 1) * 0.1f));
            }
                
        }
        for (int i = 0; i < faeToSpawn; i++)
        {
            GameObject enemy = Instantiate(gc.Fae);
            enemies.Add(enemy);
            int rand = UnityEngine.Random.Range(0, spawnPoints.Count);
            enemy.transform.position = spawnPoints[rand].transform.position;
            spawnPoints.RemoveAt(rand);
            //enemy.GetComponent<IEnemyType>().Initialise((gc.currentLevel * 0.1f) + (((int)gc.levelDifficulty + 1) * 0.1f), (gc.currentLevel * 0.1f) + (((int)gc.levelDifficulty + 1) * 0.1f));
            enemy.GetComponent<IEnemyType>().Initialise(0.5f * ((int)gc.levelDifficulty), (gc.currentLevel * 0.1f) + (((int)gc.levelDifficulty + 1) * 0.1f));
        }
    }

    public void CheckUnlockRequirements()
    {
        if (enemies.Count == 0) UnlockDoors();
    }
    private void UnlockDoors()
    {
        foreach (GameObject door in doors)
        {
            DoorScript script = door.GetComponent<DoorScript>();
            script.canEnter = true;
        }
    }
    private List<GameObject> FindDoors()
    {
        List<GameObject> searchList = new List<GameObject>();
        foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            DoorScript script = go.GetComponent<DoorScript>();
            if (script != null)
            {
                searchList.Add(go);
            }
        }
        return searchList;
    }
    private void AssignDoorDifficulties()
    {
        foreach(GameObject door in doors)
        {
            DoorScript script = door.GetComponent<DoorScript>();
            if (!script.isDisabled)
            {
                script.roomDifficulty = unAssignedDifficulties[UnityEngine.Random.Range(0, unAssignedDifficulties.Count)];
                unAssignedDifficulties.Remove(script.roomDifficulty);
            }
        }
    }

    private void EnemiesToSpawn()
    {
        //float difficultyMultiplier = ((gc.currentLevel / 50) * ((int)gc.levelDifficulty + 1));
        float difficultyMultiplier = 0;

        //print("Difficulty Multiplier: " + difficultyMultiplier);

        faeToSpawn = Mathf.RoundToInt(UnityEngine.Random.Range(gc.levelProperties[gc.levelDifficulty].minFaes + difficultyMultiplier, gc.levelProperties[gc.levelDifficulty].maxFaes + difficultyMultiplier));
        piskiesToSpawn = Mathf.RoundToInt(UnityEngine.Random.Range(gc.levelProperties[gc.levelDifficulty].minPiskies + difficultyMultiplier, gc.levelProperties[gc.levelDifficulty].maxPiskies + difficultyMultiplier));
        trowsToSpawn = Mathf.RoundToInt(UnityEngine.Random.Range(gc.levelProperties[gc.levelDifficulty].minTrows + difficultyMultiplier, gc.levelProperties[gc.levelDifficulty].maxTrows + difficultyMultiplier));
        /*print("Fae to spawn: " + faeToSpawn);
        print("Piskies to spawn: " + piskiesToSpawn);
        print("Trows to spawn: " + trowsToSpawn);*/
    }
}
