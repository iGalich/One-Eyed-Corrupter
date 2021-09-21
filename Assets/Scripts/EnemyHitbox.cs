using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to indicate the area where an enemy is able to hit the player.
/// </summary>
public class EnemyHitbox : Collidable
{
    // damage
    [SerializeField] protected int damage = 1;

    [SerializeField] protected float pushForce = 5f;

    [SerializeField] protected float PushForce => pushForce;
    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter" && coll.name == "Player")
        {
            // create new damage object before sending it to the player
            Damage dmg = new Damage()
            {
                damageAmount = damage,
                origin = transform.position,
                pushForce = pushForce
            };

            coll.SendMessage("ReceiveDamage", dmg);
        }
    }
    public float GetPushForce()
    {
        return pushForce;
    }
}
