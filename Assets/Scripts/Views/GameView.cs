using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class GameView : BaseView
{
    [SerializeField] private GameObject m_CompleteLevelPanel = null;
    [SerializeField] private TextMeshProUGUI levelText = null;
    [SerializeField] private Image mainHealthBar = null;
    [SerializeField] private Text healthText = null;
    [SerializeField] private TextMeshProUGUI coinText = null;
    [SerializeField] private TextMeshProUGUI waveText = null;
    [SerializeField] private GameObject m_GameOverPanel = null;
    [SerializeField] private GameObject m_PauseGamePanel = null;


    private void Update()
    {
        healthText.text = "HP : " + GameManager.Instance.MainHealth.ToString();
        coinText.text = GameManager.Instance.CurrentCoin.ToString();

        mainHealthBar.fillAmount = GameManager.Instance.MainHealth / GameManager.Instance.BaseHealth;
        if(GameManager.Instance.MainHealth <= 0)
        {
            m_GameOverPanel.SetActive(true);
        }
    }

    public override void OnShow()
    {
        waveText.gameObject.SetActive(false);
        m_CompleteLevelPanel.SetActive(false);
        m_GameOverPanel.SetActive(false);
        m_PauseGamePanel.SetActive(false);
        levelText.text = "Level: " + GameManager.Instance.CurrentLevel.ToString();
        mainHealthBar.fillAmount = 1f;
    }

    public override void OnHide() 
    { 
        gameObject.SetActive(false);
    }

    public void OnCompleteLevel()
    {
        m_CompleteLevelPanel.SetActive(true);
    }    



    public void OnClickNextLevelButton()
    {
        SceneManager.LoadScene(1);

    }

    public void OnClickReplayButton()
    {
        SceneManager.LoadScene(1);
    }


    public void OnClickExitButton()
    {
        Application.Quit();
    }

    public void OnClickPauseGameBtn()
    {
        GameManager.Instance.GamePause();
        m_PauseGamePanel.SetActive(true);
    }


    public void OnClickBuyTanks()
    {
        GameManager.Instance.BuyTank();
    }

    public void ResumeGame()
    {
        GameManager.Instance.GameResume();
        m_PauseGamePanel.SetActive(false);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    public void SetWaveTextEnemy(int totalWave, int currentWave)
    {
        waveText.text = "Wave : " + (currentWave+1).ToString() + " - " + totalWave.ToString();
        waveText.gameObject.SetActive(true);
        StartCoroutine(TextAnimation(0));
    }

    private IEnumerator TextAnimation(int a)
    {
        while (waveText.gameObject.activeSelf)
        {
            float t = 0;
            float moveTime = 1f;
            Vector3 startVector3 = Vector3.zero;
            Vector3 endVector3 = new Vector3(2, 2, 0);
            while (t < moveTime)
            {
                t += Time.deltaTime;
                float factor = t / moveTime;
                waveText.transform.localScale = Vector3.Lerp(startVector3, endVector3, factor);
                yield return null;
            }
            t = 0;
            while (t < moveTime)
            {
                t += Time.deltaTime;
                float factor = t / moveTime;
                waveText.transform.localScale = Vector3.Lerp(endVector3, startVector3, factor);
                yield return null;
            }
            break;
        }

        yield return new WaitForSeconds(1f);
        waveText.gameObject.SetActive(false);
        if (a == 0)
        {
            GameManager.Instance.SpawnNextWave();
        }
        else
        {
            GameManager.Instance.SpawnBoss();
        }     
    }    
    
    public void SetWaveTextBoss()
    {
        waveText.text = "Boss is Coming !!!";
        waveText.gameObject.SetActive(true);
        StartCoroutine(TextAnimation(1));
    }    
}
