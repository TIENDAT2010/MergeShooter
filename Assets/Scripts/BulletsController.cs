using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletsController : MonoBehaviour
{
    [SerializeField] private BulletType bulletType = BulletType.Bullet01;
    [SerializeField] private EnemyController enemy;
    public BulletType BulletType { get => bulletType; }

    //private void Awake()
    //{
    //    enemy = GetComponent<EnemyController>();
    //}

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = col.gameObject.GetComponent<EnemyController>();
            enemy.OneHitBullet(1);
            Destroy(gameObject);
        }
    }


}
