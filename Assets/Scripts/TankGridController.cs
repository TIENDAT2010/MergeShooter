using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TankGridController : MonoBehaviour
{
    [SerializeField] private Transform[] tankSpawns = null;

    private Dictionary<Transform, GameObject> dicTankSpawn = new Dictionary<Transform, GameObject>(); 
    private void Awake()
    {
        for (int i = 0; i < tankSpawns.Length; i++)
        {
            dicTankSpawn.Add(tankSpawns[i], null);
        }
    }

    public Transform GetCloseTankSpawn(Transform tankPos)
    {
        float minDistance = 9999;
        Transform cur = null;
        for (int i = 0; i < tankSpawns.Length; i++)
        {
            Debug.Log("aaaa");
            float distance = Vector3.Distance(tankPos.position, tankSpawns[i].position);
            if ((distance < minDistance))
            {
                minDistance = distance;
                cur = tankSpawns[i];
            }
        }
        return cur;
    }


    //public bool IsContainTank(Transform )
}
