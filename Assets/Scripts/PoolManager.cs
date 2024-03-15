using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance {  get; private set; }

    [SerializeField] private EnemyController[] enemyPrefabs;
    [SerializeField] private TankController[] tankPrefabs;
    [SerializeField] private BulletController[] bulletPrefabs;
    [SerializeField] private EnemyDieFx enemyDieFxPrefabs;


    private void Awake()
    {
        if (Instance != null)
        {
            Instance = null;
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public EnemyController GetEnemyController(EnemyType enemyType)
    {
        EnemyController resultEnemy = null;

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            if (enemyPrefabs[i].EnemyType == enemyType)
            {
                resultEnemy = Instantiate(enemyPrefabs[i], Vector3.zero, Quaternion.identity);
            }
        }
        return resultEnemy;
    }


    public TankController GetTankController(TankType tankType)
    {
        TankController resultTank = null;

        for (int i = 0; i < tankPrefabs.Length; i++)
        {
            if (tankPrefabs[i].TankType == tankType)
            {
                resultTank = Instantiate(tankPrefabs[i], Vector3.zero, Quaternion.identity);
            }
        }
        return resultTank;
    }

    public BulletController GetBulletController(BulletType bulletType)
    {
        BulletController resultBullet = null;

        for (int i = 0; i < bulletPrefabs.Length; i++)
        {
            if (bulletPrefabs[i].BulletType == bulletType)
            {
                resultBullet = Instantiate(bulletPrefabs[i], Vector3.zero, Quaternion.identity);
            }
        }
        return resultBullet;
    }


    public EnemyDieFx GetEnemyDieFx()
    {
        return Instantiate(enemyDieFxPrefabs, Vector3.zero, Quaternion.identity);
    }

}
