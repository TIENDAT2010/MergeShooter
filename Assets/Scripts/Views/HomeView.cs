using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeView : BaseView
{
    [SerializeField] private TankViewController tankViewPanel = null;
    [SerializeField] private GameObject startGameBtn = null;
    [SerializeField] private GameObject gameImage = null;
    [SerializeField] private GameObject upgradeTankBtn = null;

    public override void OnShow()
    {
        gameObject.SetActive(true);
        tankViewPanel.gameObject.SetActive(false);
    }

    public override void OnHide() 
    {
        gameObject.SetActive(false);
    }


    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ClickTankInforBtn()
    {
        tankViewPanel.gameObject.SetActive(true);
        tankViewPanel.OnInit();
        startGameBtn.SetActive(false);
        gameImage.SetActive(false);
        upgradeTankBtn.SetActive(false);
    }

    public void ClickExitPanelBtn()
    {
        tankViewPanel.gameObject.SetActive(false);
        startGameBtn.SetActive(true);
        gameImage.SetActive(true);
        upgradeTankBtn.SetActive(true);
    }
        

}
