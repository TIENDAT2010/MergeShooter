using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private BulletType bulletType = BulletType.Bullet01;
    public BulletType BulletType { get => bulletType; }
    private int damage = 0;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = col.gameObject.GetComponent<EnemyController>();
            enemy.OneHitBullet(damage);
            Destroy(gameObject);
        }
    }

    public void SetDamage(int dg)
    {
        damage = dg;
    }
        

    public void Move(float speed)
    {
        StartCoroutine(MoveBullet(speed));
    }


    private IEnumerator MoveBullet(float speed)
    {
        while (gameObject.activeSelf)
        {
            transform.position += transform.up * speed * Time.deltaTime;

            if ((transform.position.y > 11f) || (transform.position.x > 6f) || (transform.position.x < -6f))
            {
                Destroy(gameObject);
            }
            yield return null;
        }
    }
    
}
