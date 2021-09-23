using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireballBoss : Enemy
{
    [SerializeField] private float[] fireballSpeed = { 2.5f, -2.5f };
    
    //[SerializeField] private Transform[] fireballs;

    [SerializeField] private float distance = 0.25f;
    [SerializeField] private float modifier = 1.5f;
    [SerializeField] private float missleCooldown = 3f;
    [SerializeField] private float speedMultiplierOnLowHealth = 2f;

    [SerializeField] private GameObject homingMisslePrefab;

    public List<FireballEnemy> bossFireBalls;

    private RectTransform hpBarFront;
    private RectTransform hpBarBack;

    private int fireballCount;

    private float healthRatio;
    private float lastMissle;
    private float deathTime;
    private float deathParticlesCooldown = 1.5f;

    private bool activeMissleExists = false;
    private bool speedHasIncreased = false;
    private bool isDead = false;

    protected override void Start()
    {
        base.Start();
        hpBarFront = GameObject.Find("HUD/BossHealthBarCanvas/BossHealthBarUnder/Health").GetComponent<RectTransform>();
        hpBarBack = GameObject.Find("HUD/BossHealthBarCanvas/BossHealthBarUnder/DecayingHealthBar").GetComponent<RectTransform>();
        //fireballCount = fireballs.Length;

        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<FireballEnemy>() != null)
                bossFireBalls.Add(child.gameObject.GetComponent<FireballEnemy>());
        }

        fireballCount = bossFireBalls.Count;
    }
    protected override void Update()
    {
        base.Update();

        SyncBar();

        for (int i = 0; i < bossFireBalls.Count; i++)
        {
            if (bossFireBalls[i] == null)  
                continue;

            bossFireBalls[i].transform.position = transform.position + new Vector3(-Mathf.Cos(Time.time * fireballSpeed[i] * modifier + i) * distance,
                                                                    Mathf.Sin(Time.time * fireballSpeed[i] / modifier) * distance,
                                                                    0);
        }
        if (!activeMissleExists && fireballCount == 0 && Time.time - lastMissle > missleCooldown)
        {
            FireMissle();
        }
        if (!speedHasIncreased && hitpoint < maxHitpoint / 4)
        {
            speedHasIncreased = true;
            IncreaseMoveSpeed();
        }
        if (isDead && Time.time - deathTime > deathParticlesCooldown)
        {
            deathParticles.GetComponent<ParticleSystem>().Stop();
            foreach (FireballEnemy fireball in bossFireBalls)
            {
                if (fireball != null)
                    fireball.SendMessage("Destroy");
                else
                    continue;
            }
        }
    }
    private void SyncBar()
    {
        if (hpBarBack.localScale.x > hpBarFront.localScale.x)
        {
            hpBarBack.localScale = new Vector3(Mathf.Lerp(hpBarBack.localScale.x, hpBarFront.localScale.x, Time.unscaledDeltaTime), hpBarBack.localScale.y, hpBarBack.localScale.z);
        }
    }
    private void HitPointChange() 
    {
        healthRatio = (float)hitpoint / (float)maxHitpoint;
        hpBarFront.localScale = new Vector3(healthRatio, 1, 1);
    }
    protected override void ReceiveDamage(Damage dmg)
    {
        base.ReceiveDamage(dmg);
        HitPointChange();
    }
    private void IncreaseMoveSpeed()
    {
        moveSpeed *= speedMultiplierOnLowHealth;
    }
    public void DecreaseFireballCount()
    {
        fireballCount--;
        if (fireballCount == 0) 
            lastMissle = Time.time; //instead of creating another variable for last fireball death, to give player time before missle start firing
    }
    public void FireMissle()
    {
        SetActiveMissleExists(true);
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
            for (int i = 0; i < bossFireBalls.Count; i++)
            {
                if (bossFireBalls[i] != null)
                    totalXpWorth += GetComponentInChildren<FireballEnemy>().GetXpValue();
            }
        }
        return totalXpWorth;
    }
    protected override void Death()
    {
        deathParticles.GetComponent<ParticleSystem>().Play();
        deathTime = Time.time;
        gameObject.transform.position += Vector3.up * 10000;
        moveSpeed = 0;
        isDead = true;
        int xp = CalculateExperinceWorth();
        GameManager.instance.GrantXp(xp);
        GameManager.instance.ShowText("+" + xp + " xp", 35, Color.magenta, transform.position + new Vector3(0, 0.32f, 0), Vector3.up * 20, 2.0f);
    }
    public bool CheckIsDead()
    {
        return isDead;
    }
}
