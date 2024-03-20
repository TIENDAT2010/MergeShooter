using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "LevelConfiguration/LevelConfig", order = 1)]
public class LevelConfigSO : ScriptableObject
{
    public Sprite BackGround = null;
    public List<TankType> InitTanks = new List<TankType>();
    public List<WaveConfig> waveConfigs = new List<WaveConfig>();
}


[Serializable]
public class WaveConfig
{
    public float enemySpeed;
    public List<EnemyConfig> enemyConfigs = new List<EnemyConfig>();
}

[Serializable]
public class EnemyConfig
{
    public EnemyType enemyType;
    public float damage, health;
}
