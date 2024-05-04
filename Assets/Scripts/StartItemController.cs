using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartItemController : MonoBehaviour
{
    [SerializeField] private GameObject starOn;
    [SerializeField] private GameObject starOff;

    private void Awake()
    {
        starOn.SetActive(true);
        starOff.SetActive(true);
    }

    public void StarOn()
    {
        starOff.SetActive(false);
    }    

    public void StarOff()
    {
        starOff.SetActive(true);
    }    
}
