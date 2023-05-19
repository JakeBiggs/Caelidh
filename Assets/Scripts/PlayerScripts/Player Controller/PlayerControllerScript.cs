using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Diagnostics;
using UnityEditor;

public class PlayerControllerScript : MonoBehaviour
{
    #region Movement Variables
    [Tooltip("Sets base movement speed for the player")]
    public float movementSpeed;
    [HideInInspector] public Rigidbody rb;
    [Tooltip("A layermask of all layers that if hit by the raycast for keyboard and mouse player rotation will make the player look at the hit")]
    public LayerMask lookAtLayers;
    public bool canMove;
    public bool canRoll;
    #endregion
    #region Input Variables
    [Tooltip("Set true if main input is controller. This determines movement and look mode, so chose carefully")]
    public bool usingController;
    [Tooltip("A Vector2 that tracks the input from the leftstick of the gamepad.")]
    public Vector2 movementGamepadInput;
    [Tooltip("A Vector 2 that tracks the input of WASD Keys")]
    public Vector2 movementKBAMInput;
    [Tooltip("A Vector2 that tracks the input from the rightstick of the gamepad.")]
    private Vector2 rotationInput;
    [Tooltip("Tracks the mouse position relative to the screen")]
    private Vector2 mousePosition;
    #endregion
    #region Evading Variables
    private float evadeTimeStart = 0;
    public float evadeCooldown = 1f;
    public float evadeVelocity;
    public float evadeIFrameDuration;
    private Vector3 evadeVel;

    #endregion
    #region Controllers & Components
    public PlayerController controller;
    private HealthManagement healthManagement;
    private GameControllerScript gc;
    private AttackScript attackScript;
    [SerializeField] private Animator playerAnimation;
    private CardSystem cardSystem; 
    #endregion
    #region Buff Variables
    public Dictionary<GameControllerScript.BuffTypes, ValueTuple<List<float>, List<float>>> buffLists;

    public List<GameControllerScript.BuffTypes> instantBuffs;

    public Dictionary<GameControllerScript.BuffTypes, int> receivedBuffStacks;

    public int buffStackLimit;

    public int hitsTowardsLS;
    #endregion




    private void OnEnable()
    {
        controller.Gameplay.Enable();
    }
    private void OnDisable()
    {
        controller.Gameplay.Disable();
    }

    private void Awake()
    {
        controller = new PlayerController();

        controller.Gameplay.MovementGamepad.performed += context => movementGamepadInput = context.ReadValue<Vector2>();
        controller.Gameplay.MovementGamepad.canceled += context => movementGamepadInput = Vector2.zero;

        controller.Gameplay.MovementKBAM.performed += context => movementKBAMInput = context.ReadValue<Vector2>();
        controller.Gameplay.MovementKBAM.canceled += context => movementKBAMInput = Vector2.zero;

        controller.Gameplay.Rotation.performed += context => rotationInput = context.ReadValue<Vector2>();
        controller.Gameplay.Rotation.canceled += context => rotationInput = Vector2.zero;


        controller.Gameplay.MousePosition.performed += context => mousePosition = context.ReadValue<Vector2>();
        controller.Gameplay.MousePosition.canceled += context => mousePosition = Vector2.zero;

    }
    void Start()
    {
        InitialiseVariables();
    }
    private void InitialiseVariables()
    {
        rb = GetComponent<Rigidbody>();
        healthManagement = GetComponent<HealthManagement>();
        gc = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        attackScript = GetComponent<AttackScript>();
        playerAnimation =transform.GetChild(0).GetComponent<Animator>();
        cardSystem = GetComponent<CardSystem>();
        canMove = true;
        canRoll = true;
        receivedBuffStacks = new Dictionary<GameControllerScript.BuffTypes, int>();

        foreach(GameControllerScript.BuffTypes type in Enum.GetValues(typeof(GameControllerScript.BuffTypes)))
        {
            receivedBuffStacks.Add(type, 0);
        }
        SetupBuffs();

        /* This funtion is the result of day and night research and development to make the most efficient buff system I could
           It does a for loop for every buff type we have, and it creates an empty Tuple of 2 float lists then sets each list
           to a new list to initialise them, then adds the tuple to a dictionary with the key of the buff type it represents
           We have Tuples over lists because Tuples can represent both the Additive and Compounding values of effects
           and we have ValueTuples over Tuples because ValueTuples allows you to edit the values within, which Tuples do not */


        void SetupBuffs()
        {
            buffLists = new Dictionary<GameControllerScript.BuffTypes, ValueTuple<List<float>, List<float>>>();
            foreach (GameControllerScript.BuffTypes buff in Enum.GetValues(typeof(GameControllerScript.BuffTypes)))
            {
                ValueTuple<List<float>, List<float>> tup = new ValueTuple<List<float>, List<float>>();
                tup.Item1 = new List<float>();
                tup.Item2 = new List<float>();
                buffLists.Add(buff, tup);
            }
        }
    }
    private void FixedUpdate()
    {
        if (canMove) MovePlayer();
        //Rotate();
    }
    private void Update()
    {
#if UNITY_EDITOR
        DebugInputs();
#endif
    }

