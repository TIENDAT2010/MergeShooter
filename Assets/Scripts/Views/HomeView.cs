using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeView : BaseView
{
    public override void OnShow()
    {
        gameObject.SetActive(true);
    }

    public override void OnHide() 
    {
        gameObject.SetActive(false);
    }


    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
