using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour,IDamageable
{
    public NavMeshAgent agent;
    public HealthManagement healthManager;
    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public GameObject healthBarUI;
    public Slider slider;

    [Tooltip("Current health of the enemy")]
    private float health;
    [Tooltip("Maximum health of the enemy")]
    public float maxHealth;
    [Tooltip("Enemies base damage")]
    public float enemyDamage;
    private float healthPercentage;
    public float knockback;

    //Patrolling variables
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;


    //Attacking
    public float timeBetweenAttacks;
    bool hasAttacked;
  

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public bool DebugMode;
    public bool isAwake;

    public LevelController lvlctrl;

    private void Start()
    {
        InitialiseVariables();
        health = maxHealth;
        Invoke(nameof(WakeUpAI),0.25f);
    }
    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer); //Third parameter is layermask
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (isAwake)
        {
            CheckState();
        }

        //Health bar scripts
        if (this.gameObject != null)
        {
            healthPercentage = health / maxHealth;
            slider.value = healthPercentage;
        }
    }

    private void CheckState()
    {
        if (!playerInSightRange && !playerInAttackRange) IdleState();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();

        //Debug
        if (playerInSightRange && DebugMode) Debug.Log("playerInSightRange");
    }
    private void IdleState()
    {
        //Debug.Log("IdleState()");
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        //Walkpoint Reached
        if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        if (DebugMode) Debug.Log("Searching for WalkPoint...");
        //Calculate random point within range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        //Checks if walkpoint is valid ground
        if (Physics.Raycast(walkPoint, -transform.up, whatIsGround)) walkPointSet = true;
    }
    private void ChasePlayer()
    {
        if (DebugMode) Debug.Log("ChasePlayer()");
        agent.SetDestination(player.position);
    }
    
    private void AttackPlayer()
    {
        //Make sure enemy doesnt move
        agent.SetDestination(transform.position);

        if (!hasAttacked)
        {
            Rigidbody rb = player.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * knockback, ForceMode.Impulse);
            healthManager.Damage(enemyDamage);
            hasAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        hasAttacked = false;
    }
    private void DestroyEnemy()
    {
        lvlctrl.enemies.Remove(this.gameObject);
        lvlctrl.CheckUnlockRequirements();
        Destroy(gameObject);
    }

    public void Damage(float damage)
    {
        health -= damage;
        if (health<= 0)
        {
            agent.speed = 0;
            hasAttacked=true;
            Invoke(nameof(DestroyEnemy), 0.3f);
        }
    }
    private void WakeUpAI()
    {
        isAwake = true;
    }
    private void InitialiseVariables()
    {
        healthManager = player.gameObject.GetComponent<HealthManagement>();
        lvlctrl = GameObject.Find("SceneController").GetComponent<SceneController>().currentLevel.GetComponent<LevelController>();
        healthBarUI = transform.GetChild(0).gameObject;
        slider = healthBarUI.transform.GetChild(0).GetComponent<Slider>();
        whatIsGround = LayerMask.GetMask("Default", "Aimable");
        whatIsPlayer = LayerMask.GetMask("Player");
    }
}
