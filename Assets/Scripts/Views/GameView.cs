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
    [SerializeField] private GameObject m_GameOverPanel = null;
    [SerializeField] private GameObject m_PauseGamePanel = null;


    private float m_MaxmainHP = 0f;
    private float m_PreMainHP = 0f;

    public bool m_GameOver { private set; get; } = false;


    private void Update()
    {
        healthText.text = "HP : " + m_PreMainHP.ToString();
        coinText.text = GameManager.Instance.CurrentCoin.ToString();
    }

    public void OnEnemyAttack(float damage)
    {
        m_PreMainHP = m_PreMainHP - damage;
        mainHealthBar.fillAmount = m_PreMainHP / m_MaxmainHP;
        if(m_PreMainHP == 0f)
        {
            m_GameOverPanel.SetActive(true);
            m_GameOver = true;
        }
    }


    public override void OnShow()
    {
        m_CompleteLevelPanel.SetActive(false);
        m_GameOverPanel.SetActive(false);
        m_PauseGamePanel.SetActive(false);
        levelText.text = "Level: " + GameManager.Instance.CurrentLevel.ToString();

        m_MaxmainHP = GameManager.Instance.MainHealth;
        m_PreMainHP = m_MaxmainHP;
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
        Time.timeScale = 0f;
        m_PauseGamePanel.SetActive(true);
    }


    public void OnClickBuyTanks()
    {
        if(GameManager.Instance.CurrentCoin >= 100)
        {
            TankType tankType = GameManager.Instance.TankTypeToRandom;
            GameManager.Instance.SpawnTank(tankType);
            GameManager.Instance.CurrentCoin -= 100;
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        m_PauseGamePanel.SetActive(false);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }    
}
