using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class FirstPersonMovement : MonoBehaviour
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
    [SerializeField] private Transform camera;
    
    [Header("Crouching")]
    [SerializeField] private float cameraCrouchY;
    [SerializeField] private float crouchControllerHeight;
    [SerializeField] private float crouchControllerCenterY;
    private float cameraDefaultY;

    private CharacterController controller;
    
    private float defaultControllerHeight;
    private float defaultControllerCenterY;
    private float verticalRotation = 0f;
    private float verticalSpeed = 0f;
    private bool isGrounded = false;
    private bool isCrouching = false;

    private void Awake()
    {
        cameraDefaultY = camera.position.y;
        controller = GetComponent<CharacterController>();
        defaultControllerHeight = controller.height;
        defaultControllerCenterY = controller.center.y;
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        verticalRotation = 0f;
    }
    void Update()
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

        // handle crouching
        if (Input.GetButtonDown("Crouch") && isGrounded)
        {
            isCrouching = !isCrouching;
        }

        //if (isCrouching)
        //{
        //    // change camera height (Y)
        //    camera.localPosition = new Vector3(camera.localPosition.x, -1.9f, camera.localPosition.z);

        //    // change character controller height and center Y
        //    controller.height = crouchControllerHeight;
        //    controller.center = new Vector3(controller.center.x, crouchControllerCenterY, controller.center.z);
        //}
        //else
        //{
        //    // reset camera height
        //    camera.position = new Vector3(camera.position.x, cameraDefaultY, camera.position.z);

        //    // reset character contrller height and center Y
        //    controller.height = defaultControllerHeight;
        //    controller.center = new Vector3(controller.center.x, defaultControllerCenterY, controller.center.z);
        //}

        // adjust rotations based on mouse position
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity *
        Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity *
        Time.deltaTime;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        camera.localEulerAngles = new Vector3(verticalRotation, 0f,
        0f);
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
}
