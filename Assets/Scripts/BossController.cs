using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [SerializeField] private BossType bossType = BossType.Boss01;
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Image healthBar = null;
    [SerializeField] private Sprite[] sprites = null;
    [SerializeField] private int coinBonus;
    [SerializeField] private float speedAttack;
    [SerializeField] private float m_speedMove;
    private float m_damage;
    private float m_health;
    private float m_FisrtHealth;

    public BossType BossType { get => bossType; }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        healthBar.fillAmount = 1f;
        StartCoroutine(PlayAnimation());
        StartCoroutine(Moving());
        StartCoroutine(OnAttack());
    }


    private IEnumerator PlayAnimation()
    {
        while (ViewManager.Instance.GameView.m_GameOver == false)
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
        while (spriteRenderer.transform.position.y >= -4f && ViewManager.Instance.GameView.m_GameOver == false)
        {
            transform.position += Vector3.down * m_speedMove * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator OnAttack()
    {
        while (ViewManager.Instance.GameView.m_GameOver == false)
        {
            if (spriteRenderer.transform.position.y <= -3.9f)
            {
                ViewManager.Instance.GameView.OnEnemyAttack(m_damage);
                DamageEffectController damageEffect = PoolManager.Instance.GetDamageEffectController();
                damageEffect.transform.position = transform.position + Vector3.up;
                damageEffect.SetDamageEffcet(m_damage);

            }
            yield return new WaitForSeconds(speedAttack);
        }
    }


    public void OnBossInit(int oder, float damage, float health)
    {
        spriteRenderer.sortingOrder = oder;
        m_damage = damage;
        m_health = health;
        m_FisrtHealth = health;
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
    }
}
