using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    public static ViewManager Instance { get; private set; }

    [SerializeField] private HomeView homeViewPrefab;
    [SerializeField] private GameView gameViewPrefab;
    [SerializeField] private GameObject eventSystemPrefab;

    public HomeView HomeView { private set; get; }
    public GameView GameView { private set; get; }

    private BaseView currentView = null;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else 
        {
            Instance = this;
            GameObject eventSystem = Instantiate(eventSystemPrefab);
            DontDestroyOnLoad(eventSystem);
            DontDestroyOnLoad(gameObject);
            if (transform.childCount == 0)
            {
                HomeView = Instantiate(homeViewPrefab, transform, false);
                HomeView.OnShow();

                GameView = Instantiate(gameViewPrefab, transform, false);
                GameView.OnHide();
            }
        }
    }


    public void SetActiveView(ViewType viewType)
    {
        if (currentView != null)
            currentView.OnHide();
        switch(viewType)
        {
            case ViewType.HomeView:
                HomeView.gameObject.SetActive(true);
                HomeView.OnShow();
                currentView = HomeView;
                break;
            case ViewType.GameView:
                GameView.gameObject.SetActive(true);
                GameView.OnShow();
                currentView = GameView;
                break;
        }
    }
}
