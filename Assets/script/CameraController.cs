using UnityEngine;


public class CameraController : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject.
    private Vector3 offset; // distant between camera and player

    void Start()
    {
// Calculate the initial offset between the camera's position and the player's position.

        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Maintain the same offset
        transform.position = player.transform.position + offset;
    }
}
