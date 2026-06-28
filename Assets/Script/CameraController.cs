using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    // 1. THIS IS THE FIX: We changed 'private' to 'public' so the Player can access it!
    public Transform target;
    
    [SerializeField]
    private Vector3 targetOffset;
    [SerializeField]
    private float movementSpeed;

    void Start () {
        
    }

    void Update () {
        MoveCamera();
    }

    void MoveCamera () 
    {
        // SAFETY CHECK: Only follow the target if it exists
        if (target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + targetOffset, movementSpeed * Time.deltaTime);
        }
    }
}