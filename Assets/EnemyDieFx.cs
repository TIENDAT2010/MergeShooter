using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDieFx : MonoBehaviour
{
    [SerializeField] private Sprite[] enemyDieFx = null;
    [SerializeField] private SpriteRenderer spriteRenderer = null;

    private void Start()
    {
        StartCoroutine(PlayFx());
    }
    private IEnumerator PlayFx()
    {
        while (true)
        {
            for (int i = 0; i < enemyDieFx.Length; i++)
            {
                spriteRenderer.sprite = enemyDieFx[i];
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
            yield break;
        }
    }
}
