using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Transform[] tankSpawns = null;
    private Dictionary<Transform, GameObject> dicTankSpawn = new Dictionary<Transform, GameObject>();

    private Transform selectedTank = null;
    private int m_countTank = 0;



    private void Start()
    {

        for (int i = 0; i < tankSpawns.Length; i++)
        {
            dicTankSpawn.Add(tankSpawns[i], null);
        }


        while (m_countTank < 3)
        {
            TankController tank = PoolManager.Instance.GetTankController(TankType.Tank01);
            for (int i = 0; i < dicTankSpawn.Count; i++)
            {
                Transform gridTranform = tankSpawns[i];
                GameObject valueTank = null;
                dicTankSpawn.TryGetValue(gridTranform, out valueTank);
                if (valueTank == null)
                {
                    dicTankSpawn[gridTranform] = tank.gameObject;
                    tank.gameObject.transform.position = gridTranform.position;
                    tank.gameObject.SetActive(true);
                    tank.CurrentTankSpawn = gridTranform;
                    break;
                }
            }
            m_countTank++;
        }
        StartCoroutine(SpawnEnemies(EnemyType.Monster01));
    }

    private void Update()
    {
        Vector3 mousePos = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D collider2d = Physics2D.OverlapCircle(mousePos, 0.5f);
            if ((collider2d != null) && collider2d.gameObject.CompareTag("Tank"))
            {
                selectedTank = collider2d.transform;
                selectedTank.gameObject.GetComponent<TankController>().IsMoving = true;
            }
        }
        if(Input.GetMouseButton(0) && selectedTank != null)
        {
            selectedTank.gameObject.GetComponent<TankController>().IsMoving = true;
            selectedTank.position = new Vector3(mousePos.x, mousePos.y, 0f);
        }
        if(Input.GetMouseButtonUp(0) && selectedTank != null)
        {
            Transform gridTranformNearest = GetNearestTankSpawn(selectedTank);

            //if(gridTranformNearest == null) 
            //{
            //    selectedTank.gameObject.GetComponent<TankController>().MoveTankBackToGrid();
            //}
            //else
            //{
            //    if (CheckTankInDic(gridTranformNearest))
            //    {
            //        TankController nearTank = GetTankController(gridTranformNearest);
            //        TankController selectTank = selectedTank.GetComponent<TankController>();
            //        if(nearTank.TankType == selectTank.TankType)
            //        {

            //        }
            //        else
            //        {
            //            selectedTank.gameObject.GetComponent<TankController>().MoveTankBackToGrid();
            //        }
            //    }
            //}


            if((CheckTankInDic(gridTranformNearest) == false) && gridTranformNearest != null)
            {
                selectedTank.transform.position = gridTranformNearest.position;
                dicTankSpawn[gridTranformNearest] = selectedTank.gameObject;
                selectedTank.gameObject.GetComponent<TankController>().CurrentTankSpawn = gridTranformNearest;
                selectedTank.GetComponent<TankController>().IsMoving = false;
            }
            else if ( gridTranformNearest == null)
            {
                selectedTank.gameObject.GetComponent<TankController>().MoveTankBackToGrid();
            }           
            else
            {
                TankController tankInGrid = GetTankController(gridTranformNearest);

                if(tankInGrid.GetComponent<TankController>().TankType == selectedTank.GetComponent<TankController>().TankType )
                {
                    TankType tankType = tankInGrid.GetComponent<TankController>().TankType;
                    dicTankSpawn[gridTranformNearest] = null;
                    dicTankSpawn[selectedTank.GetComponent<TankController>().CurrentTankSpawn] = null;
                    tankInGrid.GetComponent<TankController>().DestroyTank();
                    selectedTank.GetComponent<TankController>().DestroyTank();

                    TankType nextTankType = GetNextTankType(tankType);

                    TankController newTank = PoolManager.Instance.GetTankController(nextTankType);
                    newTank.transform.position = gridTranformNearest.transform.position;
                    newTank.CurrentTankSpawn = gridTranformNearest;
                    dicTankSpawn[gridTranformNearest] = newTank.gameObject;
                }
                else
                {
                    selectedTank.gameObject.GetComponent<TankController>().MoveTankBackToGrid();
                }
            }

            selectedTank = null;
        }
    }


    private IEnumerator SpawnEnemies(EnemyType enemyType)
    {
        int enemyOrder = 9999;
        while (true)
        {
            EnemyController enemyController = PoolManager.Instance.GetEnemyController(enemyType);
            Vector3 spawnPos = Vector3.zero;
            spawnPos.y = 9;
            spawnPos.x = Random.Range(-4.7f, 4.7f);
            enemyController.transform.position = spawnPos;
            enemyController.SetOderInLayer(enemyOrder);
            enemyOrder--;
            yield return new WaitForSeconds(1);
        }
    }



    private Transform GetNearestTankSpawn(Transform tankPos)
    {
        float minDistance = 0.5f;
        Transform cur = null;
        for (int i = 0; i < tankSpawns.Length; i++)
        {
            float distance = Vector3.Distance(tankPos.position, tankSpawns[i].position);
            if ((distance < minDistance))
            {
                minDistance = distance;
                cur = tankSpawns[i];
            }
        }
        return cur;
    }


    private bool CheckTankInDic(Transform transform)
    {
        bool check = true;
        for (int i = 0;i < dicTankSpawn.Count; i++)
        {
            GameObject obj = null ;
            dicTankSpawn.TryGetValue(tankSpawns[i], out obj);
            if((tankSpawns[i] == transform) && (obj == null))
            {
                check = false;
            }    
        }
        return check;
    }    

    private TankController GetTankController(Transform transform)
    {
        TankController tank = null;
        for (int i = 0; i < dicTankSpawn.Count; i++)
        {
            GameObject obj = null;
            dicTankSpawn.TryGetValue(tankSpawns[i], out obj);
            if ((tankSpawns[i] == transform))
            {
                tank = obj.gameObject.GetComponent<TankController>();
            }
        }
        return tank;
    }

    private TankType GetNextTankType(TankType oldType)
    {
        TankType tankType = oldType ;
        switch(oldType)
        {
            case TankType.Tank01 : tankType = TankType.Tank02;
                break;
            case TankType.Tank02:
                tankType = TankType.Tank03;
                break;
            case TankType.Tank03:
                tankType = TankType.Tank04;
                break;
            case TankType.Tank04:
                tankType = TankType.Tank05;
                break;
            case TankType.Tank05:
                tankType = TankType.Tank06;
                break;
            case TankType.Tank06:
                tankType = TankType.Tank07;
                break;
            case TankType.Tank07:
                tankType = TankType.Tank08;
                break;
            case TankType.Tank08:
                tankType = TankType.Tank09;
                break;
        }    
        return tankType;
    }
}
