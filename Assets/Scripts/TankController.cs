using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridsController : MonoBehaviour
{
    [SerializeField] private TankType tankType = TankType.Tank01;
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Sprite[] sprites = null;
    [SerializeField] private BulletsController[] bulletPrefabs;
    [SerializeField] private GameObject bulletSpawnPos;
    private GameObject targetEnemy = null;
    [SerializeField] private int speedBullet;
    [SerializeField] private float speedFireBullet;
    [SerializeField] private float rangeFire;
    private float health;

    public TankType TankType { get => tankType; }

    private void Start()
    {
        StartCoroutine(CheckEnemy());        
    }



    private IEnumerator CheckEnemy()
    {
        while (targetEnemy == null)
        {

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                float distanceToPlayer = Vector3.Distance(this.transform.position, enemy.transform.position);
                if((distanceToPlayer <= rangeFire) && (Mathf.Abs(enemy.transform.position.x - transform.position.x) <= 4))
                {
                    targetEnemy = enemy;
                    StartCoroutine(RotateTank());
                    StartCoroutine(TankFire());
                    yield break;
                }
            }



            yield return null;
        }

    }   
    


    private IEnumerator TankFire()
    {
        while(targetEnemy != null) 
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                spriteRenderer.sprite = sprites[i];
                yield return new WaitForSeconds(0.03f);
            }
            SpawnBullet();
            if (targetEnemy == null)
            {
                StartCoroutine(CheckEnemy());
                yield break;
            }
        }
    }

    private IEnumerator RotateTank()
    {
        while (targetEnemy != null)
        {
            transform.up = (targetEnemy.transform.position - transform.position).normalized;
            yield return null;
        }
    }


    private void SpawnBullet()
    {
        for (int i = 0; i < bulletPrefabs.Length; i++)
        {
            if (bulletPrefabs[i].BulletType == BulletType.Bullet01)
            {
                BulletsController bulletClone = Instantiate(bulletPrefabs[i], bulletSpawnPos.transform.position, Quaternion.identity);
                StartCoroutine(MoveBullet(bulletClone));
            }
        }
    }

    private IEnumerator MoveBullet(BulletsController bulletClone)
    { 
        while(bulletClone != null) 
        {
            bulletClone.transform.position += (transform.up * Time.deltaTime * speedBullet);
            yield return null;
        }
    }

}
