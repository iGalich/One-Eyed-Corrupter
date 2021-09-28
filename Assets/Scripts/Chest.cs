using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Collectable
{
    [SerializeField] private Sprite emptyChest;

    [SerializeField] private int pesosAmount = 5;

    public int PesosAmount => pesosAmount;
    protected override void OnCollect()
    {
        if (!collected)
        {
            collected = true;
            GetComponent<SpriteRenderer>().sprite = emptyChest;
            GameManager.instance.GrantPesos(pesosAmount);
            AudioManager.Instance.Play("OpenedChest");
        }
    }
}
