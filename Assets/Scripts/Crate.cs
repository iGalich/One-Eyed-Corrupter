using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : Fighter
{
    protected override void Death()
    {
        Destroy(gameObject);
        int pesosAmount = Random.Range(0, 6);
        if (pesosAmount != 0)
            GameManager.instance.GrantPesos(pesosAmount);
    }
}
