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
    protected override void Death()
    {
        Destroy(gameObject);
        if (Random.value > 0.5f) // 50\50 coin flip if gets money or not
            GameManager.instance.GrantPesos(Random.Range(minAmount, maxAmount + 1));
    }
}
