using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public bool IsActiveBoss { private set; get; }


    private float totalHealth = 0;
    private float currentHealth = 0f;
    private int enemyAmount = 0;
    private int enemyWaveIndex = 0;
    private int bossWaveIndex = 0;
    private int UIWaveIndex = 0;

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

        //Spawn the init tanks
        for (int i = 0; i < levelConfig.InitTanks.Count; i++)
        {
            SpawnTank(levelConfig.InitTanks[i]);
        }
        //TankTypeToRandom = levelConfig.InitTanks[Random.Range(0, levelConfig.InitTanks.Count)];


        //Create the wave items for UI
        ViewManager.Instance.IngameView.CreateWaveItems(levelConfig.ListWaveConfig.Count, levelConfig.ListBossType.Count);

        //Init GameStart
        Invoke(nameof(GameStart), 0.15f);
    }

    private void Update()
    {
        if(GameState == GameState.GameStart)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButtonDown(0) && selectedTank == null)
            {
                float minDis = 1000f;
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                foreach(TankSpawnController tankSpawn in ListTankSpawns)
                {
                    if(tankSpawn.TankController != null && tankSpawn.TankController.IsMoving == false)
                    {
                        float distance = Vector2.Distance(tankSpawn.TankController.transform.position, touchPos);
                        if (distance < 0.5f && distance < minDis)
                        {
                            minDis = distance;
                            selectedTank = tankSpawn.TankController;
                        }
                    }
                }

                if (selectedTank != null)
                {
                    selectedTank.SetSelected(true, Vector2.zero, false);
                    originalTankSpawn = GetClosestTankSpawn(selectedTank.transform.position);
                }
            }
            if (Input.GetMouseButton(0) && selectedTank != null)
            {
                float newY = Mathf.Clamp(mousePos.y, -9.8f, 0f);
                selectedTank.transform.position = new Vector3(mousePos.x, newY, 0f);
            }
            if (Input.GetMouseButtonUp(0) && selectedTank != null)
            {
                TankSpawnController newTankSpawn = GetClosestTankSpawn(selectedTank.transform.position);
                if (newTankSpawn == null)
                {
                    selectedTank.SetSelected(false, originalTankSpawn.transform.position, false);
                }
                else
                {
                    if (newTankSpawn.Equals(originalTankSpawn))
                    {
                        selectedTank.SetSelected(false, originalTankSpawn.transform.position, true);
                    }
                    else
                    {
                        if (newTankSpawn.TankController == null)
                        {
                            originalTankSpawn.TankController = null;
                            newTankSpawn.TankController = selectedTank;
                            selectedTank.SetSelected(false, newTankSpawn.transform.position, true);
                        }
                        else
                        {
                            if ((newTankSpawn.TankController.TankType == selectedTank.TankType) && (selectedTank.TankType != TankType.Tank10))
                            {
                                TankType nextTankType = GetNextTankType(selectedTank.TankType);
                                newTankSpawn.TankController.gameObject.SetActive(false);
                                selectedTank.gameObject.SetActive(false);

                                TankController newTank = PoolManager.Instance.GetTankController(nextTankType);
                                newTank.transform.position = newTankSpawn.transform.position;
                                newTankSpawn.TankController = newTank;
                                newTank.OnTankInit();
                            }
                            else
                            {
                                selectedTank.SetSelected(false, originalTankSpawn.transform.position, false);
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
        StartCoroutine(CRSpawnNextEnemyWave(1f));
        ViewManager.Instance.IngameView.ShowTextForFirstWave();
        IsActiveBoss = false;
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
            TankSpawnController tankSpawn = ListTankSpawns[i];
            if (tankSpawn.TankController == null)
            {
                TankController tank = PoolManager.Instance.GetTankController(tankType);
                tankSpawn.TankController = tank;
                tank.gameObject.transform.position = tankSpawn.gameObject.transform.position;
                tank.OnTankInit();
                break;
            }
        }
    }




    /// <summary>
    /// Coroutine spawn the next enemy wave.
    /// </summary>
    /// <param name="delayTime"></param>
    /// <returns></returns>
    private IEnumerator CRSpawnNextEnemyWave(float delayTime)
    {
        enemyAmount = Random.Range(levelConfig.ListWaveConfig[enemyWaveIndex].minEnemyAmount, levelConfig.ListWaveConfig[enemyWaveIndex].maxEnemyAmount);
        int enemyOrder = 1000;
        WaveConfig waveConfig = levelConfig.ListWaveConfig[enemyWaveIndex];
        yield return new WaitForSeconds(delayTime);
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
    /// Coroutine spawn the next boss wave.
    /// </summary>
    /// <param name="delayTime"></param>
    /// <returns></returns>
    private IEnumerator CRSpawnNextBossWave(float delayTime)
    {
        BossType bossType = levelConfig.ListBossType[bossWaveIndex];
        yield return new WaitForSeconds(delayTime);
        BossController bossController = PoolManager.Instance.GetBossController(bossType);
        Vector3 spawnPos = Vector3.zero;
        spawnPos.y = Camera.main.ViewportToWorldPoint(new Vector2(0f, 1.2f)).y;
        spawnPos.x = Random.Range(-4.5f, 4.5f);
        bossController.transform.position = spawnPos;
        bossController.OnBossInit(1000);
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


  
  

    /// <summary>
    /// Get the closest TankSpawnController with given position.
    /// </summary>
    /// <param name="tankPos"></param>
    /// <returns></returns>
    private TankSpawnController GetClosestTankSpawn(Vector3 tankPos)
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



    /// <summary>
    /// Get the next TankType.
    /// </summary>
    /// <param name="oldType"></param>
    /// <returns></returns>
    private TankType GetNextTankType(TankType oldType)
    {
        TankType tankType = oldType;
        switch (oldType)
        {
            case TankType.Tank01:
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




    /// <summary>
    /// Update the dead enemy by enemyAmount--;
    /// </summary>
    public void UpdateDeadEnemy()
    {
        enemyAmount--;
        if (enemyAmount == 0)
        {
            if (enemyWaveIndex == levelConfig.ListWaveConfig.Count - 1)
            {
                if (levelConfig.ListBossType.Count == 0)
                {
                    //CurrentLevel++;
                    //PlayerPrefs.SetInt(PlayerPrefsKey.LEVEL_KEY, CurrentLevel);
                    //ViewManager.Instance.IngameView.OnCompleteLevel();
                }
                else
                {
                    //Update the wave on UI
                    ViewManager.Instance.IngameView.OnWaveCompleted(UIWaveIndex);
                    UIWaveIndex++;

                    //Spawn the first boss
                    IsActiveBoss = true;
                    StartCoroutine(CRSpawnNextBossWave(1f));
                }
            }
            else
            {
                //Update the wave on UI
                ViewManager.Instance.IngameView.OnWaveCompleted(UIWaveIndex);
                UIWaveIndex++;

                enemyWaveIndex++;
                StartCoroutine(CRSpawnNextEnemyWave(1f));
            }
        }
    }


    public void UpdateDeadBoss()
    {
        if (bossWaveIndex == levelConfig.ListBossType.Count - 1)
        {
            //Update the wave on UI
            ViewManager.Instance.IngameView.OnWaveCompleted(UIWaveIndex);

            //Kill all the bosses -> complete level
        }
        else
        {
            //Update the wave on UI
            ViewManager.Instance.IngameView.OnWaveCompleted(UIWaveIndex);
            UIWaveIndex++;

            //Spawn the next boss
            bossWaveIndex++;
            StartCoroutine(CRSpawnNextBossWave(1f));
        }
    }
}
