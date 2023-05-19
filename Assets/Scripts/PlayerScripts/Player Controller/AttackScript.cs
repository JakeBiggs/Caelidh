using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class AttackScript : MonoBehaviour
{
    public PlayerController controller;
    public PlayerControllerScript playerScript;
    public Animator playerAnimation;
    public HealthManagement healthManagement;

    private bool isAttacking;
    private bool isEvading;
    public float attackDuration = 1f;
    public float damage = 1.0f;
    public float knockback = 15f;
    private int attackCount = 1;


    private void Start()
    {
        playerAnimation = transform.GetChild(0).GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        /*
         * This code was for if we wanted to control attack states via animation
         * I decided on controlling the state using flags (isAttacking)
        AnimatorStateInfo stateInfo = playerAnimation.GetCurrentAnimatorStateInfo(default);
        if (stateInfo.normalizedTime >= 1.0f)
        {
            //Animation has concluded
            isAttacking = false;
        }
        else
        {
            isAttacking = true;
        }
        */
    }

    public void OnSwingSword()
    {
        if (isAttacking) return;
        
        if (!healthManagement.playerLive) return;


        //playerScript.canMove = false; //Prevents player from moving and rolling if theyre attacking
        playerScript.canRoll = false;

        if (attackCount == 3) attackCount = 1; //resets attack counter

        playerAnimation.SetTrigger("Attack" + attackCount) ; //starts the attack animation

        isAttacking = true;

        attackCount++;
        StartCoroutine(StopAttack(attackDuration));

    }

    IEnumerator StopAttack(float attackLength)
    {
        yield return new WaitForSeconds(attackLength);

        playerScript.canMove = true ;
        playerScript.canRoll = true; //allows the player to roll and attack again

        isAttacking = false; //allows the player to attack again
     // TODO : refactor this system so that a delay to attack is applied, not an increase in attack duration (sword hitbox currently lingers while you are not meant to attack)
    }

    public void OnTriggerEnter(Collider collision)
    {
        //detect any collisions between the player sword and enemy gameobject here
        //Physics.CheckBox(playerSword.center, playerSword.size/2);
        ProcessCollision(collision.gameObject);
    }


    public void ProcessCollision(GameObject collider)
    {
        if (isAttacking)
        {
            AttemptAttack(collider);
        }
    }

    public void AttemptAttack(GameObject other)
    {
        
        if (other.GetComponent<IDamageable>() != null)
        {
            other.GetComponent<IDamageable>().Damage(CalculateDamage());
            if(playerScript.hitsTowardsLS >= playerScript.hitsForLS() && playerScript.hitsForLS() != 0)
            {
                healthManagement.GiveHealth(0.5f);
                playerScript.hitsTowardsLS = 0;
            }
            else if (playerScript.receivedBuffStacks[GameControllerScript.BuffTypes.LifeSteal] > 0)
            {
                playerScript.hitsTowardsLS++;
            }
            Rigidbody rb = other.GetComponent<Rigidbody>();
            rb.AddForce(Vector3.Normalize(other.transform.position - transform.position) * knockback, ForceMode.Impulse);
            print("LS Hits " + playerScript.hitsTowardsLS + "/" + playerScript.hitsForLS());
        }
    }
    public float CalculateDamage()
    {
        float result = 1;
        foreach (float modifier in playerScript.buffLists[GameControllerScript.BuffTypes.Strength].Item1)
        {
            result += modifier;
        }
        foreach (float modifier in playerScript.buffLists[GameControllerScript.BuffTypes.Strength].Item2)
        {
            result *= modifier;
        }
        //print("You Damaged for: " + result);
        return result;
    }

}
