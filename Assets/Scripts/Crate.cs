using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : Fighter
{
    [SerializeField] private int minAmount = 1, maxAmount = 5;

    protected override void Start()
    {
        if (rb == null) return;
    }
    protected override void ReceiveDamage(Damage dmg)
    {
        if (canBeHit)
        {
            lastImmune = Time.time;
            hitpoint -= dmg.damageAmount;
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;
            GameManager.instance.ShowText(dmg.damageAmount.ToString(), (int)(35 * GameManager.instance.weapon.GetDashTextMulti() * GameManager.instance.weapon.GetCritTextMulti()), Color.red, transform.position + new Vector3(0, 0.16f, 0), new Vector3(0, 0.16f, 0), 0.5f);
            GameManager.instance.weapon.SetCritTextMulti();
            GameManager.instance.weapon.SetDashTextMulti();
        }


        if (hitpoint <= 0)
        {
            hitpoint = 0;
            Death();
        }
    }
    protected override void Death()
    {
        Destroy(gameObject);
        if (Random.value > 0.5f) // 50\50 coin flip if gets money or not
            GameManager.instance.GrantPesos(Random.Range(minAmount, maxAmount + 1));
    }
}
