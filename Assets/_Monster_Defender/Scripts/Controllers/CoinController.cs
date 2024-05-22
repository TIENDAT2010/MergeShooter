using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{


    /// <summary>
    /// Move this coin object to the target pos and add coin to CoinManager.
    /// </summary>
    /// <param name="targetPos"></param>
    public void MoveToPosAndUpdateCoin(Vector2 targetPos)
    {
        StartCoroutine(CRMoveToPos(targetPos));
    }



    /// <summary>
    /// Coroutine move this coin object to the target pos and add coin to CoinManager.
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    private IEnumerator CRMoveToPos(Vector2 targetPos)
    {
        float t = 0;
        float moveTime = 1f;
        Vector3 startVector3 = transform.position;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            Vector3 newPos = Vector3.Lerp(startVector3, targetPos, factor);
            transform.position = newPos;
            yield return null;
        }

        CoinManager.Instance.AddCoins(1, 0f);
        gameObject.SetActive(false);
    }
}
