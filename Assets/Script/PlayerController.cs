using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour {
    
    [Header("References")]
    public float movementSpeed = 10f;
    public Rigidbody rb;
    public PlayerGun myGun;
    
    [Header("Animation")]
    [SerializeField] private Animator _myAnimator; 
    private bool _hasDied = false; 

    void Update () {
        if (!IsOwner) return; 

        // --- 1. DEATH CHECK ---
        if (GetComponent<Player>().isDead.Value) {
            // Slam on the brakes so we don't slide
            rb.linearVelocity = Vector3.zero; 
            
            if (!_hasDied) {
                _hasDied = true; // Lock the door so we only trigger this once
                
                if (_myAnimator != null) {
                    _myAnimator.SetFloat("VelocityX", 0);
                    _myAnimator.SetFloat("VelocityZ", 0);
                    _myAnimator.SetTrigger("Die"); 
                }
            }
            return; // Stop the rest of the code from running!
        } else {
            // Unlock the door when we respawn
            _hasDied = false; 
        }

        // --- 2. INPUTS ---
        HandleMovementInput();
        HandleRotationInput();
        HandleShootInput();
        HandleReloadInput();
        
        if (Input.GetKeyDown(KeyCode.H)) {
            GetComponent<Player>().UseInjectorServerRpc();
        }

        // --- 3. ANIMATION MATH ---
        if (_myAnimator != null) {
            // Get our flat world velocity
            Vector3 worldVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            
            // Convert it to LOCAL velocity (relative to where the gun is pointing)
            Vector3 localVelocity = transform.InverseTransformDirection(worldVelocity);

            // Feed the X (strafe) and Z (forward/back) directly into the Blend Tree
            _myAnimator.SetFloat("VelocityZ", localVelocity.z);
            _myAnimator.SetFloat("VelocityX", localVelocity.x);
        }
    }

    // -----------------------------------------------------------
    // THE MISSING MOVEMENT & AIMING LOGIC!
    // -----------------------------------------------------------
    
    private void HandleMovementInput() {
        // Get WASD or Arrow Key input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        
        // Calculate the direction and apply the speed directly to the Rigidbody
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        rb.linearVelocity = moveDirection * movementSpeed;
    }

    private void HandleRotationInput() {
        // Shoot an invisible laser from the camera to where your mouse is pointing on the screen
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        // Create an invisible flat floor at the player's feet
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0)); 
        float rayDistance;

        // If the laser hits the floor, make the player look directly at that spot
        if (groundPlane.Raycast(ray, out rayDistance)) {
            Vector3 point = ray.GetPoint(rayDistance);
            Vector3 lookAtPoint = new Vector3(point.x, transform.position.y, point.z);
            transform.LookAt(lookAtPoint);
        }
    }

    private void HandleShootInput() {
        // TRIGGER 1: Left Mouse Button sends a "0"
        if (Input.GetMouseButtonDown(0)) {
            if (myGun != null) {
                myGun.Shoot(0);
            }
        }

        // TRIGGER 2: Right Mouse Button sends a "1"
        if (Input.GetMouseButtonDown(1)) {
            if (myGun != null) {
                myGun.Shoot(1);
            }
        }
    }

    private void HandleReloadInput() {
        // Press R to reload
        if (Input.GetKeyDown(KeyCode.R)) {
            if (myGun != null) {
                myGun.Reload();
            }
        }
    }
}