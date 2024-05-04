using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankInforController : MonoBehaviour
{
    [SerializeField] private List<StartItemController> listStarItem = null;

    [SerializeField] private TankType tankType;


    public void OnInit()
    {
        int tankLevel = PlayerPrefsKey.GetTankLevel(tankType);
        for (int i = 0; i < listStarItem.Count; i++)
        {
            if (i <= (tankLevel - 1))
            {
                listStarItem[i].StarOn();
            }
        }
    }


    public void OnClickUpgradeButton()
    {
        int currentLevel = PlayerPrefsKey.GetTankLevel(tankType);

        int upgradedLevel = currentLevel + 1;

        PlayerPrefsKey.SaveTankLevel(tankType, upgradedLevel);

        for (int i = 0; i < listStarItem.Count; i++)
        {
            if (i <= (upgradedLevel - 1))
            {
                listStarItem[i].StarOn();
            }
        }

    }
}
