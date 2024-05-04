using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankViewController : MonoBehaviour
{
    [SerializeField] private List<TankInforController> listTankInforController = null;


    public void OnInit()
    {
        foreach(TankInforController tankInfoController in listTankInforController)
        {
            tankInfoController.OnInit();
        }
    }
}
