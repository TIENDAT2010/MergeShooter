using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameView : BaseView
{
    [SerializeField] private Button m_StartGame = null;
    [SerializeField] private GameObject m_CompleteLevelPanel = null;
    [SerializeField] private TextMeshProUGUI levelText = null;
    [SerializeField] private Image mainHealthBar = null;
    [SerializeField] private Text healthText = null;
    [SerializeField] private TextMeshProUGUI coinText = null;

    private float m_MaxmainHP = 0f;
    private float m_PreMainHP = 0f;
    private float m_Coin = 0f;

    private void Update()
    {
        healthText.text = "HP : " + m_PreMainHP.ToString();
    }

    public void OnEnemyAttack(float damage)
    {
        m_PreMainHP = m_PreMainHP - damage;
        mainHealthBar.fillAmount = m_PreMainHP / m_MaxmainHP;
    }


    public override void OnShow()
    {
        m_CompleteLevelPanel.SetActive(false);
        m_StartGame.gameObject.SetActive(true);
        levelText.text = "Level: " + GameManager.Instance.CurrentLevel.ToString();
    }

    public override void OnHide() 
    { 
        gameObject.SetActive(false);
    }

    public void OnCompleteLevel()
    {
        m_CompleteLevelPanel.SetActive(true);
        m_StartGame.gameObject.SetActive(false);
    }    



    public void OnClickNextLevelButton()
    {
        SceneManager.LoadScene(1);

    }

    public void OnClickStartBtn()
    {
        GameManager.Instance.GameStart();
        m_StartGame.gameObject.SetActive(false);
        m_MaxmainHP = GameManager.Instance.MainHealth;
        m_PreMainHP = m_MaxmainHP;
        mainHealthBar.fillAmount = 1f;
    }

    public void OnClickBuyTanks()
    {
        TankType tankType = GameManager.Instance.TankTypeToRandom;
        GameManager.Instance.SpawnTank(tankType);
    }

    public void SetCoinText(float coin)
    {
        m_Coin = m_Coin + coin;
        coinText.text = m_Coin.ToString();
    }
}
