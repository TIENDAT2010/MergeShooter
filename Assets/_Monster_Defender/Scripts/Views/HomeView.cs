using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeView : BaseView
{
    [SerializeField] private Transform startButtonTrans = null;


    /// <summary>
    /// ////////////////////////////////////////////// Private Functions
    /// </summary>



    /// <summary>
    /// Coroutine bounce the start button.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CRBounceStartButton()
    {
        float t = 0;
        float bounceTime = 0.5f;
        while (gameObject.activeSelf)
        {
            t = 0;
            while (t < bounceTime)
            {
                t += Time.deltaTime;
                float factor = t / bounceTime;
                startButtonTrans.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.1f, factor);
                yield return null;
            }


            t = 0;
            while (t < bounceTime)
            {
                t += Time.deltaTime;
                float factor = t / bounceTime;
                startButtonTrans.localScale = Vector3.Lerp(Vector3.one * 1.1f, Vector3.one, factor);
                yield return null;
            }
        }
    }






    /// <summary>
    /// ////////////////////////////////////////////// Public Functions
    /// </summary>


    public override void OnShow()
    {
        gameObject.SetActive(true);
        StartCoroutine(CRBounceStartButton());
    }

    public override void OnHide() 
    {
        startButtonTrans.localScale = Vector3.one;
        gameObject.SetActive(false);
    }


    /// <summary>
    /// ////////////////////////////////////////////// UI Functions
    /// </summary>




    public void OnClickStartButton()
    {
        SceneManager.LoadScene("GameScene");
    }
}
