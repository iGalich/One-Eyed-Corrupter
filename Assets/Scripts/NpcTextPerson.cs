using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcTextPerson : Collidable
{
    [SerializeField] private string message;
    [SerializeField] private float cooldown = 4f;
    private float lastMessage;

    protected override void Start()
    {
        base.Start();
        lastMessage = -cooldown;
    }
    protected override void OnCollide(Collider2D coll)
    {
        if(Time.time - lastMessage > cooldown)
        {
            lastMessage = Time.time;
            GameManager.instance.ShowText(message, 35, Color.white, transform.position + new Vector3(0, 0.16f, 0), Vector3.up * 1, cooldown);
        }
    }
}
