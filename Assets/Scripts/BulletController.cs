using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private TankType tankType = TankType.Tank01;
    public TankType TankType { get => tankType; }
    private int damage = 0;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = col.gameObject.GetComponent<EnemyController>();
            enemy.OneHitBullet(damage);
            gameObject.SetActive(false);
        }
        if (col.gameObject.CompareTag("Boss"))
        {
            BossController boss = col.gameObject.GetComponent<BossController>();
            boss.OneHitBullet(damage);
            gameObject.SetActive(false);
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
                gameObject.SetActive(false);
            }
            yield return null;
        }
    }
    
}
