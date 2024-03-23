using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyType enemyType = EnemyType.Monster01;
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Image healthBar = null;
    [SerializeField] private Sprite[] sprites = null;

    private float m_speed;
    private float m_damage;
    private float m_health;
    private float m_FisrtHealth;

    public EnemyType EnemyType { get => enemyType; }

    private void Start()
    {
        healthBar.fillAmount = 1f;
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
        m_FisrtHealth = health;
    }    

    public void OneHitBullet(int damage)
    {
        m_health  = m_health - damage;
        healthBar.fillAmount = m_health / m_FisrtHealth; 
        if(m_health <= 0)
        {
            GameManager.Instance.UpdateDeadEnemy();
            EnemyDieFx enemyDieFx = PoolManager.Instance.GetEnemyDieFx();
            enemyDieFx.transform.position = transform.position;
            Destroy(gameObject);
        }
    }   
}
