using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.TextCore;
using UnityEngine.UIElements;

public class DoorScript : MonoBehaviour
{
    private GameControllerScript gc;
    public GameControllerScript.DoorPositions doorPosition;
    public GameObject door;
    public bool canEnter;
    public bool isDisabled;
    public GameControllerScript.DoorPositions oppositeDoor;
    public GameControllerScript.LevelDifficulties roomDifficulty;
    public Buff reward;
    public static Dictionary<GameControllerScript.BuffTypes, float> rewardRanges;
    public PlayerControllerScript pc;
    private GlobalControl globalControl;

    private void Awake()
    {
        InitialiseVariables();
        if (gc.usedDoorPosition == doorPosition)
        {
            isDisabled = true;
            return;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SetUpRewardRanges();
        CreateReward();
        SetUpText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (canEnter && !isDisabled && collision.gameObject.tag == "Player")
        {
            pc.ApplyBuff(reward);
            pc.receivedBuffStacks[reward.type] += ((int)roomDifficulty + 1);
            gc.levelDifficulty = roomDifficulty;
            gc.currentLevel++;
            
            gc.usedDoorPosition = oppositeDoor;
            GameObject.Find("Player").transform.position = newPlayerPos();
            SceneManager.LoadScene("LevelScene");
        }
    }

    private void SetUpRewardRanges()
    {
        rewardRanges = new Dictionary<GameControllerScript.BuffTypes, float>();

        //Speed
        rewardRanges.Add(GameControllerScript.BuffTypes.Speed, 0.1f);
        //Instant Health
        rewardRanges.Add(GameControllerScript.BuffTypes.InstantHealth, 0.5f);
        //Strength
        rewardRanges.Add(GameControllerScript.BuffTypes.Strength, 0.25f);
        //Max Health
        rewardRanges.Add(GameControllerScript.BuffTypes.MaxHealth, 0.5f);
        //Life Steal
        rewardRanges.Add(GameControllerScript.BuffTypes.LifeSteal, 0.025f);

    }
    private Vector3 newPlayerPos()
    {
        foreach(GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            DoorScript script = go.GetComponent<DoorScript>();
            if(script != null)
            {
                //print("Found: " + script.doorPosition + " Needs: " + oppositeDoor);
                if(script.doorPosition == oppositeDoor)
                {
                    return (go.transform.position + (Vector3.Normalize(-go.transform.position) * 2));
                }
            }
        }
        return new Vector3();
    }
    private void InitialiseVariables()
    {
        globalControl = GlobalControl.Instance;

        oppositeDoor = doorPosition + 2; if (((int)oppositeDoor) > 3) oppositeDoor -= 4;
        gc = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        pc = GameObject.Find("Player").GetComponent<PlayerControllerScript>();
    }
    private void SetUpText()
    {
        TMPro.TextMeshPro text = transform.GetChild(1).GetComponent<TMPro.TextMeshPro>();
        if (isDisabled)
        {
            text.enabled = false;
            return;
        }
        text.text = roomDifficulty.ToString() + " - " + reward.type.ToString() + " Stacks \n" + ((int)roomDifficulty + 1);
    }
    private void CreateReward()
    {
        List<GameControllerScript.BuffTypes> buffs = new List<GameControllerScript.BuffTypes>();
        foreach(GameControllerScript.BuffTypes buff in System.Enum.GetValues(typeof(GameControllerScript.BuffTypes)))
        {
            buffs.Add(buff);
        }
        reward.type = buffs[(UnityEngine.Random.Range(0, gc.NumberOfBuffs()))];

        //reward.type = GameControllerScript.BuffTypes.LifeSteal;

        float intensity = rewardRanges[reward.type] * (((int)roomDifficulty) + 1);
        if (((int)roomDifficulty + 1) > pc.buffStackLimit - pc.receivedBuffStacks[reward.type]) intensity = rewardRanges[reward.type] * (pc.buffStackLimit - pc.receivedBuffStacks[reward.type]);
        reward.affectIntensity = intensity;
        reward.affectDuration = -1;
        reward.affectDelay = 0;
        reward.isCompounding = false;
        //print("Max Health to add is " + reward.affectIntensity + " I am difficulty " + (((int)roomDifficulty) + 1));
    } 
}
