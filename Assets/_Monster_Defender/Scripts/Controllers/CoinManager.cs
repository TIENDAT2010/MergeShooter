﻿using System.Collections;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [Header("Coin Manager Configuration")]
    [SerializeField] private int initialCoins = 100;
    //[SerializeField] private int minRewardedCoins = 100;
    //[SerializeField] private int maxRewardedCoins = 150;


    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }




    public int Coins
    {
        private set { PlayerPrefs.SetInt(PlayerPrefsKey.COIN_KEY, value); }
        get { return PlayerPrefs.GetInt(PlayerPrefsKey.COIN_KEY, initialCoins); }
    }



    ///// <summary>
    ///// Get an amount of coins to reward to user.
    ///// </summary>
    ///// <returns></returns>
    //public int GetRewardedCoins()
    //{
    //    return Random.Range(minRewardedCoins, maxRewardedCoins) / 5 * 5;
    //}



    /// <summary>
    /// Add an amount of coins immediately.
    /// </summary>
    /// <param name="amount"></param>
    public void AddCoins(int amount)
    {
        Coins += amount;
    }

    /// <summary>
    /// Add an amount of coins with coroutine.
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="delay"></param>
    public void AddCoins(int amount, float delay)
    {
        int startCoins = Coins;
        int endCoins = Coins + amount;
        StartCoroutine(CRUpdateCoins(startCoins, endCoins, delay));
    }



    /// <summary>
    /// Remove an amount of coins.
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="delay"></param>
    public void RemoveCoins(int amount, float delay)
    {
        int startCoins = Coins;
        int endCoins = Coins - amount;
        StartCoroutine(CRUpdateCoins(startCoins, endCoins, delay));
    }



    /// <summary>
    /// Coroutine updating the coins with start coins, end coins and delay time.
    /// </summary>
    /// <param name="startCoins"></param>
    /// <param name="endCoins"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator CRUpdateCoins(int startCoins, int endCoins, float delay)
    {
        yield return new WaitForSeconds(delay);
        //if (endCoins > startCoins) { ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Rewarded); }
        float t = 0;
        float runTime = 0.25f;
        while (t < runTime)
        {
            t += Time.deltaTime;
            float factor = t / runTime;
            int newCoins = Mathf.RoundToInt(Mathf.Lerp(startCoins, endCoins, factor));
            Coins = newCoins;
            yield return null;
        }
        Coins = endCoins;
    }
}
