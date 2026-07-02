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

        if (GetComponent<Player>().isDead.Value) {
            rb.linearVelocity = Vector3.zero; 
            
            if (!_hasDied) {
                _hasDied = true; 
                
                if (_myAnimator != null) {
                    _myAnimator.SetFloat("VelocityX", 0);
                    _myAnimator.SetFloat("VelocityZ", 0);
                    _myAnimator.SetTrigger("Die"); 
                }
            }
            return; 
        } else {
            _hasDied = false; 
        }

        HandleMovementInput();
        HandleRotationInput();
        HandleShootInput();
        HandleReloadInput();
        
        if (Input.GetKeyDown(KeyCode.H)) {
            GetComponent<Player>().UseInjectorServerRpc();
        }

        if (_myAnimator != null) {
            Vector3 worldVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            
            Vector3 localVelocity = transform.InverseTransformDirection(worldVelocity);

            _myAnimator.SetFloat("VelocityZ", localVelocity.z);
            _myAnimator.SetFloat("VelocityX", localVelocity.x);
        }
    }


    
    private void HandleMovementInput() {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        rb.linearVelocity = moveDirection * movementSpeed;
    }

    private void HandleRotationInput() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0)); 
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance)) {
            Vector3 point = ray.GetPoint(rayDistance);
            Vector3 lookAtPoint = new Vector3(point.x, transform.position.y, point.z);
            transform.LookAt(lookAtPoint);
        }
    }

    private void HandleShootInput() {
        if (Input.GetMouseButtonDown(0)) {
            if (myGun != null) {
                myGun.Shoot(0);
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            if (myGun != null) {
                myGun.Shoot(1);
            }
        }
    }

    private void HandleReloadInput() {
        if (Input.GetKeyDown(KeyCode.R)) {
            if (myGun != null) {
                myGun.Reload();
            }
        }
    }
}