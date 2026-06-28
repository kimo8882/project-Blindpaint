using UnityEngine;

public class Billboard : MonoBehaviour {

    [Header("Position Settings")]
    [Tooltip("Drag the root Player object here")]
    [SerializeField] private Transform _playerRoot; 
    
    [Tooltip("Set your custom X, Y, and Z offsets here!")]
    [SerializeField] private Vector3 _offset = new Vector3(0, 2.5f, 0); 

    void LateUpdate() {
        // 1. Force the position to stay locked in world space, completely ignoring the player's spinning!
        if (_playerRoot != null) {
            transform.position = _playerRoot.position + _offset;
        }

        // 2. Force the UI to always perfectly match the camera's angle
        if (Camera.main != null) {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}