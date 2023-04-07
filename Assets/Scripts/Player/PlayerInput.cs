using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float runSpeed = 10f;

    [SerializeField] private Animator animator = null;

    public Player player;

    private float horizontalMove = 0f;
    private float verticalMove = 0f;
    private bool isMovingHorizontal = false;
    private bool isMovingVertical = false;
    private bool isMovingInPositiveDirection = false;

    private void Update()
    {
        // if player is alive
        if (!player.isDead)
        {
            Move();
            DoWalkingAnimations();
            DoAttackingAnimations();
        }
        else
        {
            DoDeathAnimations();
        }
        
    }

    private void Start()
    {
        StartCoroutine(AttackAnimationCoolDown());
    }

    IEnumerator AttackAnimationCoolDown()
    {

        // cool down for 1 second
        yield return new WaitForSeconds(0.6f);

        // other animations can play now
        animator.SetTrigger("isNotAttacking");

    }

    private void FixedUpdate()
    {
        isMovingVertical = false;
        isMovingHorizontal = false;
        isMovingInPositiveDirection = false;
        animator.SetFloat("Speed", 0f);
    }

    private void Move()
    {
        horizontalMove = Input.GetAxis("Horizontal") * runSpeed;
        verticalMove = Input.GetAxis("Vertical") * runSpeed;

        if (horizontalMove != 0)
        {
            resetVerticalFacing();
            isMovingHorizontal = true;
        }
        else if (verticalMove != 0)
        {
            resetHorizontalFacing();
            isMovingVertical = true;
        }

        // establish whether or not the Player is moving in positive direction (right or up)
        if (verticalMove > 0 || horizontalMove > 0)
        {
            isMovingInPositiveDirection = true;
        }

        // move Player in appropriate horizontal direction
        transform.Translate(Vector3.right * (horizontalMove * Time.deltaTime));

        // move Player in appropriate vertical direction
        transform.Translate(Vector3.up * (verticalMove * Time.deltaTime));
    }

    private void DoDeathAnimations()
    {
        animator.SetTrigger("isDead");
    }

    private void DoAttackingAnimations()
    {
        // if fire button pressed
        if (Input.GetButtonDown("Fire1"))
        {
            // make isNotAttacking false
            animator.ResetTrigger("isNotAttacking");

            // trigger attack animation
            animator.SetTrigger("isAttacking");

            StartCoroutine(AttackAnimationCoolDown());
        }
    }

    private void DoWalkingAnimations()
    {

        // move left or right depending on if moving in positive direction (right)
        if (isMovingHorizontal)
        {
            // reset other direction
            animator.SetBool("isFacingUp", false);
            player.isFacingUp = false;

            animator.SetBool("isFacingDown", false);
            player.isFacingDown = false;

            // set current direction
            animator.SetBool("isFacingRight", isMovingInPositiveDirection);
            player.isFacingRight = isMovingInPositiveDirection;

            animator.SetBool("isFacingLeft", !isMovingInPositiveDirection);
            player.isFacingLeft = !isMovingInPositiveDirection;

            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        }
        // move up or down depending on if moving in positive direction (up)
        else if (isMovingVertical)
        {
            // reset other direction
            animator.SetBool("isFacingRight", false);
            player.isFacingRight = false;

            animator.SetBool("isFacingLeft", false);
            player.isFacingLeft = false;

            // set current direction
            animator.SetBool("isFacingUp", isMovingInPositiveDirection);
            player.isFacingUp = isMovingInPositiveDirection;

            animator.SetBool("isFacingDown", !isMovingInPositiveDirection);
            player.isFacingDown = !isMovingInPositiveDirection;

            animator.SetFloat("Speed", Mathf.Abs(verticalMove));
        }

    }

    // Reset Animation booleans for horizontal movement
    private void resetHorizontalFacing()
    {
        animator.SetBool("isFacingRight", false);
        animator.SetBool("isFacingLeft", false);
    }

    // Reset Animation booleans for vertical movement
    private void resetVerticalFacing()
    {
        animator.SetBool("isFacingUp", false);
        animator.SetBool("isFacingDown", false);
    }

   
}
