using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float checkRadius = 0.25f;
    [SerializeField] public int damage = 5;

    private float _timeActive;
    
    private void OnEnable()
    {
        _timeActive = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        _timeActive += Time.deltaTime;
        if (_timeActive >= lifetime)
        {
            gameObject.SetActive(false);
            return;
        }

        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.SphereCast(ray, checkRadius, out RaycastHit hitInfo, speed * Time.deltaTime))
        {
            if (hitInfo.collider.CompareTag("Enemy"))
            {
                Enemy enemy = hitInfo.collider.GetComponent<Enemy>();
                if (enemy)
                {
                    enemy.TakeDamage(damage);
                }
            }
            gameObject.SetActive(false);
        }
        
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));
    }
}
