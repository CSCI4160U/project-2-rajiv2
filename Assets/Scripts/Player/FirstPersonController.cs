using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Player))]

public class FirstPersonController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private float movementSpeed = 15f;

    [Header("Falling")]
    [SerializeField] private float gravityFactor = 1f;
    [SerializeField] private Transform groundPosition;
    [SerializeField] private LayerMask groundLayers;
    [Header("Jumping")]
    [SerializeField] private bool canAirControl = true;
    [SerializeField] private float jumpSpeed = 7f;
    [Header("Looking")]
    [SerializeField] private float mouseSensitivity = 1000f;
    [SerializeField] private Transform firstPersonCamera;
    
    [Header("Crouching")]
    [SerializeField] private float cameraCrouchY;
    [SerializeField] private float crouchControllerHeight;
    [SerializeField] private float crouchControllerCenterY;

    public static FirstPersonController _instance;

    private Player player;

    private CharacterController controller;
    
    private float verticalRotation = 0f;
    private float verticalSpeed = 0f;
    private bool isGrounded = false;

    private void Awake()
    {
        _instance = this;
        controller = GetComponent<CharacterController>();
    }
    private void Start()
    {
        player = GetComponent<Player>();
        Cursor.lockState = CursorLockMode.Locked;
        verticalRotation = 0f;
    }

    private void LookAround()
    {
        // adjust rotations based on mouse position
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity *
        Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity *
        Time.deltaTime;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        firstPersonCamera.localEulerAngles = new Vector3(verticalRotation, 0f, 0f);
    }

    private void Move()
    {
        Vector3 x = Vector3.zero;
        Vector3 y = Vector3.zero;
        Vector3 z = Vector3.zero;

        // handle jumping
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            verticalSpeed = jumpSpeed;
            isGrounded = false;
            y = transform.up * verticalSpeed;
        }
        else if (!isGrounded)
        {
            y = transform.up * verticalSpeed;
        }

        // handle movement
        if (isGrounded || canAirControl)
        {
            x = transform.right * Input.GetAxis("Horizontal") *
            movementSpeed;

            z = transform.forward * Input.GetAxis("Vertical") *
            movementSpeed;
        }
        Vector3 movement = x + y + z;
        movement *= Time.deltaTime;
        controller.Move(movement);
    }

    private void CheckIfGrounded()
    {
        // are we on the ground?
        RaycastHit collision;
        if (Physics.Raycast(groundPosition.position, Vector3.down, out
        collision, 0.2f, groundLayers))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        // update vertical speed
        if (!isGrounded)
        {
            verticalSpeed += gravityFactor * -9.81f * Time.deltaTime;
        }
        else
        {
            verticalSpeed = 0f;
        }
    }

    void Update()
    {

        CheckIfGrounded();

        if (!player.isDead)
        {
            LookAround();
            Move();
        }
    }
}
