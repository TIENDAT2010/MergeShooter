using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance {  get; private set; }

    [SerializeField] private EnemyController[] enemyPrefabs;
    [SerializeField] private TankController[] tankPrefabs;
    [SerializeField] private BulletController[] bulletPrefabs;
    [SerializeField] private BossController[] bossPrefabs;
    [SerializeField] private DeadEffectController enemyDieFxPrefabs;
    [SerializeField] private CoinEffectController coinEffectPrefabs;
    [SerializeField] private DamageEffectController damageEffectPrefabs;


    private List<BulletController> listBullet = new List<BulletController>();
    private List<EnemyController> listEnemy = new List<EnemyController>();
    private List<DamageEffectController> listDamageEffectController = new List<DamageEffectController>();

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


    public BossController GetBossController(BossType bossType)
    {
        BossController resultBoss = null;

        for (int i = 0; i < bossPrefabs.Length; i++)
        {
            if (bossPrefabs[i].BossType == bossType)
            {
                resultBoss = Instantiate(bossPrefabs[i], Vector3.zero, Quaternion.identity);
            }
        }
        return resultBoss;
    }


    public EnemyController GetEnemyController(EnemyType enemyType)
    {
        EnemyController resultEnemy = listEnemy.Where(a => a.EnemyType.Equals(enemyType) && !a.gameObject.activeSelf).FirstOrDefault();

        if (resultEnemy == null)
        {
            EnemyController prefab = enemyPrefabs.Where(a => a.EnemyType.Equals(enemyType)).FirstOrDefault();
            resultEnemy = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            listEnemy.Add(resultEnemy);
        }
        resultEnemy.gameObject.SetActive(true);
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



    public BulletController GetBulletController(TankType tankType)
    {
        BulletController resultBullet = listBullet.Where(a => a.TankType.Equals(tankType) && !a.gameObject.activeSelf).FirstOrDefault();

        if(resultBullet == null)
        {
            BulletController prefab = bulletPrefabs.Where(a => a.TankType.Equals(tankType)).FirstOrDefault();
            resultBullet = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            listBullet.Add(resultBullet);
        }
        resultBullet.gameObject.SetActive(true);
        return resultBullet;
    }


    public DeadEffectController GetEnemyDieFx()
    {
        return Instantiate(enemyDieFxPrefabs, Vector3.zero, Quaternion.identity);
    }

    public CoinEffectController GetCoinEffectController()
    {
        return Instantiate(coinEffectPrefabs, Vector3.zero, Quaternion.identity);
    }




    /// <summary>
    /// Get an inactive DamageEffectController object.
    /// </summary>
    /// <returns></returns>
    public DamageEffectController GetDamageEffectController()
    {
        //Find the object in the list
        DamageEffectController damageEffect = listDamageEffectController.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

        if(damageEffect == null)
        {
            //Instantiate the damage effect
            damageEffect = Instantiate(damageEffectPrefabs, Vector3.zero, Quaternion.identity);
            damageEffect.gameObject.SetActive(false);
            listDamageEffectController.Add(damageEffect);
        }

        return damageEffect;
    }
}
