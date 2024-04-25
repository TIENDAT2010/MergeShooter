using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [Header("Boss Configs")]
    [SerializeField] private int coinBonus;
    [SerializeField] private float speedAttack;
    [SerializeField] private float m_speedMove;

    [Header("Boss References")]
    [SerializeField] private BossType bossType = BossType.Boss01;
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Material normalMaterial = null;
    [SerializeField] private Material whiteMaterial = null;
    [SerializeField] private Image healthBar = null;
    [SerializeField] private Sprite[] sprites = null;

    private float m_damage;
    private float m_health;
    private float m_FisrtHealth;

    public BossType BossType { get => bossType; }



    public void OnBossInit(int oder, float damage, float health)
    {
        spriteRenderer.material = normalMaterial;
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
                yield return new WaitForSeconds(0.02f);
            }
        }
    }

    private IEnumerator Moving()
    {
        while (GameManager.Instance.GameState == GameState.GamePause)
        {
            yield return null;
        }

        while (spriteRenderer.transform.position.y >= -3.5f && gameObject.activeSelf)
        {
            transform.position += Vector3.down * m_speedMove * Time.deltaTime;
            yield return null;
        }
        if(transform.position.y <= -3.5f)
        {
            StartCoroutine(OnAttack());
            yield break;
        }
    }

    private IEnumerator OnAttack()
    {
        while(gameObject.activeSelf)
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
        m_health = m_health - damage;
        healthBar.fillAmount = m_health / m_FisrtHealth;
        if (m_health <= 0)
        {
            GameManager.Instance.UpdateDeadBoss();
            EnemyDieFx enemyDieFx = PoolManager.Instance.GetEnemyDieFx();
            enemyDieFx.transform.position = transform.position;
            gameObject.SetActive(false);
            CoinEffectController coinEffect = PoolManager.Instance.GetCoinEffectController();
            coinEffect.transform.position = transform.position + Vector3.up;
            coinEffect.SetCoinBonus(coinBonus);
            GameManager.Instance.CurrentCoin += coinBonus;            
        }
        else
        {
            StartCoroutine(ChangeMaterial());
        }
    }

    private IEnumerator ChangeMaterial()
    {
        spriteRenderer.material = whiteMaterial;
        yield return new WaitForSeconds(0.02f);
        spriteRenderer.material = normalMaterial;
    }
}
