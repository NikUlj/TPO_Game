using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int health = 20;
    
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
        if (dist <= 5)
        {
            _agent.speed = 0;
        }
        else
        {
            _agent.speed = _defaultSpeed;
        }
        _agent.SetDestination(_playerTransform.position);

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
