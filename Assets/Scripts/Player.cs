using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mover
{
    [SerializeField] private int minHitPoint = 5;
    [SerializeField] private float healCooldown = 1f;

    [SerializeField] private DialogueUI dialogueUI;

    [SerializeField] private ParticleSystem levelUpParticles;

    [SerializeField] private GameObject bloodParticles;

    [SerializeField] private GameObject entireHealthBar;

    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float distanceBetweenImages;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float shakeAmount = 5f;

    [SerializeField] private string receivedDamaged = "PlayerGotHit";

    [SerializeField] private bool autoHealOn;

    ParticleSystem.MainModule main;

    private bool isAlive = true;
    private bool isDashing;
    private bool inCombat;
    private bool graceHit;
    private bool graceHitUsed;
    private bool healSfxPlayed;
    private bool canMove = true;

    private float lastHeal;
    private float dashTimeLeft;
    private float lastImageXPos;
    private float lastImageYPos;
    private float lastDash = float.MinValue;

    private int bloodDropAmount = 100;
    private int maxHitpointIncreasePerLevel = 3;
    private int healthRestoreOnLevelUp = 5;

    public bool CanMove { get => canMove;  set => canMove = value; }
    public bool IsAlive => isAlive;
    public int HealthRestoreOnLevelUp => healthRestoreOnLevelUp;
    public int MaxHitPointIncreasePerLevel => maxHitpointIncreasePerLevel;
    public DialogueUI DialogueUI => dialogueUI;
    public IInteractable Interactable { get; set; }
    public bool InCombat => inCombat;
    public bool IsInCombat() { return inCombat; }
    protected override void Start()
    {
        base.Start();
        GameManager.instance.OnHitpointChange();
        levelUpParticles.Pause();
        bloodParticles.GetComponent<ParticleSystem>().Stop();
        main = bloodParticles.GetComponent<ParticleSystem>().main;
        main.maxParticles = bloodDropAmount;
    }
    protected override void Update()
    {
        base.Update();
        // checks if player is under 50% health, and heals to 50% overtime
        if (autoHealOn)
        {
            if (hitpoint < maxHitpoint / 2 && isAlive && !inCombat && Time.time - lastImmune > 5f)
                AutoHeal();
            else if (healSfxPlayed)
                healSfxPlayed = false;
        }

        // E button is used to interact with npcs
        if (Input.GetKeyDown(KeyCode.E) && !DialogueUI.IsOpen && isAlive && !inCombat)
            Interactable?.Interact(this); // if interactable != null, then interact.interact(this)

        // dash button
        if (Input.GetKeyDown(KeyCode.LeftShift) && isAlive && canMove)
        {
            if (Time.time >= lastDash + dashCooldown)
                AttemptToDash();
        }

        if (!bloodParticles.GetComponent<ParticleSystem>().isPlaying)
            main.maxParticles = 100;
    }
    public void CorrectMaxHitPoint(int levelGap)
    {
        maxHitpoint += maxHitpointIncreasePerLevel * (levelGap - 2);
        if (GameManager.instance.GetCurrentLevel() == 10)
            maxHitpoint += 3;
        if (hitpoint > maxHitpoint)
            hitpoint = maxHitpoint;
    }
    private void FixedUpdate()
    {
        if (dialogueUI.IsOpen) return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        if (isAlive && canMove)
        { 
            UpdateMotor(new Vector3(x, y, 0).normalized);
        }
        CheckDash();
    }
    protected override void Death()
    {
        AudioManager.Instance.Mute(AudioManager.Instance.GetCurrentlyPlaying());
        AudioManager.Instance.Play("PlayerDeath");
        isAlive = false;
        GameManager.instance.deathMenuAnim.SetTrigger("Show");
    }
    protected override void ReceiveDamage(Damage dmg)
    {
        if (isAlive)
        {
            if (!graceHit || (graceHit && graceHitUsed))
            {
                if (canBeHit)
                {
                    hitpoint -= dmg.damageAmount;
                    pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;
                    AudioManager.Instance.Play(receivedDamaged);
                    lastImmune = Time.time;
                    StartCoroutine(BecomeTemporarilyInvincible());
                    if (dmg.damageAmount > 0)
                    {
                        canBeHit = false;
                        main.maxParticles *= dmg.damageAmount;
                        bloodParticles.GetComponent<ParticleSystem>().Play();
                        GameManager.instance.ShowText(dmg.damageAmount.ToString(), (int)(35 * GameManager.instance.weapon.GetDashTextMulti() * GameManager.instance.weapon.GetCritTextMulti()), Color.red, transform.position + new Vector3(0, 0.16f, 0), Vector3.up * 20, 1.5f);
                    }

                    if (this.CompareTag("Fighter"))
                    {
                        GameManager.instance.weapon.TimeStop();
                    }

                    if (this.name == "Player")
                    {
                        CinemachineShake.Insatnce.ShakeCamera(CinemachineShake.Insatnce.GetCameraShakeIntensity(), 0.1f);
                    }
                    if (rb != null)
                    {
                        ApplyKnockback(pushDirection);
                    }
                }

                if (!graceHit && !graceHitUsed && hitpoint <= 0)
                {
                    graceHit = true;
                    hitpoint = 1;
                }
                if (graceHit && !graceHitUsed)
                {
                    graceHitUsed = true;
                    hitpoint = 1;
                }
                if (hitpoint <= 0 && graceHit && graceHitUsed)
                {
                    hitpoint = 0;
                    Death();
                }
                GameManager.instance.OnHitpointChange();
                if (hitpoint == 1)
                    graceHit = true;
            }
            else if (canBeHit && !graceHitUsed && graceHit)
            {

                pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;
                AudioManager.Instance.Play(receivedDamaged);
                lastImmune = Time.time;
                StartCoroutine(BecomeTemporarilyInvincible());
                if (dmg.damageAmount > 0)
                {
                    canBeHit = false;
                    main.maxParticles *= dmg.damageAmount;
                    bloodParticles.GetComponent<ParticleSystem>().Play();
                    GameManager.instance.ShowText(dmg.damageAmount.ToString(), (int)(35 * GameManager.instance.weapon.GetDashTextMulti() * GameManager.instance.weapon.GetCritTextMulti()), Color.red, transform.position + new Vector3(0, 0.16f, 0), Vector3.up * 20, 1.5f);
                }

                if (this.CompareTag("Fighter"))
                {
                    GameManager.instance.weapon.TimeStop();
                }

                if (this.name == "Player")
                {
                    CinemachineShake.Insatnce.ShakeCamera(CinemachineShake.Insatnce.GetCameraShakeIntensity(), 0.1f);
                }
                if (rb != null)
                {
                    ApplyKnockback(pushDirection);
                }

                graceHitUsed = true;
            }
        }
        if (hitpoint == 1)
        {
            StartCoroutine(ShakeBar());
        }

    }
    private IEnumerator ShakeBar()
    {
        while (hitpoint == 1)
        {
            iTween.ShakePosition(entireHealthBar, Vector3.one * shakeAmount, 0.5f);

            yield return new WaitForSeconds(1.5f);
        }
    }
    private void AttemptToDash()
    {
        if (Stamina.Instance.CheckStamina())
        {
            isDashing = true;
            dashTimeLeft = dashTime;
            lastDash = Time.time;

            PlayerAfterImagePool.Instance.GetFromPool();
            lastImageXPos = transform.position.x;
            lastImageYPos = transform.position.y;

            AudioManager.Instance.Play("Dash");
        }
        else
        {
            Stamina.Instance.ShakeBar();
        }
    }
    private void CheckDash()
    {
        if (isDashing)
        {
            Stamina.Instance.OnStaminaUse();

            
            if (dashTimeLeft > 0)
            {
                rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * dashSpeed;

                dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXPos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXPos = transform.position.x;
                }
                if (Mathf.Abs(transform.position.y - lastImageYPos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageYPos = transform.position.y;
                }
            }
            if (dashTimeLeft <= 0)
            {
                isDashing = false;
                rb.velocity = new Vector2(0f, 0f);
            }
        }
    }
    public void SwapSprite(int skinId)
    {
        spriteRenderer.sprite = GameManager.instance.playerSprites[skinId];
    }
    public Sprite GetPlayerSprite()
    {
        return spriteRenderer.sprite;
    }
    public void OnLevelUp()
    {
        levelUpParticles.Play();
        AudioManager.Instance.Play("LevelUp");
        if (GameManager.instance.GetCurrentLevel() == 10)
            maxHitpoint += 3;
        maxHitpoint += maxHitpointIncreasePerLevel;
        hitpoint += healthRestoreOnLevelUp;
        if (hitpoint > maxHitpoint)
            hitpoint = maxHitpoint;
    }
    public void SetLevel(int level)
    {
        //for (int i = 0; i < level; i++)
        //    OnLevelUp();
    }
    public void Heal(int healingAmount)
    {
        if (hitpoint == maxHitpoint)
            return;
        hitpoint += healingAmount;
        if (hitpoint > maxHitpoint)
            hitpoint = maxHitpoint;
        GameManager.instance.ShowText("+" + healingAmount.ToString() + " hp", 30, Color.green, transform.position + new Vector3(0, 0.16f, 0), Vector3.up * 30, 1.0f);
        GameManager.instance.OnHitpointChange();
        graceHitUsed = false;
        graceHit = false;
    }
    private void AutoHeal()
    {
        if (Time.time - lastHeal > healCooldown)
        {
            lastHeal = Time.time;
            GameManager.instance.player.Heal(1);
            if (!healSfxPlayed)
            {
                AudioManager.Instance.Play("HealthRegen");
                healSfxPlayed = true;
            }
            else
                healSfxPlayed = false;
        }
        if (graceHitUsed || graceHit)
        {
            graceHitUsed = false;
            graceHit = false;
        }
    }
    public void Respawn()
    {
        isAlive = true;
        lastImmune = Time.time;
        pushDirection = Vector3.zero;
        ResetStats();
    }
    public void ResetStats()
    {
        GameManager.instance.weapon.SetWeaponLevel(0);
        GameManager.instance.SetXp(0);
        GameManager.instance.SetPesos(0);
        maxHitpoint = minHitPoint;
        hitpoint = maxHitpoint;
        GameManager.instance.OnHitpointChange();
        graceHitUsed = false;
        graceHit = false;
        GameManager.instance.weapon.SetDamage(1);
        GameManager.instance.weapon.damagePoint[6] = 7;
    }
    public int GetCurrHealth()
    {
        return hitpoint;
    }
    public void SetInCombat(bool isInCombat)
    {
        inCombat = isInCombat;
    }
    public bool GetIsDashing()
    {
        return isDashing;
    }
}