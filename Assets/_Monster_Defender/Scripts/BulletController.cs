using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private TankType tankType = TankType.Tank01;
    public TankType TankType { get => tankType; }

    private float bulletDamage = 0;



    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = col.gameObject.GetComponent<EnemyController>();
            enemy.OnTakeDamage(bulletDamage);
            gameObject.SetActive(false);
        }
        if (col.gameObject.CompareTag("Boss"))
        {
            BossController boss = col.gameObject.GetComponent<BossController>();
            boss.OneHitBullet(bulletDamage);
            gameObject.SetActive(false);
        }
            
    }




    /// <summary>
    /// Init this bullet with parameters.
    /// </summary>
    /// <param name="damage"></param>
    public void OnInitBullet(float damage)
    {
        bulletDamage = damage;
        StartCoroutine(CRMoveBullet());
    }


    /// <summary>
    /// Coroutine move this bullet by it up vector.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CRMoveBullet()
    {
        float speed = IngameManager.Instance.BulletMovementSpeed;
        while (gameObject.activeSelf)
        {
            //Stop moving on Pause game state 
            while (IngameManager.Instance.GameState == GameState.GamePause)
            {
                yield return null;
            }

            //Move using transform.up
            transform.position += transform.up * speed * Time.deltaTime;


            //Check and disable this bullet
            Vector2 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
            if (viewportPos.x >= 1.2f || viewportPos.x <= -0.2f || viewportPos.y >= 1.2f)
            {
                gameObject.SetActive(false);
            }
            yield return null;
        }
    }
    
}
