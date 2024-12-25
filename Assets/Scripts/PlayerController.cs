using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;

    private InputAction _moveAction;

    private Vector3 _cameraForward;
    private Vector3 _cameraRight;

    [SerializeField] private LayerMask groundMask;
    
    private CharacterController _controller;
    private InputSystem_Actions _inputSystemActions;
    private Camera _camera;
    private Transform _playerModel;

    private void Awake()
    {
        // Get the player camera
        _camera = transform.Find("Camera Holder/Player Camera").GetComponent<Camera>();
        if (_camera == null)
        {
            Debug.LogError("Camera not found");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        // Get the transform of the player model
        _playerModel = transform.Find("Body");

        // Get the move action from the Input system so that the movement vector can be read later
        _moveAction = InputSystem.actions.FindAction("Move");
        
        // Update the direction the player will move relative to
        UpdateMovementDirection();
        
        // Confine the cursor to the screen
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Aim();
    }

    private void Move()
    {
        // Get the movement input
        Vector2 moveValue = _moveAction.ReadValue<Vector2>();
        if (moveValue == Vector2.zero) 
            return;
        
        // Transform the movement direction to world space on a 2D plane
        Vector3 moveDir = (_cameraForward * moveValue.y + _cameraRight * moveValue.x).normalized;

        // Move the character with SimpleMove (ignores the y-axis)
        _controller.SimpleMove(moveDir * moveSpeed);
    }

    private void UpdateMovementDirection()
    {
        _cameraForward = _camera.transform.forward;
        _cameraRight = _camera.transform.right;
        
        _cameraForward.y = 0;
        _cameraRight.y = 0;
        
        _cameraForward = _cameraForward.normalized;
        _cameraRight = _cameraRight.normalized;
    }

    private Vector3 GetMousePosition()
    {
        // Cast ray from camera through the mouse position
        var ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, 1000, groundMask))
        {
            return hitInfo.point;
        }

        return Vector3.zero;
    }

    private void Aim()
    {
        var position = GetMousePosition();
        if (position != Vector3.zero)
        {
            var direction = position - transform.position;

            direction.y = 0;

            _playerModel.forward = direction;
        }
    }
}
