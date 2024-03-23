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
    }

    public void OnClickBuyTanks()
    {
        TankType tankType = GameManager.Instance.TankType;
        GameManager.Instance.SpawnTank(tankType);
    }
}
