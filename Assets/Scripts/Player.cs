using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mover
{
    [SerializeField] private int minHitPoint = 5;
    [SerializeField] private float healCooldown = 1f;

    [SerializeField] private DialogueUI dialogueUI;

    [SerializeField] private ParticleSystem levelUpParticles;

    [SerializeField] private GameObject entireHealthBar;

    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float distanceBetweenImages;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float shakeAmount = 5f;

    [SerializeField] private string receivedDamaged = "PlayerGotHit";

    private bool isAlive = true;
    private bool isDashing;
    private bool inCombat;
    private bool graceHit;
    private bool graceHitUsed;

    private float lastHeal;
    private float dashTimeLeft;
    private float lastImageXPos;
    private float lastImageYPos;
    private float lastDash = float.MinValue;


    public DialogueUI DialogueUI => dialogueUI;
    public IInteractable Interactable { get; set; }

    public void SetInCombat(bool isInCombat)
    {
        inCombat = isInCombat;
    }
    public bool GetIsDashing()
    {
        return isDashing;
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
                    lastImmune = Time.time;
                    hitpoint -= dmg.damageAmount;
                    pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;
                    canBeHit = false;
                    StartCoroutine(BecomeTemporarilyInvincible());
                    AudioManager.Instance.Play(receivedDamaged);
                    if (dmg.damageAmount > 0)
                        GameManager.instance.ShowText(dmg.damageAmount.ToString(), (int)(35 * GameManager.instance.weapon.GetDashTextMulti() * GameManager.instance.weapon.GetCritTextMulti()), Color.red, transform.position + new Vector3(0, 0.16f, 0), Vector3.up * 20, 1.5f);

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


                if (hitpoint <= 0)
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

                lastImmune = Time.time;
                pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;
                canBeHit = false;
                StartCoroutine(BecomeTemporarilyInvincible());
                AudioManager.Instance.Play(receivedDamaged);
                if (dmg.damageAmount > 0)
                    GameManager.instance.ShowText(dmg.damageAmount.ToString(), (int)(35 * GameManager.instance.weapon.GetDashTextMulti() * GameManager.instance.weapon.GetCritTextMulti()), Color.red, transform.position + new Vector3(0, 0.16f, 0), Vector3.up * 20, 1.5f);

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

            yield return new WaitForSeconds(1f);
        }
    }
    protected override void Start()
    {
        base.Start();
        GameManager.instance.OnHitpointChange();
        levelUpParticles.Pause();
    }
    protected override void Update()
    {
        base.Update();

        // checks if player is under 50% health, and heals to 50% overtime
        if (hitpoint < maxHitpoint / 2 && isAlive && !inCombat && Time.time - lastImmune > 5f)
            AutoHeal();

        // TODO remove before final build
        // currently here for bug cases
        if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
            GameManager.instance.Respawn();

        // E button is used to interact with npcs
        if (Input.GetKeyDown(KeyCode.E) && !DialogueUI.IsOpen && isAlive)
            Interactable?.Interact(this); // if interactable != null, then interact.interact(this)

        // dash button
        if (Input.GetKeyDown(KeyCode.LeftShift) && isAlive)
        {
            if (Time.time >= lastDash + dashCooldown)
                AttemptToDash();
        }
        
    }
    private void FixedUpdate()
    {
        if (dialogueUI.IsOpen) return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        if (isAlive)
        { 
            UpdateMotor(new Vector3(x, y, 0).normalized);
        }
        CheckDash();
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
        maxHitpoint++;
        hitpoint = maxHitpoint; 
    }
    public void SetLevel(int level)
    {
        for (int i = 0; i < level; i++)
            OnLevelUp();
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
    }
    private void AutoHeal()
    {
        if (Time.time - lastHeal > healCooldown)
        {
            lastHeal = Time.time;
            GameManager.instance.player.Heal(1);
            AudioManager.Instance.Play("HealthRegen");
        }
        if (graceHitUsed)
        {
            graceHitUsed = false;
            graceHit = false;
        }
    }
    public void Respawn()
    {
        GameManager.instance.weapon.SetWeaponLevel(0);
        GameManager.instance.SetXp(0);
        GameManager.instance.SetPesos(0);
        isAlive = true;
        lastImmune = Time.time;
        pushDirection = Vector3.zero;
        maxHitpoint = minHitPoint;
        hitpoint = maxHitpoint;
        GameManager.instance.OnHitpointChange();
        graceHitUsed = false;
        graceHit = false;
    }
    public int GetCurrHealth()
    {
        return hitpoint;
    }
}