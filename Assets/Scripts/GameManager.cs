using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Transform[] tankSpawns = null;
    private Dictionary<Transform, GameObject> dicTankSpawn = new Dictionary<Transform, GameObject>();

    private Transform selectedTank = null;
    private int m_level;
    private int m_enemyCount = 0;

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


    private void Start()
    {
        for (int i = 0; i < tankSpawns.Length; i++)
        {
            dicTankSpawn.Add(tankSpawns[i], null);
        }


        m_level = PlayerPrefs.GetInt(PlayerPrefsKey.LEVEL_KEY, 1);
        LevelConfigSO levelConfig = Resources.Load("Levels/" + m_level.ToString(), typeof(LevelConfigSO)) as LevelConfigSO;

        Debug.Log(m_level);
        for(int i = 0; i < levelConfig.InitTanks.Count; i++)
        {
            SpawnTank(levelConfig.InitTanks[i]);
        }

        StartCoroutine(SpawnEnemies(levelConfig.waveConfigs));
        
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
            float newY = Mathf.Clamp(mousePos.y, -9.8f, 0f);
            selectedTank.position = new Vector3(mousePos.x, newY, 0f);
        }
        if(Input.GetMouseButtonUp(0) && selectedTank != null)
        {
            Transform gridTranformNearest = GetNearestTankSpawn(selectedTank);

            if(gridTranformNearest == null)
            {
                selectedTank.gameObject.GetComponent<TankController>().MoveTankBackToGrid();
            }
            else
            {
                if((CheckTankInDic(gridTranformNearest, selectedTank.gameObject.GetComponent<TankController>().CurrentTankSpawn) == false))
                {
                    selectedTank.transform.position = gridTranformNearest.position;
                    dicTankSpawn[gridTranformNearest] = selectedTank.gameObject;
                    selectedTank.gameObject.GetComponent<TankController>().CurrentTankSpawn = gridTranformNearest;
                    selectedTank.GetComponent<TankController>().IsMoving = false;
                }
                else
                {
                    TankController tankInGrid = GetTankController(gridTranformNearest);

                    if (tankInGrid.GetComponent<TankController>().TankType == selectedTank.GetComponent<TankController>().TankType)
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
            }
            selectedTank = null;
        }
    }




    private void SpawnTank(TankType tankType)
    {
        TankController tank = PoolManager.Instance.GetTankController(tankType);
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
    }



    private IEnumerator SpawnEnemies(List<WaveConfig> waveConfigs)
    {
        int enemyOrder = 9999;
        int waveIndex = 0;
        while (true)
        {
            WaveConfig waveConfig = waveConfigs[waveIndex];
            for (int i = 0;i < waveConfig.enemyConfigs.Count;i++)
            {
                EnemyConfig enemyConfig = waveConfig.enemyConfigs[i];
                EnemyController enemyController = PoolManager.Instance.GetEnemyController(enemyConfig.enemyType);
                Vector3 spawnPos = Vector3.zero;
                spawnPos.y = 9;
                spawnPos.x = Random.Range(-4.7f, 4.7f);
                enemyController.transform.position = spawnPos;
                enemyController.OnEnemyInit(enemyOrder,waveConfig.enemySpeed,enemyConfig.damage,enemyConfig.health);
                enemyOrder--;
                m_enemyCount++;
                yield return new WaitForSeconds(1);
            }

            while (m_enemyCount > 0)
            {
                yield return null;
            }
            yield return new WaitForSeconds(1);
            waveIndex++;
            if(waveIndex >= waveConfigs.Count)
            {
                break;
            }
        }

        while (m_enemyCount > 0)
        {
            yield return null;
        }
        Debug.Log("Complete Level !!!");
        
    }



    public void UpdateDeadEnemy()
    {
        m_enemyCount--;
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


    private bool CheckTankInDic(Transform transform, Transform currentTranform)
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
            else if ((obj != null) && (currentTranform != tankSpawns[i]))
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
