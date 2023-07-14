using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationStateController : MonoBehaviour
{
    Animator animator;
    PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void Move()
    {
        bool isRunning = animator.GetBool("isRunning");

        if (playerMovement.moving && !isRunning)
        {
            animator.SetBool("isRunning", true);
        }
        else if (!playerMovement.moving && isRunning)
        {
            animator.SetBool("isRunning", false);
        }
    }

    public void Roll()
    {

    }
}
