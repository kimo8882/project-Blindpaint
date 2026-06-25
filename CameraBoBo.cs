// TopDownCameraFollow.cs
using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0f, 20f, 0f);
    [SerializeField] private float followSpeed = 10f;

    private Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        SnapToTarget();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );

        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private void SnapToTarget()
    {
        if (target == null) return;

        transform.position = target.position + offset;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}