using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankSpawnController : MonoBehaviour
{
    private TankController tankController;
    public TankController TankController
    {
        get { return tankController; }

        set { tankController = value; }
    }
}
