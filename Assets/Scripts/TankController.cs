using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TankController : MonoBehaviour
{
    [SerializeField] private TankType tankType = TankType.Tank01;
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Sprite[] sprites = null;
    [SerializeField] private GameObject bulletSpawnPos;
    [SerializeField] private float speedBullet;
    [SerializeField] private float speedFire;
    [SerializeField] private float rangeFire;
    [SerializeField] private int damageTank = 0;
    private GameObject targetEnemy = null;

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


    private Transform currentTankSpawn = null;
    public Transform CurrentTankSpawn
    {
        set 
        {
            currentTankSpawn = value; 
        }
        get
        {
            return currentTankSpawn;
        }
    }


    public TankType TankType { get => tankType; }




    private void Start()
    {
        StartCoroutine(FindEnemy());        
    }




    private IEnumerator FindEnemy()
    {
        while (targetEnemy == null && ViewManager.Instance.GameView.m_GameOver == false)
        {
            if (m_isMoving == false)
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemy in enemies)
                {
                    float distanceToPlayer = Vector3.Distance(this.transform.position, enemy.transform.position);
                    if ((distanceToPlayer <= rangeFire) && (Mathf.Abs(enemy.transform.position.x - transform.position.x) <= 4))
                    {
                        targetEnemy = enemy;
                        StartCoroutine(RotateToEnemy());
                        StartCoroutine(ShootEnemy());
                        yield break;
                    }
                }
            }
            yield return null;
        }
    }  


    private IEnumerator ShootEnemy()
    {
        while(targetEnemy != null && ViewManager.Instance.GameView.m_GameOver == false) 
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                spriteRenderer.sprite = sprites[i];
                yield return new WaitForSeconds(0.03f);
            }

            BulletController bullet = PoolManager.Instance.GetBulletController(TankType);
            bullet.transform.position = bulletSpawnPos.transform.position;
            bullet.transform.up = transform.up;
            bullet.Move(speedBullet);
            bullet.SetDamage(damageTank);

            while (true)
            {
                yield return new WaitForSeconds(speedFire); break;
            }
                

            if (targetEnemy == null)
            {
                StartCoroutine(FindEnemy());
                yield break;
            }
        }
    }

    private IEnumerator RotateToEnemy()
    {
        while (targetEnemy != null && ViewManager.Instance.GameView.m_GameOver == false)
        {
            transform.up = (targetEnemy.transform.position - transform.position).normalized;
            yield return null;
        }
    }

    public void DestroyTank()
    {
        Destroy(gameObject);
    }


    public void MoveTankBackToGrid()
    {
        IsMoving = true;
        gameObject.GetComponent<Collider2D>().enabled = false;
        StartCoroutine(MoveTank());
        gameObject.GetComponent<Collider2D>().enabled = true;

    }

    private IEnumerator MoveTank()
    {
        float t = 0;
        float moveTime = (Vector3.Distance(transform.position, currentTankSpawn.position) / 20f);
        Vector3 startVector3 = transform.position;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            Vector3 newPos = Vector3.Lerp(startVector3,currentTankSpawn.position, factor);
            transform.position = newPos;
            yield return null;
        }
        m_isMoving = false;
    }
}
