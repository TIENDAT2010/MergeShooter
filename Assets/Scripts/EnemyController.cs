using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyType enemyType = EnemyType.Monster01;
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Sprite[] sprites = null;

    private float m_speed;
    private float m_damage;
    private float m_health;

    public EnemyType EnemyType { get => enemyType; }

    private void Start()
    {
        StartCoroutine(PlayAnimation());

        StartCoroutine(Moving());
    }


    private IEnumerator PlayAnimation()
    {
        while (true)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                spriteRenderer.sprite = sprites[i];
                yield return new WaitForSeconds(0.02f);
            }
        }
    }

    private IEnumerator Moving()
    {
        while(spriteRenderer.transform.position.y >= -4f)
        {
            transform.position += Vector3.down * m_speed * Time.deltaTime;
            yield return null;
        }
    }
    
    public void OnEnemyInit(int oder, float speed, float damage, float health)
    {
        spriteRenderer.sortingOrder = oder;
        m_speed = speed;
        m_damage = damage;
        m_health = health;
    }    

    public void OneHitBullet(int damage)
    {
        m_health  = m_health - damage;
        if(m_health <= 0)
        {
            GameManager.Instance.UpdateDeadEnemy();
            EnemyDieFx enemyDieFx = PoolManager.Instance.GetEnemyDieFx();
            enemyDieFx.transform.position = transform.position;
            Destroy(gameObject);
        }
    }   
}
