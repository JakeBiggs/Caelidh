using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class TrowController : MonoBehaviour, IDamageable, IEnemyType
{
    private NavMeshAgent agent;
    private HealthManagement healthManager;
    private Transform player;

    private LayerMask whatIsGround, whatIsPlayer;

    private GameObject healthBarUI;
    private Slider slider;

    [Tooltip("Current health of the enemy")]
    private float health;
    [Tooltip("Maximum health of the enemy")]
    public float maxHealth;
    [Tooltip("Enemies base damage")]
    public float enemyDamage;
    private float healthPercentage;
    public float knockback;

    //Patrolling variables
    private Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;
    public float minFiringRange;
    public float retreatRange;


    //Attacking
    public float timeBetweenAttacks;
    private bool hasAttacked;
    public GameObject projectile;
    public float projectileSpeed;


    //States
    public float sightRange, attackRange;
    private bool playerInSightRange, playerInAttackRange,playerInRetreatRange;
    public bool DebugMode;
    public bool isAwake;

    [HideInInspector] public LevelController lvlctrl;

    private void Start()
    {
        InitialiseVariables();
        health = maxHealth;
        Invoke(nameof(WakeUpAI), 0.25f);
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
        playerInRetreatRange = Physics.CheckSphere(transform.position, retreatRange, whatIsPlayer);

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
        Vector3 point = (player.position + (Vector3.Normalize(transform.position - player.position) * minFiringRange));
        point.y = player.position.y;
        agent.SetDestination(point);
        //GameObject test = Instantiate(projectile);
        //test.transform.position = player.position + (Vector3.Normalize(transform.position - player.position) * minFiringRange);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesnt move
        agent.SetDestination(transform.position);

        if (!hasAttacked)
        {
            GameObject proj = Instantiate(projectile,transform.position + new Vector3(0,1,0),new Quaternion());
            proj.GetComponent<Rigidbody>().velocity = Vector3.Normalize(player.position - transform.position) * projectileSpeed;
            projectile.transform.rotation = Quaternion.Euler(proj.GetComponent<Rigidbody>().velocity);
            //Starts reset for attacking

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
        lvlctrl.enemies.Remove(gameObject);
        lvlctrl.CheckUnlockRequirements();
        Destroy(gameObject);
    }

    public void Damage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            agent.speed = 0;
            hasAttacked = true;
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

    public void Initialise(float damage, float health)
    {
        enemyDamage += damage;
        maxHealth += health;
    }
}
