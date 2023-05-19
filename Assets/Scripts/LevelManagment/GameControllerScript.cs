using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void Damage(float damage);
}
public interface ILevel
{
    public void SetupLevel();
}

public interface IEnemyType
{
    public void Initialise(float damageToAdd, float healthToAdd);
}


public class GameControllerScript : MonoBehaviour
{
    [Tooltip("Number of the current level the player is on")]
    public int currentLevel;
    [Tooltip("Number of regular levels per Act")]
    public int numOfRegularLevels;
    [Tooltip("Number of Act Boss levels per chapter")]
    public int actsPerChapter;
    [Tooltip("Number of chapters in the whole game")]
    public int numOfChapters;
    [Tooltip("Number of Chapters the player has completed")]
    [HideInInspector] public int chaptersPassed;
    public int levelsPerChapter;
    public int totalLevels;
    public float timeSurvived;
    private GlobalControl globalControl;
    public enum LevelDifficulties {Easy, Medium, Hard};
    [HideInInspector] public LevelDifficulties levelDifficulty;
    public enum DoorPositions {North, East, South, West}
    public DoorPositions usedDoorPosition;

    public GameObject genericEnemy;
    public GameObject Fae;
    public GameObject Trow;
    public GameObject Piskie;
    public enum BuffTypes {Speed, LifeSteal, Strength, MaxHealth, InstantHealth}

    public Dictionary<LevelDifficulties,LevelProperty> levelProperties;

    // Start is called before the first frame update
    void Start()
    {
        
        InitialiseVariables();
        SetDoNotDestroys();
    }

    public void RestartController()
    {
        chaptersPassed = 0;
        currentLevel = 0;
        levelDifficulty = LevelDifficulties.Easy;
    }

    // Update is called once per frame
    void Update()
    {
        timeSurvived += Time.smoothDeltaTime;
        GlobalControl.Instance.timePassed = Mathf.RoundToInt(timeSurvived);
        GlobalControl.Instance.levelsCleared = currentLevel;
        
    }
    private void InitialiseVariables()
    {
        globalControl = GlobalControl.Instance;
        if (numOfChapters < 1 || numOfRegularLevels < 1 || actsPerChapter < 1)
        {
            throw new System.Exception("Cannot have Less than 1 Regular Levels / Act Boss Levels / Chapters.");
        }
        totalLevels = (numOfRegularLevels * actsPerChapter * numOfChapters) + (actsPerChapter * numOfChapters) + numOfChapters;
        levelsPerChapter = levelsPerChapter = totalLevels / numOfChapters;
        
        SetupLevelProperties();

        void SetupLevelProperties()
        {
            levelProperties = new Dictionary<LevelDifficulties, LevelProperty>();
            LevelProperty property = new LevelProperty();
            //Easy Level
            property.minFaes = 1;
            property.maxFaes = 3;

            property.minPiskies = 0;
            property.maxPiskies = 0;

            property.minTrows = 0;
            property.maxTrows = 1;

            levelProperties.Add(LevelDifficulties.Easy,property);

            //Medium Level
            property.minFaes = 2;
            property.maxFaes = 4;

            property.minPiskies = 0;
            property.maxPiskies = 2;

            property.minTrows = 0;
            property.maxTrows = 1;

            levelProperties.Add(LevelDifficulties.Medium, property);

            //Hard Level
            property.minFaes = 3;
            property.maxFaes = 5;

            property.minPiskies = 1;
            property.maxPiskies = 2;

            property.minTrows = 1;
            property.maxTrows = 3;

            levelProperties.Add(LevelDifficulties.Hard, property);
        }
    }
    public bool isActBossLevel()
    {
        if ((currentLevel - (levelsPerChapter * chaptersPassed)) % (numOfRegularLevels + 1) == 0)
        {
            return true;
        }
        return false;
    }
    public bool isChapterBossLevel()
    {
        if(currentLevel % levelsPerChapter == 0)
        {
            return true;
        }
        return false;
    }
    private void SetDoNotDestroys()
    {
        DontDestroyOnLoad(GameObject.Find("GameController"));
        DontDestroyOnLoad(Camera.main);
        DontDestroyOnLoad(GameObject.FindGameObjectWithTag("Player"));
        DontDestroyOnLoad(GameObject.FindGameObjectWithTag("UICanvas"));
        DontDestroyOnLoad(GameObject.Find("CM vcam1"));
    }
    public int NumberOfBuffs()
    {
        int result = 0;
        foreach (BuffTypes buff in System.Enum.GetValues(typeof(BuffTypes)))
        {
            result++;
        }
        return result;
    }
}
