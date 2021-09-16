using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Mover
{
    // experience worth
    [SerializeField] private int xpValue = 1;

    // logic
    [SerializeField] private float triggerLength = 1;
    [SerializeField] private float chaseLength = 5;

    private bool chasing;
    private bool collidingWithPlayer;

    private Transform playerTransform;

    private Vector3 startingPosition;

    // hitbox
    public ContactFilter2D filter;

    private BoxCollider2D hitbox;

    private Collider2D[] hits = new Collider2D[10];

    protected override void Start()
    {
        base.Start();
        playerTransform = GameManager.instance.player.transform;
        startingPosition = transform.position;
        hitbox = transform.GetChild(0).GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        // is the player in range
        if (Vector3.Distance(playerTransform.position, startingPosition) < chaseLength)
        {
            if (Vector3.Distance(playerTransform.position, startingPosition) < triggerLength)
            {
                chasing = true;
                GameManager.instance.player.SetInCombat(true);
            }

            if (chasing)
            {
                if (!collidingWithPlayer)
                {
                    UpdateMotor((playerTransform.position - transform.position).normalized);
                }
            }
            else
            {
                UpdateMotor(startingPosition - transform.position);
            }
        }
        else
        {
            UpdateMotor(startingPosition - transform.position);
            chasing = false;
            GameManager.instance.player.SetInCombat(false);
        }

        //check for overlap
        collidingWithPlayer = false;
        boxCollider.OverlapCollider(filter, hits);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            if (hits[i].tag == "Fighter" && hits[i].name == "Player")
            {
                collidingWithPlayer = true;
            }

            // manually clean array
            hits[i] = null;
        }
    }
    public int GetXpValue()
    {
        return xpValue;
    }
    protected override void Death()
    {
        Destroy(gameObject);
        GameManager.instance.player.SetInCombat(false);
        GameManager.instance.GrantXp(xpValue);
        GameManager.instance.ShowText("+" + xpValue + " xp", 35, Color.magenta, transform.position + new Vector3(0, 0.32f, 0), Vector3.up * 20, 2.0f);
    }
}