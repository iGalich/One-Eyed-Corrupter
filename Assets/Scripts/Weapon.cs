using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Collidable
{
    // damage structure
    public int[] damagePoint = { 1, 2, 3, 4, 5, 6, 7 };
    public float[] pushForce = { 2.0f, 2.2f, 2.5f, 3f, 3.2f, 3.6f, 4f };

    // upgrades
    public int weaponLevel = 0;

    public SpriteRenderer spriteRenderer;

    // swing
    private Animator anim;

    private float lastSwing;

    [SerializeField] private float cooldown = 0.5f;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
    }
    protected override void Update()
    {
        if (GameManager.instance.player.DialogueUI.IsOpen) return; // stops swing during dialogue

        base.Update();
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Time.time - lastSwing > cooldown)
            {
                lastSwing = Time.time;
                Swing();
            }
        }
    }
    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter")
        {
            if (coll.name == "Player")
            {
                return;
            }

            //create new damage object, and send it to the fighter hit
            Damage dmg = new Damage()
            {
                damageAmount = damagePoint[weaponLevel],
                origin = transform.position,
                pushForce = pushForce[weaponLevel]
            };

            if (GameManager.instance.player.GetIsDashing())
                dmg.damageAmount *= 2;

            coll.SendMessage("ReceiveDamage", dmg);
        }
    }
    private void Swing()
    {
        anim.SetTrigger("Swing");
    }
    public void UpgradeWeapon()
    {
        weaponLevel++;
        spriteRenderer.sprite = GameManager.instance.weaponSprites[weaponLevel];

        //change stats
    }
    public void SetWeaponLevel(int level)
    {
        weaponLevel = level;
        spriteRenderer.sprite = GameManager.instance.weaponSprites[weaponLevel];
    }
}
