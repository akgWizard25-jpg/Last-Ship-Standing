
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Forward Movement")]
    public float acceleration = 15f;
    public float deceleration = 20f;
    public float maxSpeed = 40f;
    public float minSpeed = 0f;

    [Header("Mouse Steering")]
    public float mouseSensitivity = 2f;
    public float maxPitch = 60f;
    public float rotationSmoothness = 5f;

    [Header("Banking")]
    public float bankAngle = 35f;
    public float bankSmoothness = 5f;

    private Rigidbody rb;

    private float currentSpeed;

    private float yaw;
    private float pitch;
    private float currentBank;

    private float horizontalInput;
    private float verticalInput;
    private float bankInput;

    private Quaternion initialRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        initialRotation = transform.rotation;

        yaw = 0f;
        pitch = 0f;

        currentSpeed = 0f;

        // Lock cursor to screen centre
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleInput();
        HandleMouseSteering();
    }

    void HandleInput()
    {
        verticalInput = 0f;

        if (Keyboard.current.wKey.isPressed)
            verticalInput = 1f;

        if (Keyboard.current.sKey.isPressed)
            verticalInput = -1f;

        bankInput = 0f;

        if (Keyboard.current.aKey.isPressed)
            bankInput = 1f;

        if (Keyboard.current.dKey.isPressed)
            bankInput = -1f;
    }

    void HandleMouseSteering()
    {
        if (Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        yaw += mouseDelta.x * mouseSensitivity * 0.02f;
        pitch -= mouseDelta.y * mouseSensitivity * 0.02f;

        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);
    }

    void FixedUpdate()
    {
        HandleSpeed();
        HandleRotation();
        HandleMovement();
    }

    void HandleSpeed()
    {
        if (verticalInput > 0f)
        {
            currentSpeed += acceleration * Time.fixedDeltaTime;
        }
        else if (verticalInput < 0f)
        {
            currentSpeed -= deceleration * Time.fixedDeltaTime;
        }

        currentSpeed = Mathf.Clamp(
            currentSpeed,
            minSpeed,
            maxSpeed
        );
    }

    void HandleRotation()
    {
        // Calculate desired banking
        float targetBank = bankInput * bankAngle;

        currentBank = Mathf.Lerp(
            currentBank,
            targetBank,
            bankSmoothness * Time.fixedDeltaTime
        );

        // Mouse controls pitch and yaw
        // A/D controls roll
        Quaternion targetRotation =
            initialRotation *
            Quaternion.Euler(pitch, yaw, currentBank);

        Quaternion smoothedRotation = Quaternion.Slerp(
            rb.rotation,
            targetRotation,
            rotationSmoothness * Time.fixedDeltaTime
        );

        rb.MoveRotation(smoothedRotation);
    }

    void HandleMovement()
    {
        Vector3 movement =
            transform.forward * currentSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + movement);
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}
