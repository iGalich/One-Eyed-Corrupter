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

    protected override void FixedUpdate()
    {
        // is the player in range
        if (Vector3.Distance(playerTransform.position, startingPosition) < runAwayTrigger)
        {
            if (Vector3.Distance(playerTransform.position, startingPosition) < triggerLength)
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
