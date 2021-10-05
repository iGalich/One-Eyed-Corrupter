using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScreamSource : MonoBehaviour
{
    private float lastScream = float.MaxValue;
    private int screamCooldown = 10;
    private int minScreamCooldown = 15;
    private int maxScreamCooldown = 45;

    public float LastScream { set => lastScream = value; }
    private void Start()
    {
        if (GameObject.FindObjectOfType<FireballBoss>() == null)
            lastScream = Time.time;
    }
    private void Update()
    {
        if (Time.time - lastScream > screamCooldown)
            Scream();
    }
    private void Scream()
    {
        lastScream = Time.time;
        AudioManager.Instance.Play("BossScream", transform.position);
        NewRandomScreamCooldown();
    }
    private void NewRandomScreamCooldown()
    {
        screamCooldown = Random.Range(minScreamCooldown, maxScreamCooldown + 1);
    }
}
