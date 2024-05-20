using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameManager : MonoBehaviour
{
    public static IngameManager Instance { get; private set; }

    //[Header("Ingame Configs")]
    //[SerializeField] private float bulletMovementSpeed = 70f;


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
    public TankType TankTypeToRandom { get ; private set ; }


    private float totalHealth = 0;
    private float currentHealth = 0f;
    private int enemyAmount = 0;
    private int bossAmount = 0;
    private int waveIndex = 0;

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
        if(GameState == GameState.GameStart)
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





    ////////////////////////////////////////////////// State Functions



    /// <summary>
    /// Init GameStart state.
    /// </summary>
    public void GameStart()
    {
        GameState = GameState.GameStart;
        StartCoroutine(CRSpawnNextWave());
        //ViewManager.Instance.GameView.SetWaveTextEnemy(levelConfig.waveConfigs.Count, waveIndex);
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



    ////////////////////////////////////////////////// Private Functions




    private void SpawnTank(TankType tankType)
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



    /// <summary>
    /// Coroutine spawn the next wave.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CRSpawnNextWave()
    {
        enemyAmount = Random.Range(levelConfig.waveConfigs[waveIndex].minEnemyAmount, levelConfig.waveConfigs[waveIndex].maxEnemyAmount);
        int enemyOrder = 100;
        WaveConfig waveConfig = levelConfig.waveConfigs[waveIndex];
        for (int i = 0; i < enemyAmount; i++)
        {
            EnemyController enemyController = PoolManager.Instance.GetEnemyController(GetEnemyType(waveConfig.enemyTypeConfigs));
            Vector3 spawnPos = Vector3.zero;
            spawnPos.y = Camera.main.ViewportToWorldPoint(new Vector2(0f, 1.2f)).y;
            spawnPos.x = Random.Range(-4.5f, 4.5f);
            enemyController.transform.position = spawnPos;
            enemyController.OnEnemyInit(enemyOrder);
            enemyOrder--;
            yield return new WaitForSeconds(waveConfig.enemyDelayTime);
        }
    }



    /// <summary>
    /// Get the EnemyType based on List<EnemyTypeConfig>.
    /// </summary>
    /// <param name="enemyConfigs"></param>
    /// <returns></returns>
    private EnemyType GetEnemyType(List<EnemyTypeConfig> enemyConfigs)
    {
        //Calculate the total frequency
        float totalFreq = 0;
        foreach (EnemyTypeConfig configuration in enemyConfigs)
        {
            totalFreq += configuration.frequency;
        }

        float randomFreq = Random.Range(0, totalFreq);
        for (int i = 0; i < enemyConfigs.Count; i++)
        {
            if (randomFreq < enemyConfigs[i].frequency)
            {
                return enemyConfigs[i].enemyType;
            }
            else
            {
                randomFreq -= enemyConfigs[i].frequency;
            }
        }

        return enemyConfigs[0].enemyType;
    }


  
    
    public void SpawnBoss()
    {
        //StartCoroutine(SpawnBossEnemy());
    }    

    //private IEnumerator SpawnBossEnemy()
    //{
    //    int bossOrder = 100;
    //    for (int i = 0; i < levelConfig.bossConfig.Count; i++)
    //    {
    //        BossConfig bossConfig = levelConfig.bossConfig[i];
    //        BossController boss = PoolManager.Instance.GetBossController(bossConfig.bossType);
    //        Vector3 spawnPos = Vector3.zero;
    //        spawnPos.y = 9;
    //        spawnPos.x = Random.Range(-3f, 3f);
    //        boss.transform.position = spawnPos;
    //        boss.OnBossInit(bossOrder, bossConfig.damage, bossConfig.health);
    //        bossOrder--;
    //        bossAmount++;
    //        yield return new WaitForSeconds(1);
    //    }
    //    yield break;
    //}


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
   


    ////////////////////////////////////////////////// Public Functions





    /// <summary>
    /// Handle the health bar take damage.
    /// </summary>
    /// <param name="damage"></param>
    public void OnTakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, totalHealth);
        healthBar.localScale = new Vector3(currentHealth / totalHealth, 1f, 1f);
    }




    public void UpdateDeadEnemy()
    {
        enemyAmount--;
        if (enemyAmount == 0)
        {
            if (waveIndex == levelConfig.waveConfigs.Count - 1)
            {
                //if (levelConfig.bossConfig.Count == 0)
                //{
                //    CurrentLevel++;
                //    PlayerPrefs.SetInt(PlayerPrefsKey.LEVEL_KEY, CurrentLevel);
                //    ViewManager.Instance.GameView.OnCompleteLevel();
                //}
                //else
                //{
                //    ViewManager.Instance.GameView.SetWaveTextBoss();
                //}
            }
            else
            {
                waveIndex++;
                ViewManager.Instance.GameView.SetWaveTextEnemy(levelConfig.waveConfigs.Count, waveIndex);
            }
        }
    }

    public void UpdateDeadBoss()
    {
        bossAmount--;
        if (bossAmount == 0)
        {
            CurrentLevel++;
            PlayerPrefs.SetInt(PlayerPrefsKey.LEVEL_KEY, CurrentLevel);
            ViewManager.Instance.GameView.OnCompleteLevel();
        }
    }



    public void BuyTank()
    {
        //if(CurrentCoin >= levelConfig.CointToBuyTank)
        //{
        //    int randomIdx = Random.Range(0, levelConfig.SpawnTanks.Count);
        //    TankType tankType = levelConfig.SpawnTanks[randomIdx];
        //    SpawnTank(tankType);
        //    CurrentCoin -= levelConfig.CointToBuyTank;
        //}
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
