//using System.Threading.Tasks.Dataflow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mover
{
    [SerializeField] private int minHitPoint = 5;
    [SerializeField] private float healCooldown = 1f;

    [SerializeField] private DialogueUI dialogueUI;

    [SerializeField] private ParticleSystem levelUpParticles;

    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float distanceBetweenImages;
    [SerializeField] private float dashCooldown;


    private SpriteRenderer spriteRenderer;

    private bool isAlive = true;
    private bool isDashing;
    private bool inCombat;

    private float lastHeal;
    private float dashTimeLeft;
    private float lastImageXPos;
    private float lastImageYPos;
    private float lastDash = float.MinValue;

    private int direction;

    private Coroutine coroutine;

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
        AudioManager.Instance.Play("PlayerDeath");
        isAlive = false;
        GameManager.instance.deathMenuAnim.SetTrigger("Show");
    }
    protected override void ReceiveDamage(Damage dmg)
    {
        if (isAlive)
        {
            base.ReceiveDamage(dmg);
            GameManager.instance.OnHitpointChange();
        }
    }
    protected override void Start()
    {
        base.Start();
        GameManager.instance.OnHitpointChange();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //levelUpParticles = GetComponentInChildren<ParticleSystem>();
        levelUpParticles.Pause();
    }
    private void Update()
    {
        if (hitpoint < maxHitpoint / 2 && isAlive && !inCombat && Time.time - lastImmune > 5f)
            AutoHeal();
        if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
            GameManager.instance.Respawn();
        if (Input.GetKeyDown(KeyCode.E) && !DialogueUI.IsOpen)
            Interactable?.Interact(this); // if interactable != null, then interact.interact(this)

        if (Input.GetKeyDown(KeyCode.A)) //TODO improve this system using switch statement and add have 8 dash directions
            {
                direction = 1;
            }
        else if (Input.GetKeyDown(KeyCode.D))
            {
                direction = 2;
            }
        else if (Input.GetKeyDown(KeyCode.W))
            {
                direction = 3;
            }
        else if (Input.GetKeyDown(KeyCode.S))
            {
                direction = 4;
            }
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
            UpdateMotor(new Vector3(x, y, 0).normalized);
        CheckDash();
    }
    private void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXPos = transform.position.x;
        lastImageYPos = transform.position.y;
    }
    private void CheckDash()
    {
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                switch (direction)
                {
                    case 1:
                        rb.velocity = dashSpeed * Vector2.left + new Vector2(0, rb.velocity.y);
                        dashTimeLeft -= Time.deltaTime;
                        if (Mathf.Abs(transform.position.x - lastImageXPos) > distanceBetweenImages)
                        {
                            PlayerAfterImagePool.Instance.GetFromPool();
                            lastImageXPos = transform.position.x;
                        }
                        break;
                    case 2:
                        rb.velocity = dashSpeed * Vector2.right + new Vector2(0, rb.velocity.y);
                        dashTimeLeft -= Time.deltaTime;
                        if (Mathf.Abs(transform.position.x - lastImageXPos) > distanceBetweenImages)
                        {
                            PlayerAfterImagePool.Instance.GetFromPool();
                            lastImageXPos = transform.position.x;
                        }
                        break;
                    case 3:
                        rb.velocity = dashSpeed * Vector2.up + new Vector2(rb.velocity.x, 0);
                        dashTimeLeft -= Time.deltaTime;
                        if (Mathf.Abs(transform.position.y - lastImageYPos) > distanceBetweenImages)
                        {
                            PlayerAfterImagePool.Instance.GetFromPool();
                            lastImageYPos = transform.position.y;
                        }
                        break;
                    case 4:
                        rb.velocity = dashSpeed * Vector2.down + new Vector2(rb.velocity.x, 0);
                        dashTimeLeft -= Time.deltaTime;
                        if (Mathf.Abs(transform.position.y - lastImageYPos) > distanceBetweenImages)
                        {
                            PlayerAfterImagePool.Instance.GetFromPool();
                            lastImageYPos = transform.position.y;
                        }
                        break;
                }
                //rb.velocity = new Vector2(dashSpeed * rb.velocity.x, rb.velocity.y * dashSpeed);
                dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXPos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXPos = transform.position.x;
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
    }
}