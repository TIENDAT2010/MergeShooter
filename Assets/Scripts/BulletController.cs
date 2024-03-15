using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private BulletType bulletType = BulletType.Bullet01;
    [SerializeField] private EnemyController enemy;
    public BulletType BulletType { get => bulletType; }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = col.gameObject.GetComponent<EnemyController>();
            enemy.OneHitBullet(1);
            Destroy(gameObject);
        }
    }


    public void Move(float speed)
    {
        StartCoroutine(MoveBullet(speed));
    }


    private IEnumerator MoveBullet(float speed)
    {
        while (gameObject.activeSelf)
        {

            //Move bullet
            transform.position += transform.up * speed * Time.deltaTime;

            //Check and destroy bullet
            if ((transform.position.y > 11f) || (transform.position.x > 6f) || (transform.position.x < -6f))
            {
                Destroy(gameObject);
            }
            yield return null;
        }
    }
    
}
