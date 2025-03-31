using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 20f;
    public float rotationSpeed = 100f;
    
    [Header("Boundaries")]
    public Vector2 horizontalBounds = new Vector2(-50f, 50f);
    public Vector2 verticalBounds = new Vector2(-50f, 50f);
    
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float currentRotationX = 0f;
    private float currentRotationY = 0f;
    
    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        
        // Store initial rotation angles
        Vector3 initialEuler = initialRotation.eulerAngles;
        currentRotationX = initialEuler.y;
        currentRotationY = initialEuler.x;
    }
    
    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }
    
    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();
        
        Vector3 right = transform.right;
        
        Vector3 movement = (forward * vertical + right * horizontal) * moveSpeed * Time.deltaTime;
        
        // Apply movement
        Vector3 newPosition = transform.position + movement;
        
        // Apply boundaries
        newPosition.x = Mathf.Clamp(newPosition.x, horizontalBounds.x, horizontalBounds.y);
        newPosition.z = Mathf.Clamp(newPosition.z, verticalBounds.x, verticalBounds.y);
        
        transform.position = newPosition;
    }
    
    private void HandleRotation()
    {
        // Q and E for horizontal rotation
        if (Input.GetKey(KeyCode.Q))
        {
            currentRotationX -= rotationSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            currentRotationX += rotationSpeed * Time.deltaTime;
        }

        // Z and C for vertical rotation
        if (Input.GetKey(KeyCode.Z))
        {
            currentRotationY = Mathf.Clamp(currentRotationY - rotationSpeed * Time.deltaTime, -80f, 80f);
        }
        if (Input.GetKey(KeyCode.C))
        {
            currentRotationY = Mathf.Clamp(currentRotationY + rotationSpeed * Time.deltaTime, -80f, 80f);
        }

        // Apply rotations
        transform.rotation = Quaternion.Euler(currentRotationY, currentRotationX, 0f);
    }
    
    public void ResetCamera()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        
        // Reset rotation angles
        Vector3 initialEuler = initialRotation.eulerAngles;
        currentRotationX = initialEuler.y;
        currentRotationY = initialEuler.x;
    }
} 