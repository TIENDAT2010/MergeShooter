using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsKey
{
    public const string LEVEL_KEY = "levelkey";
    public const string COIN_KEY = "COIN_KEY";


    public static void SaveTankLevel(TankType tankType, int upgradeLevel)
    {
        PlayerPrefs.SetInt(tankType.ToString(), upgradeLevel);
        PlayerPrefs.Save();
    }


    public static int GetTankLevel(TankType tankType)
    {
        return PlayerPrefs.GetInt(tankType.ToString(), 1);
    }
}
