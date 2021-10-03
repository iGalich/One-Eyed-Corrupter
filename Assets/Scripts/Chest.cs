using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Chest : Collectable
{
    [SerializeField] private Sprite emptyChest;

    [SerializeField] private int pesosAmount = 5;

    private Light2D light2D;

    public int PesosAmount => pesosAmount;
    protected override void Start()
    {
        base.Start();
        light2D = GetComponentInChildren<Light2D>();
    }
    protected override void OnCollect()
    {
        if (!collected)
        {
            collected = true;
            GetComponent<SpriteRenderer>().sprite = emptyChest;
            GameManager.instance.GrantPesos(pesosAmount);
            AudioManager.Instance.Play("OpenedChest");
            light2D.enabled = false;
        }
    }
}