using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private TankType tankType = TankType.Tank01;
    public TankType TankType { get => tankType; }
    private float damage = 0;
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

    public void SetDamage(float dg)
    {
        damage = dg;
    }
        
    public void Move()
    {
        StartCoroutine(MoveBullet());
    }


    private IEnumerator MoveBullet()
    {
        float speed = 30f;
        while (gameObject.activeSelf)
        {
            while (GameManager.Instance.GameState == GameState.GamePause)
            {
                yield return null;
            }

            transform.position += transform.up * speed * Time.deltaTime;

            if ((transform.position.y > 11f) || (transform.position.x > 6f) || (transform.position.x < -6f))
            {
                gameObject.SetActive(false);
            }
            yield return null;
        }
    }
    
}
