using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImageSprite : MonoBehaviour
{
    [SerializeField] private float activeTime = 0.1f;
    [SerializeField] private float alphaSet = 0.8f;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer playerSpriteRenderer;
    private Color color;
    private float timeActivated;
    private float alpha;
    private float alphaMultiplier = 0.85f;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.Find("Player").transform;
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        spriteRenderer.sprite = playerSpriteRenderer.sprite;
        transform.position = player.position;
        transform.rotation = player.rotation;
        timeActivated = Time.time;
    }
    private void Update()
    {
        alpha *= alphaMultiplier;
        color = new Color(1f, 1f, 1f, alpha);
        spriteRenderer.color = color;

        if (Time.time >= timeActivated + activeTime)
        {
            PlayerAfterImagePool.Instance.AddToPool(gameObject);
        }
    }
}
