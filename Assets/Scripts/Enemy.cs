using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        Sphere,
        Cube,
        Cylinder
    }

    [Header("Enemy Type Settings")]
    [SerializeField] private EnemyType enemyType;

    [SerializeField] private int health = 20;
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int attackDamage = 5;

    private float _lastDamageTime;

    private PlayerController _player;
    private Transform _playerTransform;
    private NavMeshAgent _agent;
    
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            _playerTransform = player.transform;
            _player = _playerTransform.transform.GetComponent<PlayerController>();
        }
        else
        {
            Debug.LogWarning("Player transform not found");
        }
    }

    private void Update()
    {
        var dist = Vector3.Distance(_playerTransform.position, transform.position);
        
        // If enemy is close to player, stop moving and look at player
        if (dist <= attackDistance)
        {
            _agent.enabled = false;
            Vector3 directionToPlayer = _playerTransform.position - transform.position;
            
            directionToPlayer.y = 0;

            // If the direction is not zero, rotate towards the player
            if (directionToPlayer.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = targetRotation;
            }

            if (Time.time >= _lastDamageTime + attackCooldown)
            {
                _player.TakeDamage(attackDamage);
                _lastDamageTime = Time.time;
            }
        }
        else
        {
            _agent.enabled = true;
            _agent.SetDestination(_playerTransform.position);
        }
        

    }

    public EnemyType GetEnemyType()
    {
        return enemyType;
    }
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // Add score before destroying the enemy
            ScoreManager.Instance.AddKillScore();
            Destroy(gameObject);
        }
    }
}
