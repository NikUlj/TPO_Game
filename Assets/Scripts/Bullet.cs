using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum BulletType
    {
        Sphere,
        Cube,
        Cylinder
    }

    [SerializeField] private BulletType bulletType;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float checkRadius = 0.25f;
    [SerializeField] public int damage = 5;

    private readonly Collider[] _overlaps = new Collider[10];
    private float _timeActive;

    private void OnEnable()
    {
        _timeActive = 0f;
    }

    public void CheckSpawn()
    {
        Array.Clear(_overlaps, 0, _overlaps.Length);
        int length = Physics.OverlapSphereNonAlloc(transform.position, checkRadius, _overlaps);
        Debug.Log(length);
        if (length > 0)
        {
            for (int i = 0; i < length; i++)
            {
                var col = _overlaps[i];
                
                if (col == null) break;
                
                Debug.Log("Collider detected: " + col.gameObject.name + ", Tag: " + col.tag);
                
                if (col.CompareTag("Enemy"))
                {
                    Enemy enemy = col.GetComponent<Enemy>();

                    if (enemy)
                    {
                        if (CanDamageEnemyType(enemy.GetEnemyType()))
                            enemy.TakeDamage(damage);
                    }

                    gameObject.SetActive(false);
                }
            }
            gameObject.SetActive(false);
        }
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
                    if (CanDamageEnemyType(enemy.GetEnemyType()))
                        enemy.TakeDamage(damage);
                }
            }
            gameObject.SetActive(false);
        }
    
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));
    }

    bool CanDamageEnemyType(Enemy.EnemyType enemyType)
    {
        return (int)bulletType == (int)enemyType;

    }
}
