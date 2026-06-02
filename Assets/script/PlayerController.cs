using UnityEngine;
using UnityEngine.InputSystem; //notify input
using TMPro;//use for TextMeshProUGUI


public class PlayerController : MonoBehaviour
{
    public GameObject winTextObject;
    public TextMeshProUGUI countText; 
    private Rigidbody rb;
    private int count;
    private float movementX;
    private float movementY;

   public float speed = 0; //speed are changed in setting

    // Start is called before the first frame update.
   void Start()
    {
        rb = GetComponent<Rigidbody>();// Get and store the Rigidbody component attached to the player.
        count = 0;
        SetCountText();//print first
        winTextObject.SetActive(false);
   }

    // This function is called when a move input is detected.
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();// Convert the input value into a Vector2 for movement.

        // Store the X and Y components of the movement.
        movementX = movementVector.x;
        movementY = movementVector.y;
    }
    
    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 12)
        {
            winTextObject.SetActive(true);
        }
    }

    // FixedUpdate is called once per fixed frame-rate frame.
    void FixedUpdate() 
    {

        Vector3 movement = new Vector3 (movementX, 0.0f, movementY);// Create a 3D movement vector using the X and Y inputs.
        rb.AddForce(movement * speed); // Apply force to the Rigidbody to move the player.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp")) //only object with "PickUp" tag can be deactivate
        {
            other.gameObject.SetActive(false); //deactivate object instead of destroy it for reuse
            count++;

            SetCountText();
        }
    }
    
}
