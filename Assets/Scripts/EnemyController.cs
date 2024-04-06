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
    [SerializeField] private int coinBonus;
    [SerializeField] private float speedAttack;
    [SerializeField] private float m_speedMove;
    private float m_damage;
    private float m_health;
    private float m_FisrtHealth;

    public EnemyType EnemyType { get => enemyType; }


    public void OnEnemyInit(int oder, float damage, float health)
    {
        spriteRenderer.sortingOrder = oder;
        m_damage = damage;
        m_health = health;
        m_FisrtHealth = health;

        healthBar.fillAmount = 1f;
        StartCoroutine(PlayAnimation());
        StartCoroutine(Moving());
    }


    private IEnumerator PlayAnimation()
    {
        while (gameObject.activeSelf)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                spriteRenderer.sprite = sprites[i];
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    private IEnumerator Moving()
    {
        while(gameObject.activeSelf) 
        {
            while (GameManager.Instance.GameState == GameState.GamePause)
            {
                yield return null;
            }
            transform.position += Vector3.down * m_speedMove * Time.deltaTime;
            yield return null;              

            if(transform.position.y <= -4f)
            {
                StartCoroutine(OnAttack());
                yield break;
            }
        }            
    }

    private IEnumerator OnAttack()
    {
        while (gameObject.activeSelf)
        {
            while (GameManager.Instance.GameState == GameState.GamePause)
            {
                yield return null;
            }

            GameManager.Instance.OnEnemyAttack(m_damage);
            DamageEffectController damageEffect = PoolManager.Instance.GetDamageEffectController();
            damageEffect.transform.position = transform.position + Vector3.up;
            damageEffect.SetDamageEffcet(m_damage);
            yield return new WaitForSeconds(speedAttack);
        }
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



            CoinEffectController coinEffect = PoolManager.Instance.GetCoinEffectController();
            coinEffect.transform.position = transform.position + Vector3.up;
            coinEffect.SetCoinBonus(coinBonus);
            GameManager.Instance.CurrentCoin += coinBonus;

            gameObject.SetActive(false);
        }
    }     
}
