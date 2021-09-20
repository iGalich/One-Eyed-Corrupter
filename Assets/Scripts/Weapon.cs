using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Collidable
{
    [SerializeField] private int dashDmgMultiplier = 2;
    [SerializeField] private int critDmgMultiplier = 3;

    // damage structure
    public int[] damagePoint = { 1, 2, 3, 4, 5, 6, 7 };
    public float[] pushForce = { 2.0f, 2.2f, 2.5f, 3f, 3.2f, 3.6f, 4f };
    public float[] critChance = { 1f, 1.5f, 2f, 2.5f, 3f, 4f, 5f };

    // upgrades
    public int weaponLevel = 0;

    public SpriteRenderer spriteRenderer;

    // swing
    private Animator anim;

    private float lastSwing;
    private float dashTextMultiSize = 1f;
    private float critTextMultiSize = 1f;

    [SerializeField] private float cooldown = 0.5f;

    // hit feel
    private bool stoppingTime = false; // is time stopped

    private Vector3 camPositionOriginal;

    private string hitLandOnEnemySFX;

    [SerializeField] private float stopTime = 0.2f; // how long to stop time for
    [SerializeField] private float slowTime = 0.2f;
    [SerializeField] private float shake = 0.5f;
    [SerializeField] private Transform cam;


    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
    }
    public string GetHitLandOnEnemySFX()
    {
        return hitLandOnEnemySFX;
    }
    protected override void Update()
    {
        if (GameManager.instance.player.DialogueUI.IsOpen) return; // stops swing during dialogue

        base.Update();

        if (cam == null)
            cam = FindObjectOfType<Camera>().transform;

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
        if (coll.tag == "Fighter" && coll.GetComponent<Fighter>() != null)
        {
            if (coll.name == "Player")
            {
                return;
            }

            hitLandOnEnemySFX = "NormalPlayerHit";

            //create new damage object, and send it to the fighter hit
            Damage dmg = new Damage()
            {
                damageAmount = damagePoint[weaponLevel],
                origin = transform.position,
                pushForce = pushForce[weaponLevel]
            };

            if (GameManager.instance.player.GetIsDashing())
            {
                dmg.damageAmount *= dashDmgMultiplier;
                dashTextMultiSize = 1.2f;
                hitLandOnEnemySFX = "DashPlayerHit";
            }

            if (Random.value > (100f - GameManager.instance.weapon.critChance[weaponLevel]) / 100f)
            {
                dmg.damageAmount *= critDmgMultiplier;
                critTextMultiSize = 1.5f;
                hitLandOnEnemySFX = "CritPlayerHit";
            }

            object[] tempStorage = new object[2];
            

            coll.SendMessage("ReceiveDamage", dmg);
        }
    }
    private void Swing()
    {
        anim.SetTrigger("Swing");
        switch (weaponLevel)
        {
            case 0:
                AudioManager.Instance.Play("SwordSwingWooden");
                break;
            default:
                AudioManager.Instance.Play("SwordSwingNormal");
                break;
        }
    }
    public void TimeStop()
    {
        if (!stoppingTime)
        {
            stoppingTime = true;
            Time.timeScale = 0; // time is stopped

            StartCoroutine("Stop");
            StartCoroutine("CamAction");
        }
    }
    private IEnumerator Stop()
    {
        yield return new WaitForSecondsRealtime(stopTime); // stops time
        Time.timeScale = 0.01f;

        yield return new WaitForSecondsRealtime(slowTime); // slow motion effect

        Time.timeScale = 1; // normal time flow
        stoppingTime = false;
    }
    private IEnumerator CamAction()
    {
        camPositionOriginal = cam.position;

        cam.position = new Vector3(cam.position.x + Random.Range(-shake, shake), cam.position.y + Random.Range(-shake, shake), cam.position.z + Random.Range(-shake, shake));

        yield return new WaitForSecondsRealtime(0.001f);

        cam.position = new Vector3(cam.position.x + Random.Range(-shake, shake), cam.position.y + Random.Range(-shake, shake), cam.position.z + Random.Range(-shake, shake));

        yield return new WaitForSecondsRealtime(0.05f);

        cam.transform.position = camPositionOriginal;
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
    public float GetDashTextMulti()
    {
        return dashTextMultiSize;
    }
    public void SetDashTextMulti()
    {
        dashTextMultiSize = 1f;
    }
    public float GetCritTextMulti()
    {
        return critTextMultiSize;
    }
    public void SetCritTextMulti()
    {
        critTextMultiSize = 1f;
    }
}
