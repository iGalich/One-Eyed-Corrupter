using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public int hitpoint = 10;
    public int maxHitpoint = 10;

    [SerializeField] private float pushRecoverySpeed = 0.2f; // how long it takes to recover from knockback
    [SerializeField] protected float immuneTime = 1.0f;
    [SerializeField] protected float invincibilityDeltaTime = 0.15f;

    [SerializeField] protected bool isDummy;

    [SerializeField] protected string hitSFX;

    protected float lastImmune;

    protected Vector3 pushDirection;
    //protected Vector3 defaultScale;

    protected Rigidbody2D rb;

    protected SpriteRenderer spriteRenderer;

    protected bool canBeHit = true;

    private WaitForSecondsRealtime invinicilityTick;

    protected Vector3 startingPosition;

    protected virtual void Start()
    {
        invinicilityTick = new WaitForSecondsRealtime(invincibilityDeltaTime);
        //defaultScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        startingPosition = transform.position;
    }
    protected virtual void Update()
    {
        if (GameManager.instance.weapon.GetDashTextMulti() != 1)
            GameManager.instance.weapon.SetDashTextMulti();
        if (GameManager.instance.weapon.GetCritTextMulti() != 1)
            GameManager.instance.weapon.SetCritTextMulti();

        if (Time.time - lastImmune > immuneTime)
            canBeHit = true;
        else
            canBeHit = false;

        if (hitpoint < maxHitpoint && isDummy)
            hitpoint = maxHitpoint;
    }
    // all fighers can receive damage and die
    protected virtual void ReceiveDamage(Damage dmg)
    {
        if (canBeHit)
        {
            lastImmune = Time.time;
            hitpoint -= dmg.damageAmount;
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;
            canBeHit = false;
            StartCoroutine(BecomeTemporarilyInvincible());
            AudioManager.Instance.Play(GameManager.instance.weapon.GetHitLandOnEnemySFX());
            if (dmg.damageAmount > 0)
                GameManager.instance.ShowText(dmg.damageAmount.ToString(), (int)(35 * GameManager.instance.weapon.GetDashTextMulti() * GameManager.instance.weapon.GetCritTextMulti()), Color.red, transform.position + new Vector3(0, 0.16f, 0), Vector3.up * 20, 1.5f);

            if (this.CompareTag("Fighter"))
            {
                GameManager.instance.weapon.TimeStop();
            }

            if (this.name == "Player")
            {
                CinemachineShake.Insatnce.ShakeCamera(CinemachineShake.Insatnce.GetCameraShakeIntensity(), 0.1f);
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
    protected virtual void Death()
    {
        Debug.Log("Death was not implemented in " + this.name);
    }
    protected virtual IEnumerator BecomeTemporarilyInvincible()
    {
        while (!canBeHit)
        {
            //alternating between 0 and normal scale scale to simulate flashing
            if (spriteRenderer.enabled == true)
                spriteRenderer.enabled = false;
            else
                spriteRenderer.enabled = true;

            yield return invinicilityTick;
        }
        spriteRenderer.enabled = true;
    }
    public float GetPushRecoverySpeed()
    {
        return pushRecoverySpeed;
    }
    public void ApplyKnockback(Vector3 pushDirection)
    {
        rb.isKinematic = false;
        rb.AddForce(pushDirection, ForceMode2D.Impulse);
        StartCoroutine(KnockbackCo(rb));
    }
    private IEnumerator KnockbackCo(Rigidbody2D rb)
    {
        if (rb != null)
        {
            yield return new WaitForSeconds(GetPushRecoverySpeed());
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
    }
}