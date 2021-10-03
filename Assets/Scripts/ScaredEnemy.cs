using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaredEnemy : Enemy
{
    private bool isRunningAway;

    [SerializeField] private float runAwayTrigger = 1;
    [SerializeField] private float runAwayLength = 5;

    [SerializeField] private bool isGolden;

    [SerializeField] private GameObject coinParticles;

    private AudioSource coinSFX;

    private bool isAlive = true;

    protected override void Start()
    {
        base.Start();
        coinSFX = GetComponent<AudioSource>();
        if (coinParticles != null)
            coinParticles.GetComponent<ParticleSystem>().Stop();
    }
    protected override void ReceiveDamage(Damage dmg)
    { 
        if (!isGolden && hitpoint > 0)
            base.ReceiveDamage(dmg);

        if (canBeHit && isGolden && hitpoint > 0)
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

        // When hit, will run in sudden random direction, to avoid player abusing corners
        if (hitpoint > 0)
        {
            var x = new RandomFloat().NextFloat(-1f, 1f);
            var y = new RandomFloat().NextFloat(-1f, 1f);
            var direction = new Vector3(x, y, 0f);
            ApplyKnockback(direction);
        }

        if (hitpoint <= 0)
        {
            hitpoint = 0;
            Death();
        }
    }
    protected override void FixedUpdate()
    {
        var dis = Vector3.Distance(playerTransform.position, startingPosition);
        if (dis < runAwayLength)
        // is the player in range
        //if (Vector3.Distance(playerTransform.position, startingPosition) < runAwayLength)
        {
            if (Vector3.Distance(playerTransform.position, transform.position) < runAwayTrigger)
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
        if (isAlive)
        {
            if (!isGolden)
                GameManager.instance.player.Heal(Random.Range(1, GameManager.instance.GetCurrentLevel() + 1));
            else
                GameManager.instance.GrantPesos(Random.Range(10, 101));
            Instantiate(deathParticles, transform.position, transform.rotation);
            Destroy(gameObject);
            GameManager.instance.player.SetInCombat(false);
            GameManager.instance.GrantXp(xpValue);
            GameManager.instance.ShowText("+" + xpValue + " xp", 35, Color.magenta, transform.position + new Vector3(0, 0.32f, 0), Vector3.up * 20, 2.0f);
            isAlive = false;
        }
    }
}