    void OnSwitchInputMode()
    {
        usingController = !usingController;
    }

    void OnSwingSword()
    {
    }

    #region BuffSystem
    public void ApplyBuff(Buff buff)
    {
        if (buff.affectDelay < 0) throw new System.Exception("Cannot have a negative delay. This would make the effect never be added. Might aswell not call the function, lol :)");

        if (instantBuffs.Contains(buff.type))
        {
            StartCoroutine(HandleInstantBuff(buff));
            return;
        }
        StartCoroutine(HandleBuff(buff));
    }



    public IEnumerator HandleBuff(Buff buff)
    {
        if (!buff.isCompounding)
        {
            yield return new WaitForSeconds(buff.affectDelay);
            buffLists[buff.type].Item1.Add(buff.affectIntensity);
            if (buff.affectDuration < 0)
            {
                yield break;
            }
            yield return new WaitForSeconds(buff.affectDuration);
            buffLists[buff.type].Item1.Remove(buff.affectIntensity);
        }
        else
        {
            yield return new WaitForSeconds(buff.affectDelay);
            buffLists[buff.type].Item2.Add(buff.affectIntensity);
            if (buff.affectDuration < 0)
            {
                yield break;
            }
            yield return new WaitForSeconds(buff.affectDuration);
            buffLists[buff.type].Item2.Remove(buff.affectIntensity);
        }
    }

    public IEnumerator HandleInstantBuff(Buff buff)
    {
        // Giving Instant Health
        if(buff.type == GameControllerScript.BuffTypes.InstantHealth)
        {
            healthManagement.GiveHealth(buff.affectIntensity);
        }

        // Setting Max Health
        if(buff.type == GameControllerScript.BuffTypes.MaxHealth)
        {
            if (!buff.isCompounding)
            {
                yield return new WaitForSeconds(buff.affectDelay);
                healthManagement.setMaxHealth(healthManagement.maxHealth + buff.affectIntensity);
                if (buff.affectDuration < 0)
                {
                    yield break;
                }
                yield return new WaitForSeconds(buff.affectDuration);
                healthManagement.setMaxHealth(healthManagement.maxHealth - buff.affectIntensity);
            }
            else
            {
                yield return new WaitForSeconds(buff.affectDelay);
                float healthAdded = (healthManagement.maxHealth * buff.affectIntensity) - healthManagement.maxHealth;
                healthManagement.setMaxHealth(healthManagement.maxHealth + healthAdded);
                if (buff.affectDuration < 0)
                {
                    yield break;
                }
                yield return new WaitForSeconds(buff.affectDuration);
                healthManagement.setMaxHealth(healthManagement.maxHealth - healthAdded);
            }
        }
    }

    public float LifeStealMultiplier()
    {
        float result = 0;
        foreach(float ls in buffLists[GameControllerScript.BuffTypes.LifeSteal].Item1)
        {
            result += ls;
        }
        foreach (float ls in buffLists[GameControllerScript.BuffTypes.LifeSteal].Item2)
        {
            result *= ls;
        }
        return result;
    }

    public int hitsForLS()
    {
        int result = Mathf.RoundToInt(1 / (DoorScript.rewardRanges[GameControllerScript.BuffTypes.LifeSteal] * receivedBuffStacks[GameControllerScript.BuffTypes.LifeSteal]));
        if(receivedBuffStacks[GameControllerScript.BuffTypes.LifeSteal] == 0)
        {
            result = 0;
        }
        return result;
    }


    #endregion
    #region Movement

