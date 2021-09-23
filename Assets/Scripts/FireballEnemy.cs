using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballEnemy : Enemy
{
    [SerializeField] private FireballBoss fireballBoss;
    [SerializeField] private float addSpeed = 1f;

    protected override void Start()
    {
        base.Start();
        fireballBoss = this.GetComponentInParent<FireballBoss>();
    }
    protected override void Death()
    {
        base.Death();
        fireballBoss.DecreaseFireballCount();
        fireballBoss.IncreaseFireballSpeed(addSpeed);
    }
    private void Destroy()
    {
        Destroy(this.gameObject);
    }
}
