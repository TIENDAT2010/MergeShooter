using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyType enemyType = EnemyType.Monster01;
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Sprite[] sprites = null;

    public float speed;
    private float damage;
    private float health = 5;

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
            transform.position += Vector3.down * speed * Time.deltaTime;
            yield return null;
        }
    }
    
    public void SetOderInLayer(int oder)
    {
        spriteRenderer.sortingOrder = oder;
    }    

    public void OneHitBullet(int damage)
    {
        health  = health - damage;
        if(health <= 0)
        {
            Die();
        }
    }   

    public void Die()
    {
        EnemyDieFx enemyDieFx  = PoolManager.Instance.GetEnemyDieFx();
        enemyDieFx.transform.position = transform.position;
        Destroy(gameObject);
    }

}
