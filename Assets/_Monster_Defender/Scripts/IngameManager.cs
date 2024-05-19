using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameManager : MonoBehaviour
{
    public static IngameManager Instance { get; private set; }

    [Header("Ingame Configs")]
    [SerializeField] private float bulletMovementSpeed = 70f;


    [Header("Ingame References")]
    [SerializeField] private Transform healthBar = null;
    [SerializeField] private List<TankSpawnController> ListTankSpawns = new List<TankSpawnController>();


    private LevelConfigSO levelConfig = null;
    private TankController selectedTank = null;
    private TankSpawnController originalTankSpawn = null;

    public int CurrentCoin 
    {
        get { return PlayerPrefs.GetInt(PlayerPrefsKey.COIN_KEY, 0); }
        set
        {
            PlayerPrefs.SetInt(PlayerPrefsKey.COIN_KEY, value);
        }
    }

    public GameState GameState { private set; get; }
    public int CurrentLevel { private set; get; }
    public float BaseHealth { private set; get; }   
    public TankType TankTypeToRandom { get ; private set ; }

    private float totalHealth = 0;
    private float currentHealth = 0f;

    private int m_enemyCount = 0;
    private int m_bossCount = 0;
    private int m_waveIndex = 0;

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
        GameState = GameState.GameInit;
        ViewManager.Instance.SetActiveView(ViewType.IngameView);

        //Load level
        CurrentLevel = PlayerPrefs.GetInt(PlayerPrefsKey.LEVEL_KEY, 1);
        levelConfig = Resources.Load("Levels/" + CurrentLevel.ToString(), typeof(LevelConfigSO)) as LevelConfigSO;

        //Setup health bar
        totalHealth = levelConfig.HealthAmount;
        currentHealth = levelConfig.HealthAmount;

        //Spawn the tanks
        for (int i = 0; i < levelConfig.InitTanks.Count; i++)
        {
            SpawnTank(levelConfig.InitTanks[i]);
        }
        TankTypeToRandom = levelConfig.InitTanks[Random.Range(0, levelConfig.InitTanks.Count)];


        //Init GameStart
        Invoke(nameof(GameStart), 1f);
    }

    private void Update()
    {
        if(GameState == GameState.GameStart && IsGameOver() == false)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButtonDown(0))
            {
                Collider2D collider2d = Physics2D.OverlapCircle(mousePos, 0.5f);
                if ((collider2d != null) && collider2d.gameObject.CompareTag("Tank"))
                {
                    selectedTank = collider2d.GetComponent<TankController>();
                    selectedTank.SortingOder = 100;
                    originalTankSpawn = GetNearestTankSpawn(selectedTank.transform.position);
                }
            }
            if (Input.GetMouseButton(0) && selectedTank != null)
            {
                selectedTank.OnTankMove();
                float newY = Mathf.Clamp(mousePos.y, -9.8f, 0f);
                selectedTank.transform.position = new Vector3(mousePos.x, newY, 0f);

            }
            if (Input.GetMouseButtonUp(0) && selectedTank != null)
            {
                TankSpawnController newTankSpawn = GetNearestTankSpawn(selectedTank.transform.position);
                if (newTankSpawn == null)
                {
                    selectedTank.MoveTankBackToGrid(originalTankSpawn.transform.position);
                }
                else
                {
                    if (newTankSpawn.Equals(originalTankSpawn))
                    {
                        selectedTank.transform.position = newTankSpawn.transform.position;
                        selectedTank.IsMoving = false;
                        selectedTank.OnTankInit();
                    }
                    else
                    {
                        if (newTankSpawn.TankController == null)
                        {
                            originalTankSpawn.TankController = null;
                            newTankSpawn.TankController = selectedTank;
                            selectedTank.transform.position = newTankSpawn.transform.position;
                            selectedTank.IsMoving = false;
                            selectedTank.OnTankInit();
                        }
                        else
                        {
                            if ((newTankSpawn.TankController.TankType == selectedTank.TankType) && (selectedTank.TankType != TankType.Tank10))
                            {
                                TankType nextTankType = GetNextTankType(selectedTank.TankType);

                                Destroy(newTankSpawn.TankController);
                                Destroy(selectedTank);

                                TankController newTank = PoolManager.Instance.GetTankController(nextTankType);
                                newTank.transform.position = newTankSpawn.transform.position;
                                newTankSpawn.TankController = newTank;
                                newTank.OnTankInit();
                            }
                            else
                            {
                                selectedTank.MoveTankBackToGrid(originalTankSpawn.transform.position);
                            }
                        }
                    }
                }


                selectedTank = null;
                originalTankSpawn = null;
            }
        }
    }



    /// <summary>
    /// Init GameStart state.
    /// </summary>
    public void GameStart()
    {
        GameState = GameState.GameStart;
        ViewManager.Instance.GameView.SetWaveTextEnemy(levelConfig.waveConfigs.Count, m_waveIndex);
    }


    /// <summary>
    /// Init GamePause state
    /// </summary>
    public void GamePause()
    {
        GameState = GameState.GamePause;
    }


    /// <summary>
    /// Resume the game
    /// </summary>
    public void GameResume()
    {
        GameState = GameState.GameStart;
    }


    private void SpawnTank(TankType tankType)
    {
        if(IsGameOver() == false)
        {
            for (int i = 0; i < ListTankSpawns.Count; i++)
            {
                TankSpawnController tankSpawn = ListTankSpawns[i].GetComponent<TankSpawnController>();
                if (tankSpawn.TankController == null)
                {
                    TankController tank = PoolManager.Instance.GetTankController(tankType);
                    tankSpawn.TankController = tank;
                    tank.gameObject.transform.position = tankSpawn.gameObject.transform.position;
                    tank.gameObject.SetActive(true);
                    tank.OnTankInit();
                    break;
                }
            }
        }
    }


    public void SpawnNextWave()
    {
        m_enemyCount = levelConfig.waveConfigs[m_waveIndex].enemyConfigs.Count;
        StartCoroutine(SpawnEnemiesOfWave());
    }
    private IEnumerator SpawnEnemiesOfWave()
    {
        int enemyOrder = 100;
        WaveConfig waveConfig = levelConfig.waveConfigs[m_waveIndex];
        for (int i = 0; i < waveConfig.enemyConfigs.Count; i++)
        {
            EnemyConfig enemyConfig = waveConfig.enemyConfigs[i];
            EnemyController enemyController = PoolManager.Instance.GetEnemyController(enemyConfig.enemyType);
            Vector3 spawnPos = Vector3.zero;
            spawnPos.y = 9;
            spawnPos.x = Random.Range(-4.7f, 4.7f);
            enemyController.transform.position = spawnPos;
            enemyController.OnEnemyInit(enemyOrder, enemyConfig.damage, enemyConfig.health);
            enemyOrder--;
            yield return new WaitForSeconds(1);
        }
    }

  
    
    public void SpawnBoss()
    {
        StartCoroutine(SpawnBossEnemy());
    }    

    private IEnumerator SpawnBossEnemy()
    {
        int bossOrder = 100;
        for (int i = 0; i < levelConfig.bossConfig.Count; i++)
        {
            BossConfig bossConfig = levelConfig.bossConfig[i];
            BossController boss = PoolManager.Instance.GetBossController(bossConfig.bossType);
            Vector3 spawnPos = Vector3.zero;
            spawnPos.y = 9;
            spawnPos.x = Random.Range(-3f, 3f);
            boss.transform.position = spawnPos;
            boss.OnBossInit(bossOrder, bossConfig.damage, bossConfig.health);
            bossOrder--;
            m_bossCount++;
            yield return new WaitForSeconds(1);
        }
        yield break;
    }



    public void UpdateDeadEnemy()
    {
        m_enemyCount--;
        if (m_enemyCount == 0)
        {
            if (m_waveIndex == levelConfig.waveConfigs.Count - 1)
            {
                if (levelConfig.bossConfig.Count == 0)
                {
                    CurrentLevel++;
                    PlayerPrefs.SetInt(PlayerPrefsKey.LEVEL_KEY, CurrentLevel);
                    ViewManager.Instance.GameView.OnCompleteLevel();
                }
                else
                {
                    ViewManager.Instance.GameView.SetWaveTextBoss();
                }
            }
            else
            {
                m_waveIndex++;
                ViewManager.Instance.GameView.SetWaveTextEnemy(levelConfig.waveConfigs.Count, m_waveIndex);
            }  
        }
    }    

    public void UpdateDeadBoss()
    {
        m_bossCount--;
        if(m_bossCount == 0)
        {
            CurrentLevel++;
            PlayerPrefs.SetInt(PlayerPrefsKey.LEVEL_KEY, CurrentLevel);
            ViewManager.Instance.GameView.OnCompleteLevel();
        }
    }



    private TankSpawnController GetNearestTankSpawn(Vector3 tankPos)
    {
        float minDistance = 0.5f;
        TankSpawnController cur = null;
        for (int i = 0; i < ListTankSpawns.Count; i++)
        {
            float distance = Vector3.Distance(tankPos, ListTankSpawns[i].gameObject.transform.position);
            if ((distance < minDistance))
            {
                minDistance = distance;
                cur = ListTankSpawns[i];
            }
        }
        return cur;
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