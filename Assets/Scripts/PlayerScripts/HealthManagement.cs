using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthManagement : MonoBehaviour, IDamageable
{
    [Tooltip("Current health of the player")]
    public float currentHealth;
    [Tooltip("How much max Health the player has")]
    public float maxHealth;
    [Tooltip("The number of hearts the player has displayed (Only if automaticHearts is false")]
    [SerializeField] private int numOfHearts;
    [Tooltip("Determins if the player can take damage. If false, then no damage is applied to player. (Used for I-Frames)")]
    public bool canDamage;
    [Tooltip("Determins if the player is alive. If false, player attacking animations are disabled.")]
    public bool playerLive;
    [Tooltip("The duration of invulnerability the player has after being damaged")]
    public float iFrameDuration;

    private PlayerControllerScript playerController;
    private GameObject uiCanvas;
    public Image heartPrefab;
    private Image[] hearts;
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;
    [Tooltip("The distance betwen each row of hearts")]
    public float heartRowsDistance;
    [Tooltip("The maximum number of hearts that can be displayed before they start a new row")]
    public int maxHeartsOnARow;

    [Tooltip("If True will base the amount of hearts based on Max Health and Health Per Heart")]
    public bool automaticHearts;
    [Tooltip("The amount of health should be per heart (Only used for automaticHearts, and is best being a factor of maxHealth for best accuracy)")]
    public float healthPerHeart;

    [SerializeField]
    private AnimationController animationController;

    private List<float> IFrameRequests;

    void Awake()
    {
        canDamage = true;    
        playerLive = true;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        InitialiseVariables();
        SetUpCanvas();
    }

    /// <summary>
    /// Updates Hearts to show the current and up to date representation of player's health
    /// (Usually good to call after making updates to variables)
    /// </summary>
    private void UpdateHearts()
    {
        for(int i = 0; i < numOfHearts; i++)
        {
            int x = i + 1; // x is the Human read-able representation of where we are in the list (starts at 1)
            float perHeart = maxHealth / numOfHearts;
            // If currentHealth is more than the amount of health that heart's max represents - half a heart's health then display as full
            if (currentHealth > (perHeart * x) - perHeart / 2)
            {
                hearts[i].sprite = fullHeart;
            }
            // If currentHealth is less than or equal to half that heart's represented health but above the heart below it's maximum health, display as half
            else if (currentHealth <= (perHeart * x) - perHeart / 2 && currentHealth > (perHeart * i))
            {
                hearts[i].sprite = halfHeart;
            }
            else
            // If all else fails, the heart is empty
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }
    void Update()
    {
        if(IFrameRequests.Count > 0)
        {
            canDamage = false;
        }
        else
        {
            canDamage = true;
        }
    }

    [ContextMenu("Refresh UI")]
    private void RefreshUI()
    {
        DestroyAllHearts();
        SetUpCanvas();
        UpdateHearts();
    }


    private void InitialiseVariables()
    {
        uiCanvas = GameObject.FindGameObjectWithTag("UICanvas");
        hearts = new Image[numOfHearts];
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerScript>();
        currentHealth = maxHealth;
        IFrameRequests = new List<float>();

        ThrowExceptions();
    }
    /// <summary>
    /// Sets up the Hearts on the Canvas based on parameters in the class
    /// (Should always be called after adjusting the number of hearts a player has, or if automaticHearts is enabled, then after making adjustments to maxHealth)
    /// </summary>
    private void SetUpCanvas()
    {
        if (automaticHearts)
        {
            numOfHearts = Mathf.RoundToInt(maxHealth / healthPerHeart);
        }
        // Creates a new array everytime to accommodate heart amount changes
        hearts = new Image[numOfHearts];
        float xOffset = -28;
        float yOffset = -28;
        // For every heart desired, will create one, parent it to canvas, and move it to the appropriate position and scale
        for (int i = 0; i < numOfHearts; i++)
        {
            Image heart = Instantiate(heartPrefab);
            heart.transform.SetParent(uiCanvas.transform);
            heart.sprite = fullHeart;
            heart.rectTransform.anchoredPosition = new Vector3(xOffset, yOffset, 0.7f);
            // scale on x is -1 because the hearts would become half on the wrong side so it would break up the appearance of the heart bar
            heart.rectTransform.localScale = new Vector3(-1, 1 ,1);
            xOffset -= 32;
            hearts[i] = heart;
            // If the number of hearts is now a multiple of maxHeartsOnARow then reset and adjust values to create a new row
            if ((i+1) % maxHeartsOnARow == 0)
            {
                yOffset -= heartRowsDistance;
                xOffset = -28;
            }
        }
        Canvas.ForceUpdateCanvases();
    }

    /// <summary>
    /// Destroys all Hearts on screen. Should only be used before re-building the hearts on screen.
    /// </summary>
    private void DestroyAllHearts()
    {
        foreach (Image heart in hearts)
        {
            Destroy(heart.gameObject);
        }
    }
    /// <summary>
    /// A premade function to destroy and re-make canvas to accommodate heart amount change.
    /// (Will do nothing if automaticHearts is true)
    /// </summary>
    /// <param name="newNumberOfHearts"></param>
    public void SetNumOfHearts(int newNumberOfHearts)
    {
        numOfHearts = newNumberOfHearts;
        RefreshUI();
    }
    /// <summary>
    /// Sets max Health and updates hearts to show the change
    /// (If automaticHearts is false then it may visually look as if the player loses health if you do not also increase Heart count)
    /// </summary>
    /// <param name="newMaxHealth"></param>
    public void setMaxHealth(float newMaxHealth)
    {
        if (newMaxHealth <= 0) throw new System.Exception("Cannot try to set newMaxHealth to a value of 0 or less.");
        maxHealth = newMaxHealth;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        RefreshUI();
    }
    /// <summary>
    /// Deals Damage to Player based on a damage value if canDamage is true.
    /// </summary>
    /// <param name="dam"></param>
    public void Damage(float dam)
    {
        if (dam < 0) throw new System.Exception("Cannot deal negative damage.");
        if (canDamage)
        {
            animationController.HurtAnimation();
            currentHealth -= dam;
            UpdateHearts();
            StartCoroutine(IFrames(iFrameDuration));


            if (currentHealth <= 0) //Load Death Scene
            { 
                playerLive = false;
                StopAllCoroutines();
                playerController.movementSpeed = 0;
                canDamage = false;
                animationController.DeathAnimation();
                StartCoroutine(LoadDeathScene());
            }
        }
        
    }
    public void GiveHealth(float health)
    {
        currentHealth += health;
        if(currentHealth > maxHealth) currentHealth = maxHealth;
        RefreshUI();
    }

    /// <summary>
    /// Gives the player Invincibility Frames based on a variable duration
    /// </summary>
    /// <returns></returns>
    public IEnumerator IFrames(float duration)
    {
        IFrameRequests.Add(duration);
        yield return new WaitForSeconds(duration);
        IFrameRequests.Remove(duration);
    }

    public void IFramesMethod(float duration)
    {
        StartCoroutine(IFrames(duration));
    }

    /// <summary>
    /// Loads the "Death Scene" which can either be a physical scene, or be replaced with UI To show the player they have lost.
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadDeathScene()
    {
        yield return new WaitForSecondsRealtime(2);
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        Destroy(GameObject.FindGameObjectWithTag("UICanvas"));
        GameObject.Find("GameController").GetComponent<GameControllerScript>().RestartController();
        SceneManager.LoadScene("MainMenu");
    }
    private void ThrowExceptions()
    {
        if (maxHealth <= 0) throw new System.Exception("Cannot have a Max Health of 0 or less");
        if (numOfHearts < 1 && automaticHearts == false) throw new System.Exception("Cannot have less than 1 number of hearts when automaticHearts is false.");
        if (healthPerHeart <= 0 && automaticHearts == true) throw new System.Exception("Cannot have a value of 0 Health Per Heart or less when automaticHearts is true.");
    }
}
