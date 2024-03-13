using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TankType
{
    Tank01 = 0,
    Tank02,
    Tank03,
    Tank04,
    Tank05,
    Tank06,
    Tank07,
    Tank08,
    Tank09,
    Tank10,
}

public enum EnemyType
{
    Monster01 = 0,
    Monster02,
    Monster03,
    Monster04,
    Monster05,
    Monster06,
    Monster07,
    Monster08,
    Monster09,
    Monster10,
}

public enum BulletType
{
    Bullet01 = 0,
    Bullet02,
    Bullet03,
    Bullet04,
    Bullet05,
    Bullet06,
    Bullet07,
    Bullet08,
    Bullet09,
    Bullet10,
}
public class GameManager : MonoBehaviour
{
    [SerializeField] private EnemyController[] enemyPrefabs;
    [SerializeField] private GridsController[] tankPrefabs;
    [SerializeField] private Transform tankSpawn01;
    [SerializeField] private Transform tankSpawn02;
    [SerializeField] private Transform tankSpawn03;
    [SerializeField] private TankGridController TankGridController = null;

    private Transform selectedTank = null;
    private int m_countEnemy = 0;
    private int m_countTank = 0;

    private void Start()
    {
        SpawnTank(tankSpawn01);
        SpawnTank(tankSpawn02);
        SpawnTank(tankSpawn03);
        StartCoroutine(SpawnEnemies());
    }

    private void Update()
    {
        Vector3 mousePos = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D collider2d = Physics2D.OverlapCircle(mousePos, 0.5f);
            if (collider2d != null)
            {
                selectedTank = collider2d.transform;
            }
        }
        if(Input.GetMouseButton(0) && selectedTank != null)
        {
            selectedTank.position = new Vector3(mousePos.x, mousePos.y, 0f);
        }
        if(Input.GetMouseButtonUp(0) && selectedTank != null)
        {
            Transform closeTankSpawn = TankGridController.GetCloseTankSpawn(selectedTank);
            selectedTank.position = closeTankSpawn.position;
        }
    }

    private IEnumerator SpawnEnemies()
    {
        int enemyOrder = 50;
        while (m_countEnemy <= 3)
        {
            EnemyController enemyController = SpawnOneEnemy(EnemyType.Monster01);
            Vector3 spawnPos = Vector3.zero;
            spawnPos.y = 9;
            spawnPos.x = Random.Range(-4.7f, 4.7f);
            enemyController.transform.position = spawnPos;
            enemyController.SetOderInLayer(enemyOrder);
            enemyOrder--;
            yield return new WaitForSeconds(1);
        }
    }




    private EnemyController SpawnOneEnemy(EnemyType enemyType)
    {
        EnemyController resultEnemy = null;

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            if (enemyPrefabs[i].EnemyType == EnemyType.Monster01)
            {
                resultEnemy = Instantiate(enemyPrefabs[i], Vector3.zero, Quaternion.identity);
                m_countEnemy++;
            }
        }

        return resultEnemy;
    }    




    private void SpawnTank(Transform tankSpawn)
    {
        Vector3 spawnPos = Vector3.zero;

        for (int i = 0; i < tankPrefabs.Length; i++)
        {
            if (tankPrefabs[i].TankType == TankType.Tank01)
            {
                GridsController tankClone = Instantiate(tankPrefabs[i], tankSpawn.position, Quaternion.identity);

                m_countTank++;
            }
        }
    }
}
