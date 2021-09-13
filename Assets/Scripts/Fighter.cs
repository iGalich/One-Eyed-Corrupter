using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public int hitpoint = 10;
    public int maxHitpoint = 10;

    [SerializeField] private float pushRecoverySpeed = 0.2f; // how long it takes to recover from knockback
    [SerializeField] protected float immuneTime = 1.0f;
    protected float lastImmune;

    protected Vector3 pushDirection;

    protected Rigidbody2D rb;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    // all fighers can receive damage and die
    protected virtual void ReceiveDamage(Damage dmg)
    {
        if (Time.time - lastImmune > immuneTime)
        {
            lastImmune = Time.time;
            hitpoint -= dmg.damageAmount;
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;
            GameManager.instance.ShowText(dmg.damageAmount.ToString(), 35, Color.red, transform.position, Vector3.zero, 0.5f);
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