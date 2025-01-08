using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private BulletPool sphereBulletPool;
    [SerializeField] private BulletPool cubeBulletPool;
    [SerializeField] private BulletPool cylinderBulletPool;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private int health = 20;

    private InputAction _moveAction;
    private InputAction _attackAction;
    private InputAction _switchToSphereAction;
    private InputAction _switchToCubeAction;
    private InputAction _switchToCylinderAction;

    private Vector3 _cameraForward;
    private Vector3 _cameraRight;

    private CharacterController _controller;
    private Camera _camera;

    private Transform _playerModel;
    private Transform _firePoint;

    private float _lastShotTime;
    private BulletPool _currentBulletPool;

    private void Awake()
    {
        // Get the player camera
        _camera = transform.Find("CameraHolder/PlayerCamera").GetComponent<Camera>();
        if (_camera == null)
        {
            Debug.LogError("Camera not found");
        }
    }

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        // Get the transform of the player model
        _playerModel = transform.Find("Body");
        // Get the point where the bullets will spawn
        _firePoint = transform.Find("Body/FirePoint");

        // Set initial bullet pool to sphere
        _currentBulletPool = sphereBulletPool;

        // Get the standard actions
        _moveAction = InputSystem.actions.FindAction("Move");
        _attackAction = InputSystem.actions.FindAction("Attack");

        // Get the actions for weapon switching
        _switchToSphereAction = InputSystem.actions.FindAction("Ammo1");  // Key 1
        _switchToCubeAction = InputSystem.actions.FindAction("Ammo2");     // Key 2
        _switchToCylinderAction = InputSystem.actions.FindAction("Ammo3"); // Key 3

        // Subscribe to weapon switch events
        _switchToSphereAction.performed += _ => SwitchWeapon(sphereBulletPool, "Sphere");
        _switchToCubeAction.performed += _ => SwitchWeapon(cubeBulletPool, "Cube");
        _switchToCylinderAction.performed += _ => SwitchWeapon(cylinderBulletPool, "Cylinder");

        // Update the direction the player will move relative to
        UpdateMovementDirection();

        // Confine the cursor to the screen
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnEnable()
    {
        _switchToSphereAction?.Enable();
        _switchToCubeAction?.Enable();
        _switchToCylinderAction?.Enable();
    }

    private void OnDisable()
    {
        _switchToSphereAction?.Disable();
        _switchToCubeAction?.Disable();
        _switchToCylinderAction?.Disable();
    }

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
        if (!_attackAction.IsPressed() || Time.time < _lastShotTime + fireRate || _currentBulletPool == null)
        {
            return;
        }

        _lastShotTime = Time.time;

        GameObject bullet = _currentBulletPool.GetBullet();
        if (!bullet) return;
        bullet.transform.position = _firePoint.position;
        bullet.transform.rotation = _firePoint.rotation;
    }

    private void SwitchWeapon(BulletPool newPool, string weaponName)
    {
        _currentBulletPool = newPool;
        Debug.Log($"Switched to {weaponName} ammo");
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

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Debug.Log("You died");
        }
        else
        {
            Debug.Log("You took damage, your current health: " + health);
        }
    }
}