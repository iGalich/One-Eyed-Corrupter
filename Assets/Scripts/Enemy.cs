using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Mover
{
    // experience worth
    [SerializeField] private int xpValue = 1;

    //gold worth
    [SerializeField] private int pesosValue;

    // logic
    [SerializeField] protected float triggerLength = 1;
    [SerializeField] private float chaseLength = 5;

    [SerializeField] protected GameObject deathParticles;

    private bool chasing;
    protected bool collidingWithPlayer;

    private float lastHeal;
    private float healCooldown = 2f;

    protected Transform playerTransform;

    // hitbox
    public ContactFilter2D filter;

    private BoxCollider2D hitbox;

    private Collider2D[] hits = new Collider2D[10];

    protected override void Start()
    {
        base.Start();
        deathParticles.GetComponent<ParticleSystem>().Stop();
        playerTransform = GameManager.instance.player.transform;
        hitbox = transform.GetChild(0).GetComponent<BoxCollider2D>(); // potetial error place don't forget
    }
    private void ReturnToPlace()
    {
        UpdateMotor(startingPosition - transform.position);
    }
    protected virtual void FixedUpdate()
    {
        if (isDummy && startingPosition != transform.position)
            ReturnToPlace();
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
    protected override void Update()
    {
        base.Update();
        if (hitpoint < maxHitpoint && Time.time - lastImmune > 5f && !chasing)
            AutoHeal();
    }
    private void AutoHeal()
    {
        if (hitpoint < (int)(maxHitpoint * 0.75f) && Time.time - lastHeal > healCooldown)
        {
            lastHeal = Time.time;
            hitpoint++;
        }
    }
    public int GetXpValue()
    {
        return xpValue;
    }
    protected override void Death()
    {
        Instantiate(deathParticles, transform.position, transform.rotation);
        Destroy(gameObject);
        GameManager.instance.player.SetInCombat(false);
        GameManager.instance.GrantXp(xpValue);
        if (pesosValue > 0)
            GameManager.instance.GrantPesos(pesosValue);
        GameManager.instance.ShowText("+" + xpValue + " xp", 35, Color.magenta, transform.position + new Vector3(0, 0.32f, 0), Vector3.up * 20, 2.0f);
    }
}