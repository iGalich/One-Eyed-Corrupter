using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : EnemyHitbox
{
    private Animator anim;
    private Enemy parentEnemy;
    private TrailRenderer trail;

    private float animationMultiplier = 1.75f;

    protected override void Start()
    {
        base.Start();
        parentEnemy = GetComponentInParent<Enemy>();
        trail = GetComponentInChildren<TrailRenderer>();
        anim = GetComponent<Animator>();
        anim.SetFloat("SwingMultiplier", 1f);
    }
    protected override void Update()
    {
        base.Update();

        if (parentEnemy.isChasing())
        {
            anim.SetBool("PlayerInRange", true);
        }
        else
            anim.SetBool("PlayerInRange", false);

        if (parentEnemy.hitpoint <= parentEnemy.maxHitpoint / 3)
        {
            anim.SetFloat("SwingMultiplier", animationMultiplier);
        }
    }
}
