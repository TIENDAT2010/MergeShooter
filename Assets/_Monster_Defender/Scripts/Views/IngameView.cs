using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class IngameView : BaseView
{
    [SerializeField] private Text levelText = null;
    [SerializeField] private Text waveText = null;
    [SerializeField] private RectTransform wavePanelTrans = null;
    [SerializeField] private Transform wavesPanelTrans = null;
    [SerializeField] private WaveItemController waveItemControllerPrefab = null;



    private List<WaveItemController> listActiveWaveItem = new List<WaveItemController>();
    private List<WaveItemController> listWaveItemController = new List<WaveItemController>();


    /// <summary>
    /// ////////////////////////////////////////////// Private Functions
    /// </summary>


    /// <summary>
    /// Get a WaveItemController object.
    /// </summary>
    /// <returns></returns>
    private WaveItemController GetWaveItemController()
    {
        //Find the object in the list
        WaveItemController waveItem = listWaveItemController.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

        if (waveItem == null)
        {
            //Instantiate the dead effect
            waveItem = Instantiate(waveItemControllerPrefab, Vector3.zero, Quaternion.identity);
            listWaveItemController.Add(waveItem);
        }

        waveItem.gameObject.SetActive(true);
        return waveItem;
    }




    /// <summary>
    /// Coroutine handle the given wave is completed. 
    /// </summary>
    /// <param name="waveIndex"></param>
    /// <returns></returns>
    private IEnumerator CROnWaveCompleted(int waveIndex)
    {
        if (waveIndex < listWaveItemController.Count - 1)
        {
            listActiveWaveItem[waveIndex].UpdateSlider();
            yield return new WaitForSeconds(0.5f);
            listActiveWaveItem[waveIndex + 1].OnActive(true);
            StartCoroutine(CRShowWaveText(waveIndex + 2));
        }
    }



    /// <summary>
    /// Coroutine show the wave text.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    private IEnumerator CRShowWaveText(int number)
    {
        Vector2 startPos = new Vector2(1000f, wavePanelTrans.anchoredPosition.y);
        Vector2 midPos = new Vector2(0f, wavePanelTrans.anchoredPosition.y);
        Vector2 endPos = new Vector2(-1000f, wavePanelTrans.anchoredPosition.y);

        wavePanelTrans.anchoredPosition = startPos;
        waveText.text = "WAVE: " + number.ToString();

        float t = 0;
        float moveTime = 0.5f;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            wavePanelTrans.anchoredPosition = Vector2.Lerp(startPos, midPos, factor);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        t = 0;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            wavePanelTrans.anchoredPosition = Vector2.Lerp(midPos, endPos, factor);
            yield return null;
        }
    }



    /// <summary>
    /// ////////////////////////////////////////////// Public Functions
    /// </summary>


    public override void OnShow()
    {
        //Show the level
        levelText.text = "LEVEL: " + IngameManager.Instance.CurrentLevel.ToString();

        //Hide the wave text
        wavePanelTrans.anchoredPosition = new Vector2(1000f, wavePanelTrans.anchoredPosition.y);
    }

    public override void OnHide() 
    {
        //Disable all wave items
        foreach(WaveItemController waveItem in listActiveWaveItem)
        {
            waveItem.gameObject.SetActive(false);
        }
        listActiveWaveItem.Clear();


        gameObject.SetActive(false);
    }


    /// <summary>
    /// Create the wave items with given wave amount.
    /// </summary>
    /// <param name="enemyWaveAmount"></param>
    /// <param name="bossWaveAmount"></param>
    public void CreateWaveItems(int enemyWaveAmount, int bossWaveAmount)
    {
        //Create the wave items for enemy waves
        for (int i = 0; i < enemyWaveAmount; i++)
        {
            WaveItemController waveItem = GetWaveItemController();
            waveItem.transform.SetParent(wavesPanelTrans);
            waveItem.transform.localScale = Vector3.one;
            listActiveWaveItem.Add(waveItem);
            waveItem.OnActive(i == 0);
            waveItem.SetupSprite(0);
        }

        //Create the wave items for boss waves
        for (int i = 0; i < bossWaveAmount; i++)
        {
            WaveItemController waveItem = GetWaveItemController();
            waveItem.transform.SetParent(wavesPanelTrans);
            waveItem.transform.localScale = Vector3.one;
            listActiveWaveItem.Add(waveItem);
            waveItem.OnActive(false);
            waveItem.SetupSprite(1);
        }
    }


    /// <summary>
    /// Show the wave text of the first wave.
    /// </summary>
    public void ShowTextForFirstWave()
    {
        StartCoroutine(CRShowWaveText(1));
    }


    /// <summary>
    /// Handle UI when the given wave index is completed.
    /// </summary>
    /// <param name="waveIndex"></param>
    public void OnWaveCompleted(int waveIndex)
    {
        StartCoroutine(CROnWaveCompleted(waveIndex));
    }
}
