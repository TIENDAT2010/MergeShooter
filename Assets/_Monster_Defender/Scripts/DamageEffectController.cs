using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageEffectController : MonoBehaviour
{
    [SerializeField] private Text damageEffectText = null;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    private void Start()
    {
        StartCoroutine(MoveDamageEffect());
    }

    private IEnumerator MoveDamageEffect()
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

    public void SetDamageEffcet(float damage)
    {
        damageEffectText.text = "-" + damage.ToString();
    }
}
