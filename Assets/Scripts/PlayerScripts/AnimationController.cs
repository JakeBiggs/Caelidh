using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private SpriteRenderer sprite;
    [SerializeField]
    private HealthManagement healthManagement;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    PlayerControllerScript playerControllerScript;

    void FixedUpdate()
    {
        if (playerControllerScript.canMove)
        {

            //The player is in movement
            if (rb.velocity.magnitude > 1)
            {
                animator.SetFloat("MoveX", rb.velocity.x);
                animator.SetFloat("MoveY", rb.velocity.z);
                animator.SetInteger("AnimState", 1); //set the player to the move animation

                //if the player if moving to the right, flip their x localscale

                    if (playerControllerScript.movementGamepadInput.x > 0 || playerControllerScript.movementKBAMInput.x > 0)
                    {

                        player.transform.localScale = new Vector3(1, 1, 1);

                    }

 


                else if (playerControllerScript.movementGamepadInput.x < 0 || playerControllerScript.movementKBAMInput.x < 0)
                {

                    player.transform.localScale = new Vector3(-1, 1, 1);
                }
            }

            else { 
            animator.SetInteger("AnimState", 2);//set the player to idle animation
            }   
            //Sets player to idle animation if there is no other animation played
         }
     }

    public void HurtAnimation()
    {
        animator.SetTrigger("Hurt");
    }

    public void DeathAnimation()
    {
        animator.SetTrigger("Death");
    }


}
