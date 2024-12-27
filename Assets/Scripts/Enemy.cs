using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int health = 20;
    [SerializeField] private int attackDistance = 2;
    
    private Transform _playerTransform;
    private NavMeshAgent _agent;

    private float _defaultSpeed;
    
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _defaultSpeed = _agent.speed;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            _playerTransform = player.transform;
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
        }
        else
        {
            _agent.enabled = true;
            _agent.SetDestination(_playerTransform.position);
        }
        

    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
