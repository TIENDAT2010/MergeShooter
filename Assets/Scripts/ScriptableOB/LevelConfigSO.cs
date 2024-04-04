using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "LevelConfiguration/LevelConfig", order = 1)]
public class LevelConfigSO : ScriptableObject
{
    public float MainHealth = 0f;
    public Sprite BackGround = null;
    public int CointToBuyTank = 0;
    public List<TankType> SpawnTanks = new List<TankType>();
    public List<TankType> InitTanks = new List<TankType>();
    public List<WaveConfig> waveConfigs = new List<WaveConfig>();
    public List<BossConfig> bossConfig = new List<BossConfig>();
}


[Serializable]
public class WaveConfig
{
    public List<EnemyConfig> enemyConfigs = new List<EnemyConfig>();
}


[Serializable]
public class EnemyConfig
{
    public EnemyType enemyType;
    public float damage, health;
}

[Serializable]
public class BossConfig
{
    public BossType bossType;
    public float damage, health;
}
