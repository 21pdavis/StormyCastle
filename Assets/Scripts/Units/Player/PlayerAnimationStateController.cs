using UnityEngine;

public class PlayerAnimationStateController : MonoBehaviour
{
    Animator animator;
    PlayerMovement playerMovement;
    PlayerCombat playerCombat;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        playerMovement.canMove = !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
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

    public void Attack()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            animator.SetTrigger("attackTrigger");
        }
    }
}
