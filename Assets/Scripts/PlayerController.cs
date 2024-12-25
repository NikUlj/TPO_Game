using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;

    private InputAction _moveAction;

    private Vector3 _cameraForward;
    private Vector3 _cameraRight;
    

    private CharacterController _controller;
    private InputSystem_Actions _inputSystemActions;
    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = transform.Find("Camera Holder/Player Camera");
        if (_cameraTransform == null)
        {
            Debug.LogError("Camera not found");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _controller = GetComponent<CharacterController>();

        _moveAction = InputSystem.actions.FindAction("Move");
        
        UpdateMovementDirection();
        
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector2 moveValue = _moveAction.ReadValue<Vector2>();
        if (moveValue == Vector2.zero) 
            return;
        
        Vector3 moveDir = (_cameraForward * moveValue.y + _cameraRight * moveValue.x).normalized;

        _controller.SimpleMove(moveDir * moveSpeed);
    }

    void UpdateMovementDirection()
    {
        _cameraForward = _cameraTransform.forward;
        _cameraRight = _cameraTransform.right;
        
        _cameraForward.y = 0;
        _cameraRight.y = 0;
        
        _cameraForward = _cameraForward.normalized;
        _cameraRight = _cameraRight.normalized;
    }
    
}
