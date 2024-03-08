using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Sprite[] sprites = null;

    public float speed;
    private float damage;
    private float health;


    private void Start()
    {
        StartCoroutine(PlayAnimation());

        StartCoroutine(Moving());
    }


    private IEnumerator PlayAnimation()
    {
        while (true)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                spriteRenderer.sprite = sprites[i];
                yield return new WaitForSeconds(0.02f);
            }
        }
    }

    private IEnumerator Moving()
    {
        while(spriteRenderer.transform.position.y >= -1f)
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
            yield return null;
        }
    }
}