    /// <summary>
    /// Will move the player depending on their inputs and also independantly if they are on Mouse and Keyboard or Controller 
    /// (Because otherwise inputs overlap and alternate between 1 and 0 when pressed)
    /// </summary>
    private void MovePlayer()
    {
        if (usingController)
        {
            rb.velocity = new Vector3(rb.velocity.x + (movementGamepadInput.x * movementSpeed * Time.deltaTime) * MovementMultiplier(),
                                      rb.velocity.y,
                                      rb.velocity.z + (movementGamepadInput.y * movementSpeed * Time.deltaTime) * MovementMultiplier());
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x + (movementKBAMInput.x * movementSpeed * Time.deltaTime) * MovementMultiplier(),
                                      rb.velocity.y,
                                      rb.velocity.z + (movementKBAMInput.y * movementSpeed * Time.deltaTime) * MovementMultiplier());
        }
    }

    void OnEvade()
    {
        if (canRoll && healthManagement.playerLive)
        {
            if (Time.time > evadeTimeStart + evadeCooldown)
            {
                if (!usingController)
                {
                    playerAnimation.SetTrigger("Roll");
                    evadeVel = new Vector3(rb.velocity.x + (movementKBAMInput.x * evadeVelocity) * MovementMultiplier(),
                                           rb.velocity.y,
                                           rb.velocity.z + (movementKBAMInput.y * evadeVelocity) * MovementMultiplier());

                    rb.velocity += evadeVel;
                    healthManagement.IFramesMethod(evadeIFrameDuration);

                    evadeTimeStart = Time.time;
                }
                else if (usingController)
                {
                    playerAnimation.SetTrigger("Roll");
                    evadeVel = new Vector3(rb.velocity.x + (movementGamepadInput.x * evadeVelocity) * MovementMultiplier(),
                                           rb.velocity.y,
                                           rb.velocity.z + (movementGamepadInput.y * evadeVelocity) * MovementMultiplier());

                    rb.velocity += evadeVel;
                    healthManagement.IFramesMethod(evadeIFrameDuration);

                    evadeTimeStart = Time.time;
                }
                print("Evaded");
            }
        }


    }

    /// <summary>
    /// Multiplies all modifiers to 1 compounding ontop of eachother to end up with an overall movement multiplier every frame the player's velocity is updated.
    /// </summary>
    /// <returns></returns>
    private float MovementMultiplier()
    {
        //Sets result to 1 initially to represent a default movement multiplier
        float result = 1;
        foreach (float modifier in buffLists[GameControllerScript.BuffTypes.Speed].Item1)
        {
            result += modifier;
        }
        // Goes through a list of modifier values and multiplies them to the 1 to get an end result of an overall movement modifier
        foreach (float modifier in buffLists[GameControllerScript.BuffTypes.Speed].Item2)
        {
            result *= modifier;
        }
        return result;
    }

    /// <summary>
    /// Applies a Speed Modifier to the Player (Compounds with other active modifiers). If the duration is < 0 then the effect is infinite.
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="speed"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    #endregion
    #region Rotation

    /// <summary>
    /// Will rotate the player in 2 different ways depending if they are on controller or Mouse and Keyboard.
    /// </summary>
    public void Rotate()
    {
        // Runs certain code depending on if player is on controller or not
        if (usingController)
        {
            // If player is not giving aim input, make player face towards their movement direction
            if (rotationInput.x == 0 && rotationInput.y == 0)
            {
                transform.LookAt(transform.position + new Vector3(rb.velocity.x, 0, rb.velocity.z));
            }
            else
            {
                // Makes player rotation match the position of the right analog stick
                transform.LookAt(transform.position + new Vector3(rotationInput.x, 0, rotationInput.y));
            }
        }
        else
        {
            // Casts a ray from camera, if it hits a certain layer, will make the player look at it
            Camera cam = Camera.main;
            Vector3 hitPoint;
            Vector3 MousePos = mousePosition;
            MousePos.z = 100f;
            MousePos = cam.ScreenToWorldPoint(MousePos);
            Debug.DrawRay(cam.transform.position, MousePos - cam.transform.position, Color.blue);
            Ray ray = cam.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, lookAtLayers, QueryTriggerInteraction.Ignore))
            {
                Debug.DrawRay(hit.point, Vector3.up * 100, Color.blue);
                hitPoint = hit.point; hitPoint.y = transform.position.y;
                transform.LookAt(hitPoint, Vector3.up);
            }
        }
    }
    #endregion
    private void DebugInputs()
    {
        if (Keyboard.current.mKey.wasPressedThisFrame) healthManagement.SetNumOfHearts(41);
        if (Keyboard.current.lKey.wasPressedThisFrame) healthManagement.Damage(5);
        if(Keyboard.current.rKey.wasPressedThisFrame) healthManagement.setMaxHealth(0);
        if (Keyboard.current.zKey.wasPressedThisFrame) ApplyBuff(new Buff(GameControllerScript.BuffTypes.Strength, 100, 0, -1, true));
        if (Keyboard.current.xKey.wasPressedThisFrame) hitsForLS();
        //if (Keyboard.current.cKey.wasPressedThisFrame) usingController = !usingController;
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            gc.usedDoorPosition = GameControllerScript.DoorPositions.East;
            gc.currentLevel++;
            float rand = UnityEngine.Random.Range(0f,0f);
            if (rand == 0) gc.levelDifficulty = GameControllerScript.LevelDifficulties.Easy;
            if (rand == 1) gc.levelDifficulty = GameControllerScript.LevelDifficulties.Medium;
            if (rand == 2) gc.levelDifficulty = GameControllerScript.LevelDifficulties.Hard;
            //print(gc.levelDifficulty);
            SceneManager.LoadScene("LevelScene");
        }
        if (Keyboard.current.hKey.wasPressedThisFrame) ApplyBuff(new Buff(GameControllerScript.BuffTypes.MaxHealth, 2, 0, 5, true));
        if (Keyboard.current.jKey.wasPressedThisFrame) ApplyBuff(new Buff(GameControllerScript.BuffTypes.InstantHealth, 2, 0, 0, false));
    }
}