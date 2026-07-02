using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

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
        if (target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + targetOffset, movementSpeed * Time.deltaTime);
        }
    }
}