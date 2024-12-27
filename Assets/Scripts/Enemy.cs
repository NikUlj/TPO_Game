using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int health = 20;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
