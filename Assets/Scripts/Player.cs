//using System.Threading.Tasks.Dataflow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mover
{
    private SpriteRenderer spriteRenderer;
    private bool isAlive = true;
    private Rigidbody2D rb;
    private ParticleSystem levelUpPaticles;

    [SerializeField] private int minHitPoint = 5;
    [SerializeField] private DialogueUI dialogueUI;

    public DialogueUI DialogueUI => dialogueUI;
    public IInteractable Interactable { get; set; }

    private void Awake()
    {
        levelUpPaticles = GetComponentInChildren<ParticleSystem>();
        levelUpPaticles.Pause();
    }
    protected override void Death()
    {
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
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        //DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            Interactable?.Interact(this); // if interactable != null, then interact.interact(this)
    }
    private void FixedUpdate()
    {
        if (dialogueUI.IsOpen) return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        if (isAlive)
            UpdateMotor(new Vector3(x, y, 0).normalized);
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
        levelUpPaticles.Play();
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
        GameManager.instance.ShowText("+" + healingAmount.ToString() + " hp", 30, Color.green, transform.position, Vector3.up * 30, 1.0f);
        GameManager.instance.OnHitpointChange();
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