using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    [Header("Tank Configs")]
    [SerializeField] private float attackRate = 0.5f;
    [SerializeField] private float attackRange = 7f;
    [SerializeField] private float minDamageAmount = 1f;
    [SerializeField] private float maxDamageAmount = 3f;
    [SerializeField] private float bulletMovementSpeed = 35f;

    [Header("Tank References")]
    [SerializeField] private TankType tankType = TankType.Tank01;
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private ShootEffectController shootEffect = null;
    [SerializeField] private GameObject bulletSpawnPos;
    [SerializeField] private Sprite[] sprites = null;

    private GameObject targetEnemy = null;
    private float damageAmount = 0f;

    public int SortingOder
    {
        set { spriteRenderer.sortingOrder = value; }
    }


    private bool m_isMoving = false;
    public bool IsMoving
    {
        set 
        {
            targetEnemy = null;
            m_isMoving = value; 
        }
    }



    public TankType TankType { get => tankType; }



    public void OnTankInit()
    {
        shootEffect.gameObject.SetActive(false);
        damageAmount = Random.Range(minDamageAmount, maxDamageAmount);
        StartCoroutine(FindEnemy());
    }



    public void OnTankMove()
    {
        m_isMoving = true;
        targetEnemy = null;
    }    


    private IEnumerator FindEnemy()
    {
        while (targetEnemy == null)
        {
            while (IngameManager.Instance.GameState == GameState.GamePause || m_isMoving == true)
            {
                yield return null;
            }

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                float distanceToPlayer = Vector3.Distance(this.transform.position, enemy.transform.position);
                if ((distanceToPlayer <= attackRange) && (Mathf.Abs(enemy.transform.position.x - transform.position.x) <= 4) && enemy.activeSelf == true)
                {
                    targetEnemy = enemy;
                    StartCoroutine(RotateToEnemy());
                    StartCoroutine(ShootEnemy());
                    yield break;
                }
            }
            GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");
            foreach (GameObject boss in bosses)
            {
                float distanceToPlayer = Vector3.Distance(this.transform.position, boss.transform.position);
                if ((distanceToPlayer <= attackRange) && (Mathf.Abs(boss.transform.position.x - transform.position.x) <= 4) && boss.activeSelf == true)
                {
                    targetEnemy = boss;
                    StartCoroutine(RotateToEnemy());
                    StartCoroutine(ShootEnemy());
                    yield break;
                }
            }
            yield return null;
        }
    }  


    private IEnumerator ShootEnemy()
    {
        while(targetEnemy != null)
        {
            shootEffect.gameObject.SetActive(true);
            shootEffect.PlayShootEffect();

            for (int i = 0; i < sprites.Length; i++)
            {
                spriteRenderer.sprite = sprites[i];
                yield return new WaitForSeconds(0.01f);
            }

            while (IngameManager.Instance.GameState == GameState.GamePause && m_isMoving)
            {
                yield return null;
            }

            SpawnBullet();
            yield return new WaitForSeconds(attackRate);

            shootEffect.gameObject.SetActive(false);

            if (targetEnemy != null)
            {
                if (targetEnemy.activeSelf == false)
                {
                    targetEnemy = null;
                }
            }
            
            if(targetEnemy == null)
            {
                StartCoroutine(FindEnemy());
                yield break;
            }
        }
    }


    private IEnumerator RotateToEnemy()
    {
        while (targetEnemy != null)
        { 
            transform.up = (targetEnemy.transform.position - transform.position).normalized;
            yield return null;
        }
    }




    public void MoveTankBackToGrid(Vector3 targetPos)
    {
        OnTankMove();
        gameObject.GetComponent<Collider2D>().enabled = false;
        StartCoroutine(MoveTank(targetPos));
        gameObject.GetComponent<Collider2D>().enabled = true;
    }

    private IEnumerator MoveTank(Vector3 targetPos)
    {
        float t = 0;
        float moveTime = (Vector3.Distance(transform.position, targetPos) / 20f);
        Vector3 startVector3 = transform.position;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            Vector3 newPos = Vector3.Lerp(startVector3, targetPos, factor);
            transform.position = newPos;
            yield return null;
        }
        m_isMoving = false;
        OnTankInit();
    }


    private void SpawnBullet()
    {
        BulletController bulletspawn = PoolManager.Instance.GetBulletController(tankType);
        bulletspawn.transform.position = bulletSpawnPos.transform.position;
        bulletspawn.transform.up = transform.up;
        bulletspawn.gameObject.SetActive(true);
        bulletspawn.OnInitBullet(damageAmount, bulletMovementSpeed);
    }
}
