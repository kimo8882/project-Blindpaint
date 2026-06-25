// PlayerTopDownMovement.cs
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerTopDownMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Aiming")]
    [SerializeField] private Camera mainCamera;

    private CharacterController controller;
    private Vector3 velocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        Move();
        AimAtMouse();
        ApplyGravity();
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); // A D
        float vertical = Input.GetAxisRaw("Vertical");     // W S

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    private void AimAtMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorldPosition = ray.GetPoint(distance);

            Vector3 lookDirection = mouseWorldPosition - transform.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.01f)
            {
                transform.forward = lookDirection.normalized;
            }
        }
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}   