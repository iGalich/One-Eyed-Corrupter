using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballBoss : Enemy
{
    [SerializeField] private float[] fireballSpeed = { 2.5f, -2.5f };

    [SerializeField] private const float distance = 0.25f;
    [SerializeField] private const float modifier = 1.5f;
    [SerializeField] private const float missleCooldown = 3f;

    [SerializeField] private Transform[] fireballs;
    [SerializeField] private GameObject homingMisslePrefab;

    private int fireballCount;

    private float lastMissle;

    private bool activeMissleExists = false;

    protected override void Start()
    {
        base.Start();
        fireballCount = fireballs.Length;
    }
    private void Update()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (fireballs[i] == null)  
                continue;

            fireballs[i].position = transform.position + new Vector3(-Mathf.Cos(Time.time * fireballSpeed[i] * modifier + i) * distance,
                                                                    Mathf.Sin(Time.time * fireballSpeed[i] / modifier) * distance,
                                                                    0);
        }
        if (!activeMissleExists && fireballCount == 0 && Time.time - lastMissle > missleCooldown)
        {
            FireMissle();
        }
    }
    public void DecreaseFireballCount()
    {
        fireballCount--;
        if (fireballCount == 0) 
            lastMissle = Time.time; //instead of creating another variable for last fireball death, to give player time before missle start firing
    }
    public void FireMissle()
    {
        //SetActiveMissleExists(true);
        Instantiate(homingMisslePrefab, transform.position, transform.rotation);
    }
    public void SetActiveMissleExists(bool exists)
    {
        activeMissleExists = exists;
    }
    public void SetLastMissleTime()
    {
        lastMissle = Time.time;
    }
    public void IncreaseFireballSpeed(float addSpeed)
    {
        for (int i = 0; i < fireballSpeed.Length; i++)
        {
            if (fireballSpeed[i] > 0)
                fireballSpeed[i] += addSpeed;
            else
                fireballSpeed[i] -= addSpeed;
        }
    }
    private int CalculateExperinceWorth()
    {
        int totalXpWorth = GetXpValue();
        {
            for (int i = 0; i < fireballs.Length; i++)
            {
                if (fireballs[i] != null)
                    totalXpWorth += GetComponentInChildren<FireballEnemy>().GetXpValue();
            }
        }
        return totalXpWorth;
    }
    protected override void Death()
    {
        Destroy(gameObject);
        int xp = CalculateExperinceWorth();
        GameManager.instance.GrantXp(xp);
        GameManager.instance.ShowText("+" + xp + " xp", 35, Color.magenta, transform.position, Vector3.up * 40, 1.0f);
    }
}
