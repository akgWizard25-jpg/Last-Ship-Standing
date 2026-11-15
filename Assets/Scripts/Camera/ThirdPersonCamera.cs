
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow Position")]
    public float followHeight = 3f;
    public float followDistance = 10f;
    public float followPositionSmoothness = 3f;

    [Header("Camera Look")]
    public float lookAheadDistance = 15f;
    public float lookHeight = 1f;
    public float rotationSmoothness = 4f;

    [Header("Independent Camera Roll")]
    public bool followShipPitch = true;
    public bool followShipYaw = true;
    public bool followShipRoll = false;

    [Header("Camera Distance")]
    public float minDistance = 5f;
    public float maxDistance = 20f;
    public float zoomSpeed = 3f;
    public float zoomSmoothness = 5f;

    private float currentDistance;
    private float targetDistance;

    private float cameraYaw;
    private float cameraPitch;

    void Start()
    {
        currentDistance = followDistance;
        targetDistance = followDistance;

        if (target != null)
        {
            Vector3 angles = target.eulerAngles;

            cameraYaw = angles.y;
            cameraPitch = angles.x;
        }
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        HandleZoom();
        HandleCameraRotation();
        HandleCameraPosition();
    }

    void HandleZoom()
    {
        float scroll = Mouse.current != null
            ? Mouse.current.scroll.ReadValue().y / 120f
            : 0f;

        targetDistance -= scroll * zoomSpeed * 10f;

        targetDistance = Mathf.Clamp(
            targetDistance,
            minDistance,
            maxDistance
        );

        currentDistance = Mathf.Lerp(
            currentDistance,
            targetDistance,
            zoomSmoothness * Time.deltaTime
        );
    }

    void HandleCameraRotation()
    {
        Vector3 targetAngles = target.eulerAngles;

        // Follow selected ship rotation axes
        if (followShipYaw)
            cameraYaw = targetAngles.y;

        if (followShipPitch)
            cameraPitch = targetAngles.x;

        // Calculate desired camera rotation
        Quaternion desiredRotation = Quaternion.Euler(
            cameraPitch,
            cameraYaw,
            followShipRoll ? targetAngles.z : 0f
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            rotationSmoothness * Time.deltaTime
        );
    }

    void HandleCameraPosition()
    {
        // Camera offset relative to desired camera orientation
        Vector3 offset = new Vector3(
            0f,
            followHeight,
            -currentDistance
        );

        Vector3 desiredPosition =
            target.position +
            transform.rotation * offset;

        // Smooth camera following
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followPositionSmoothness * Time.deltaTime
        );

        // Look ahead of the ship
        Vector3 lookTarget =
            target.position +
            target.forward * lookAheadDistance +
            Vector3.up * lookHeight;

        Quaternion lookRotation = Quaternion.LookRotation(
            lookTarget - transform.position,
            Vector3.up
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            rotationSmoothness * Time.deltaTime
        );
    }
}



