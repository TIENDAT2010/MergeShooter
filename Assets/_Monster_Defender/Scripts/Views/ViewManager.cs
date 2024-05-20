using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    public static ViewManager Instance { get; private set; }

    [SerializeField] private HomeView homeViewPrefab;
    [SerializeField] private IngameView ingameViewPrefab;
    [SerializeField] private GameObject eventSystemPrefab;

    public HomeView HomeView { private set; get; }
    public IngameView IngameView { private set; get; }

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

                IngameView = Instantiate(ingameViewPrefab, transform, false);
                IngameView.OnHide();
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
            case ViewType.IngameView:
                IngameView.gameObject.SetActive(true);
                IngameView.OnShow();
                currentView = IngameView;
                break;
        }
    }
}
