using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEffectController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Sprite[] tankShootSprites = null;



    /// <summary>
    /// Play the shoot effect.
    /// </summary>
    public void PlayShootEffect()
    {
        spriteRenderer.enabled = true;
        StartCoroutine(ShootEffect());
    }


    /// <summary>
    /// Coroutine change the sprite to create shoot effect.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShootEffect()
    {
        for (int i = 0; i < tankShootSprites.Length; i++)
        {
            spriteRenderer.sprite = tankShootSprites[i];
            yield return null;
        }
        spriteRenderer.enabled = false;
    }
}
