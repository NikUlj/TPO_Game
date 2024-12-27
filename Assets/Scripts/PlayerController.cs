using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private BulletPool bulletPool;

    private InputAction _moveAction;
    private InputAction _attackAction;

    private Vector3 _cameraForward;
    private Vector3 _cameraRight;
    
    private CharacterController _controller;
    private InputSystem_Actions _inputSystemActions;
    private Camera _camera;
    
    private Transform _playerModel;
    private Transform _firePoint;
    
    // Placeholder fire rate
    [SerializeField] private float fireRate = 0.5f;
    private float _lastShotTime = 0;

    private void Awake()
    {
        // Get the player camera
        _camera = transform.Find("CameraHolder/PlayerCamera").GetComponent<Camera>();
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
        // Get the point where the bullets will spawn
        _firePoint = transform.Find("Body/FirePoint");

        // Get the move and attack actions from the Input system so that the input can be read later
        _moveAction = InputSystem.actions.FindAction("Move");
        _attackAction = InputSystem.actions.FindAction("Attack");
        
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
        Shoot();
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
        var ray = _camera.ScreenPointToRay(Input.mousePosition);

        Vector3 position = GetRayPlaneIntersection(ray, _firePoint.position.y);
        
        var direction = position - transform.position;

        direction.y = 0;

        _playerModel.forward = direction;
    }

    private void Shoot()
    {
        if (!_attackAction.IsPressed() || Time.time < _lastShotTime + fireRate)
        {
            return;
        }

        _lastShotTime = Time.time;
        
        GameObject bullet = bulletPool.GetBullet();
        if (!bullet) return;
        bullet.transform.position = _firePoint.position;
        bullet.transform.rotation = _firePoint.rotation;
    }
    
    private Vector3 GetRayPlaneIntersection(Ray ray, float targetY)
    {
        // Calculate the t value for the intersection
        float t = (targetY - ray.origin.y) / ray.direction.y;

        // If t is negative, the intersection point is behind the ray's origin
        if (t < 0)
        {
            return Vector3.zero; // No intersection in front of the ray
        }

        // Calculate the intersection point
        Vector3 intersection = ray.origin + t * ray.direction;

        return intersection;
    }
}
