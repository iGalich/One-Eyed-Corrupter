using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaredEnemy : Enemy
{
    private bool isRunningAway;

    [SerializeField] private float runAwayTrigger = 1;
    [SerializeField] private float runAwayLength = 5;

    [SerializeField] private int healOnDeath = 2;

    [SerializeField] private bool isGolden;

    [SerializeField] private GameObject coinParticles;

    private AudioSource coinSFX;

    protected override void Start()
    {
        base.Start();
        coinSFX = GetComponent<AudioSource>();
        coinParticles.GetComponent<ParticleSystem>().Stop();
    }
    protected override void ReceiveDamage(Damage dmg)
    {
        if (!isGolden)
            base.ReceiveDamage(dmg);

        if (canBeHit && isGolden)
        {
            lastImmune = Time.time;
            hitpoint -= dmg.damageAmount;
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;
            canBeHit = false;
            StartCoroutine(BecomeTemporarilyInvincible());
            coinSFX.Play();
            if (hitpoint > 0)
            {
                coinParticles.GetComponent<ParticleSystem>().Play();
                GameManager.instance.GrantPesos(Random.Range(1, dmg.damageAmount + 1));
            }

            if (dmg.damageAmount > 0)
                GameManager.instance.ShowText(dmg.damageAmount.ToString(), (int)(35 * GameManager.instance.weapon.GetDashTextMulti() * GameManager.instance.weapon.GetCritTextMulti()), Color.red, transform.position + new Vector3(0, 0.16f, 0), Vector3.up * 20, 1.5f);

            if (this.CompareTag("Fighter"))
            {
                GameManager.instance.weapon.TimeStop();
            }

            if (rb != null)
            {
                ApplyKnockback(pushDirection);
            }
        }

        if (hitpoint <= 0)
        {
            hitpoint = 0;
            Death();
        }
    }
    protected override void FixedUpdate()
    {
        // is the player in range
        if (Vector3.Distance(playerTransform.position, startingPosition) < runAwayLength)
        {
            if (Vector3.Distance(playerTransform.position, startingPosition) < runAwayTrigger)
            {
                isRunningAway = true;
                GameManager.instance.player.SetInCombat(true);
            }

            if (isRunningAway)
            {
                if (!collidingWithPlayer)
                {
                    UpdateMotor(-(playerTransform.position - transform.position).normalized);
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
            isRunningAway = false;
            GameManager.instance.player.SetInCombat(false);
        }
    }
    protected override void Death()
    {
        if (!isGolden)
            GameManager.instance.player.Heal(healOnDeath);
        else
            GameManager.instance.GrantPesos(Random.Range(10, 101));
        base.Death();
    }
}