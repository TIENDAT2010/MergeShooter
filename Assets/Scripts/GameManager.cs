using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Transform[] tankSpawns = null;
    private Dictionary<Transform, GameObject> dicTankSpawn = new Dictionary<Transform, GameObject>();
    private LevelConfigSO levelConfig = null;
    private Transform selectedTankPos = null;

    public int CurrentCoin 
    {
        get { return PlayerPrefs.GetInt(PlayerPrefsKey.COIN_KEY, 0); }
        set
        {
            PlayerPrefs.SetInt(PlayerPrefsKey.COIN_KEY, value);
        }
    }
    public float MainHealth
    { 
        get { return m_mainHealth; }

        set { m_mainHealth = value; }
    }
    public int CurrentLevel { private set; get; }
    public float BaseHealth { private set; get; }   
    public TankType TankTypeToRandom { get ; private set ; }

    private int m_enemyCount = 0;
    private int m_bossCount = 0;
    private float m_mainHealth = 0;
    private GameState m_state;

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
        Application.targetFrameRate = 60;
        CurrentLevel = PlayerPrefs.GetInt(PlayerPrefsKey.LEVEL_KEY, 1);
        ViewManager.Instance.SetActiveView(ViewType.GameView);

        levelConfig = Resources.Load("Levels/" + CurrentLevel.ToString(), typeof(LevelConfigSO)) as LevelConfigSO;
        BaseHealth = levelConfig.MainHealth;
        MainHealth = BaseHealth;

        for (int i = 0; i < tankSpawns.Length; i++)
        {
            dicTankSpawn.Add(tankSpawns[i], null);
        }


        for (int i = 0; i < levelConfig.InitTanks.Count; i++)
        {
            SpawnTank(levelConfig.InitTanks[i]);
        }
        TankTypeToRandom = levelConfig.InitTanks[Random.Range(0, levelConfig.InitTanks.Count)];

        m_state = GameState.GameStart;

        StartCoroutine(SpawnEnemies());
    }

    private void Update()
    {
        if(m_state == GameState.GameStart && IsGameOver() == false)
        {
            Vector3 mousePos = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButtonDown(0))
            {
                Collider2D collider2d = Physics2D.OverlapCircle(mousePos, 0.5f);
                if ((collider2d != null) && collider2d.gameObject.CompareTag("Tank"))
                {
                    selectedTankPos = collider2d.transform;
                    selectedTankPos.gameObject.GetComponent<TankController>().SortingOder = 100;
                }
            }
            if (Input.GetMouseButton(0) && selectedTankPos != null)
            {
                selectedTankPos.gameObject.GetComponent<TankController>().IsMoving = true;
                float newY = Mathf.Clamp(mousePos.y, -9.8f, 0f);
                selectedTankPos.position = new Vector3(mousePos.x, newY, 0f);

            }
            if (Input.GetMouseButtonUp(0) && selectedTankPos != null)
            {
                Transform gridTranformNearest = GetNearestTankSpawn(selectedTankPos);
                TankController selectedTank = selectedTankPos.GetComponent<TankController>();

                if (gridTranformNearest == null)
                {
                    selectedTank.MoveTankBackToGrid();
                }
                else
                {
                    if(gridTranformNearest == selectedTank.CurrentTankSpawn)
                    {
                        selectedTank.MoveTankBackToGrid();
                    }
                    else
                    {
                        if (CheckTankInDic(gridTranformNearest) == false)
                        {
                            selectedTankPos.transform.position = gridTranformNearest.position;
                            dicTankSpawn[gridTranformNearest] = selectedTankPos.gameObject;
                            dicTankSpawn[selectedTank.CurrentTankSpawn] = null;
                            selectedTank.CurrentTankSpawn = gridTranformNearest;
                            selectedTank.IsMoving = false;
                        }
                        else
                        {
                            TankController tankInGrid = GetTankController(gridTranformNearest);

                            if ((tankInGrid.GetComponent<TankController>().TankType == selectedTank.TankType) && (selectedTank.TankType != TankType.Tank10))
                            {
                                TankType tankType = tankInGrid.GetComponent<TankController>().TankType;
                                dicTankSpawn[gridTranformNearest] = null;
                                dicTankSpawn[selectedTank.CurrentTankSpawn] = null;
                                tankInGrid.GetComponent<TankController>().DestroyTank();
                                selectedTank.DestroyTank();

                                TankType nextTankType = GetNextTankType(tankType);

                                TankController newTank = PoolManager.Instance.GetTankController(nextTankType);
                                newTank.transform.position = gridTranformNearest.transform.position;
                                newTank.CurrentTankSpawn = gridTranformNearest;
                                dicTankSpawn[gridTranformNearest] = newTank.gameObject;
                            }
                            else
                            {
                                selectedTank.MoveTankBackToGrid();
                            }
                        }
                    }
                }
                selectedTankPos = null;
            }
        }
    }


    public void SpawnTank(TankType tankType)
    {
        if(IsGameOver() == false)
        {
            for (int i = 0; i < dicTankSpawn.Count; i++)
            {
                Transform gridTranform = tankSpawns[i];
                GameObject valueTank = null;
                dicTankSpawn.TryGetValue(gridTranform, out valueTank);
                if (valueTank == null)
                {
                    TankController tank = PoolManager.Instance.GetTankController(tankType);
                    dicTankSpawn[gridTranform] = tank.gameObject;
                    tank.gameObject.transform.position = gridTranform.position;
                    tank.gameObject.SetActive(true);
                    tank.CurrentTankSpawn = gridTranform;
                    break;
                }
            }
        }
    }


    private IEnumerator SpawnEnemies()
    {
        int enemyOrder = 100;
        int waveIndex = 0;
        List<WaveConfig> waveConfigs = levelConfig.waveConfigs;
        while (IsGameOver() == false)
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
                enemyController.OnEnemyInit(enemyOrder,enemyConfig.damage,enemyConfig.health);
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
        yield return new WaitForSeconds(2f);

        List<BossConfig> m_bossConfigs = levelConfig.bossConfigs;
        for(int i = 0; i < m_bossConfigs.Count; i++)
        {
            BossConfig bossConfig = m_bossConfigs[i];
            BossController boss = PoolManager.Instance.GetBossController(bossConfig.bossType);
            Vector3 spawnPos = Vector3.zero;
            spawnPos.y = 9;
            spawnPos.x = Random.Range(-3f, 3f);
            boss.transform.position = spawnPos;
            boss.OnBossInit(enemyOrder, bossConfig.damage, bossConfig.health);
            enemyOrder--;
            m_bossCount++;
            yield return new WaitForSeconds(1);
        }

        while (m_bossCount > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2);
        CurrentLevel++;
        PlayerPrefs.SetInt(PlayerPrefsKey.LEVEL_KEY, CurrentLevel);
        ViewManager.Instance.GameView.OnCompleteLevel();
        yield break;

    }

             

    public void UpdateDeadEnemy()
    {
        m_enemyCount--;
    }    

    public void UpdateDeadBoss()
    {
        m_bossCount--;
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
        bool check = false;
        for (int i = 0;i < dicTankSpawn.Count; i++)
        {
            GameObject obj = null ;
            dicTankSpawn.TryGetValue(tankSpawns[i], out obj);
            if(transform == tankSpawns[i])
            {
                if (obj == null)
                {
                    check = false;
                }
                else
                {
                    check = true;
                }
                break;
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


    public void OnEnemyAttack(float damage)
    {
        MainHealth = MainHealth - damage;
    }

    public bool IsGameOver()
    {
        if (m_mainHealth == 0)
        {
            return true;
        }            
        else
        {
            return false;
        }           
    }


    public void BuyTank()
    {
        if(CurrentCoin >= levelConfig.CointToBuyTank)
        {
            int randomIdx = Random.Range(0, levelConfig.SpawnTanks.Count);
            TankType tankType = levelConfig.SpawnTanks[randomIdx];
            SpawnTank(tankType);
            CurrentCoin -= levelConfig.CointToBuyTank;
        }
    }



    private TankType GetNextTankType(TankType oldType)
    {
        TankType tankType = oldType ;
        switch(oldType)
        {
            case TankType.Tank01 : 
                tankType = TankType.Tank02;
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
            case TankType.Tank09:
                tankType = TankType.Tank10;
                break;
        }    
        return tankType;
    }
}
