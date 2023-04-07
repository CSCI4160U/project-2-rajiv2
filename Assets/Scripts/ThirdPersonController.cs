using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Transform cam;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [Header("Falling")]
    [SerializeField] private float gravityFactor = 1f;
    [SerializeField] private Transform groundPosition;
    [SerializeField] private LayerMask groundLayers;

    [Header("Jumping")]
    [SerializeField] private bool canAirControl = true;
    [SerializeField] private float jumpSpeed = 7f;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    private CharacterController controller;
    //private float verticalRotation = 0f;
    private float verticalSpeed = 0f;
    private bool isGrounded = false;

    public void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        //float horizontal = Input.GetAxisRaw("Horizontal");
        //float vertical = Input.GetAxisRaw("Vertical");
        //Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //// are we on the ground?
        //RaycastHit collision;
        //if (Physics.Raycast(groundPosition.position, Vector3.down, out collision, 0.2f, groundLayers))
        //{
        //    isGrounded = true;
        //}
        //else
        //{
        //    isGrounded = false;
        //}
        //// update vertical speed
        //if (!isGrounded)
        //{
        //    verticalSpeed += gravityFactor * -9.81f * Time.deltaTime;
        //}
        //else
        //{
        //    verticalSpeed = 0f;
        //}


        //Vector3 y = Vector3.zero;

        //// handle jumping
        //if (isGrounded && Input.GetButtonDown("Jump"))
        //{
        //    verticalSpeed = jumpSpeed;
        //    isGrounded = false;
        //    y = transform.up * verticalSpeed;
        //}
        //else if (!isGrounded)
        //{
        //    y = transform.up * verticalSpeed;
        //}

        //if (direction.magnitude >= 0.1f)
        //{
        //    float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + cam.eulerAngles.y;
        //    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

        //    transform.rotation = Quaternion.Euler(0f, angle, 0f);

        //    Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        //    controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        //}

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
 
        float targetAngle;
        float angle;
        Vector3 xAndZMovement = new Vector3(0,0,0);

        // are we on the ground?
        RaycastHit collision;
        if (Physics.Raycast(groundPosition.position, Vector3.down, out collision, 0.15f, groundLayers))
        {
            animator.SetTrigger("isGrounded");
            isGrounded = true;
            animator.ResetTrigger("Falling");
            animator.ResetTrigger("JumpFall");
            verticalSpeed = 0f;
        }
        else
        {
            animator.ResetTrigger("isGrounded");
            isGrounded = false;
            verticalSpeed += gravityFactor * -9.81f * Time.deltaTime;
        }

        // y movement
        Vector3 y = Vector3.zero;

        // handle jumping
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            animator.SetTrigger("Jump");
            animator.ResetTrigger("isGrounded");
            
            verticalSpeed = jumpSpeed;
            isGrounded = false;
            y = transform.up * verticalSpeed;
        }
        else if (!isGrounded)
        {
            
            y = transform.up * verticalSpeed;

            if (verticalSpeed < -2)
            {
                animator.SetTrigger("JumpFall");
            }
            
            animator.SetFloat("Speed", 0);
        }

        // handle movement
        Vector3 movement = y;
        if (direction.magnitude >= 0.1f && (isGrounded || canAirControl))
        {

            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            xAndZMovement = moveDirection.normalized * speed;
            movement += xAndZMovement;
        }

        if (isGrounded)
        {
            animator.SetFloat("Speed", xAndZMovement.magnitude);
        }

        movement *= Time.deltaTime;
        controller.Move(movement);
    }
}
