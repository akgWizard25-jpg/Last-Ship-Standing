
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;

    [Header("Camera Position")]
    public Vector3 offset = new Vector3(0f, 4f, -10f);
    public float followSpeed = 5f;
    public float lookHeight = 1.5f;

    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition =
            target.TransformPoint(offset);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        Vector3 lookTarget =
            target.position + Vector3.up * lookHeight;

        Vector3 direction =
            lookTarget - transform.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion desiredRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                desiredRotation,
                followSpeed * Time.deltaTime
            );
        }
    }
}


