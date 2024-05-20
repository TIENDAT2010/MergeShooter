using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinEffectController : MonoBehaviour
{
    [SerializeField] private Text m_CoinText;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    private void Start()
    {
        StartCoroutine(MoveCoin());
    }

    private IEnumerator MoveCoin()
    {
        float t = 0;
        float moveTime = 1f;
        Vector3 startVector3 = transform.position;
        Vector3 endVector3 = transform.position + Vector3.up * Time.deltaTime * 100;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            Vector3 newPos = Vector3.Lerp(startVector3, endVector3, factor);
            m_CanvasGroup.alpha = Mathf.Lerp(1, 0, factor);
            transform.position = newPos;
            yield return null;
        }
        Destroy(gameObject);
    }

    public void SetCoinBonus(float coin)
    {
        m_CoinText.text = coin.ToString();
    }
}
