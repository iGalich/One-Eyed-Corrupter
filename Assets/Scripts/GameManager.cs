using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // resources
    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;

    public List<int> weaponPrices;
    public List<int> xpTable;

    // references
    public Player player;

    public Weapon weapon;

    public RectTransform hitpointBar;
    [SerializeField] private RectTransform decayingBar;

    public Animator deathMenuAnim;
    private Animator inventoryMenuAnim;

    public FloatingTextManager floatingTextManager;

    public GameObject vCam;
    public GameObject hud;
    public CharacterMenu menu;
    public GameObject dialogue;
    public GameObject playerAfterImagePool;
    public GameObject audioManager;
    public GameObject stamina;

    [SerializeField] private Button inventoryButton;

    // logic
    public int pesos;
    public int experience;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (GameManager.instance != null)
        {
            Destroy(gameObject);
            Destroy(player.gameObject);
            Destroy(floatingTextManager.gameObject);
            Destroy(vCam.gameObject);
            Destroy(menu.gameObject);
            Destroy(hud.gameObject);
            Destroy(dialogue.gameObject);
            Destroy(playerAfterImagePool.gameObject);
            Destroy(audioManager.gameObject);
            Destroy(stamina.gameObject);
            return;
        }

        if (hud.activeSelf == false)
            hud.SetActive(true);
        if (menu.gameObject.activeSelf == false)
            menu.gameObject.SetActive(true);
        if (dialogue.activeSelf == false)
            dialogue.SetActive(true);

        instance = this;

        SceneManager.sceneLoaded += LoadState;
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        UpdateSceneMusicPlayer();
    }
    private void Start()
    {
        inventoryMenuAnim = GameObject.Find("Menu").GetComponent<Animator>();
    }
    private void Update()
    {
        SyncBar();
    }
    public Animator GetInventoryMenuAnim()
    {
        return inventoryMenuAnim;
    }
    private void SyncBar()
    {
        if (decayingBar.localScale.y != hitpointBar.localScale.y)
        {
            decayingBar.localScale = new Vector3(decayingBar.localScale.x, Mathf.Lerp(decayingBar.localScale.y, hitpointBar.localScale.y, Time.unscaledDeltaTime), decayingBar.localScale.z);
        }
    }
    public void CanClickInvetnory(bool canClick)
    {
        inventoryButton.enabled = canClick;
    }
    public bool IsInventoryClickable()
    {
        return inventoryButton.enabled;
    }
    //floating text
    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
    }
    // upgrade weapon
    public bool TryUpgradeWeapon()
    {
        // is the weapon maxed
        if (weaponPrices.Count <= weapon.weaponLevel)
            return false;

        if (pesos >= weaponPrices[weapon.weaponLevel])
        {
            pesos -= weaponPrices[weapon.weaponLevel];
            weapon.UpgradeWeapon();
            return true;
        }

        return false;
    }
    private void CheckIfWeaponUpgradeable()
    {
        if (weaponPrices.Count <= weapon.weaponLevel)
            return;
        if (pesos >= weaponPrices[weapon.weaponLevel])
            ShowText("I've got enough pesos to upgrade my blade", 30, Color.white, player.transform.position - new Vector3(0, 0.16f, 0), Vector3.up * 25, 4.5f);
    }

    //hitpoint bar
    public void OnHitpointChange()
    {
        float ratio = (float)player.hitpoint / (float)player.maxHitpoint;
        hitpointBar.localScale = new Vector3(1, ratio, 1);
    }

    // experience system
    public int GetCurrentLevel()
    {
        int r = 0;
        int add = 0;

        while (experience >= add)
        {
            add += xpTable[r];
            r++;

            if (r == xpTable.Count) //max level check
                return r;
        }

        return r;
    }
    public int GetXpToLevel(int level)
    {
        int r = 0;
        int xp = 0; 

        while (r < level)
        {
            xp += xpTable[r];
            r++;
        }
        return xp;
    }
    public void GrantXp(int xp)
    {
        int currLevel = GetCurrentLevel();
        int prevLevel = currLevel;
        experience += xp;
        if (currLevel < GetCurrentLevel())
            OnLevelUp(prevLevel);
    }
    public void SetXp(int xp)
    {
        experience = xp;
    }
    public void OnLevelUp(int prevLevel)
    {
        player.OnLevelUp();
        if (GetCurrentLevel() == 10)
        {
            ShowText("Max Level", 50, Color.yellow, player.transform.position + new Vector3(0, 0.48f, 0), Vector3.up * 15, 5.5f);
            ShowText("Max Health Increased by " + (player.MaxHitPointIncreasePerLevel + 3), 30, Color.red, player.transform.position + new Vector3(0, 0.32f, 0), Vector3.up * 15, 5.5f);
            if (player.hitpoint == player.maxHitpoint)
                ShowText("Health fully restored", 30, Color.green, player.transform.position + new Vector3(0, 0.16f, 0), Vector3.up * 15, 5.5f);
            else
                ShowText("Health restored by " + player.HealthRestoreOnLevelUp, 30, Color.green, player.transform.position + new Vector3(0, 0.16f, 0), Vector3.up * 15, 5.5f);
        }
        else
        {
            ShowText("Level " + GameManager.instance.GetCurrentLevel().ToString(), 50, Color.yellow, player.transform.position + new Vector3(0, 0.48f, 0), Vector3.up * 15, 5.5f);
            ShowText("Max Health Increased by " + player.MaxHitPointIncreasePerLevel, 30, Color.red, player.transform.position + new Vector3(0, 0.32f, 0), Vector3.up * 15, 5.5f);
            if (player.hitpoint == player.maxHitpoint)
                ShowText("Health fully restored", 30, Color.green, player.transform.position + new Vector3(0, 0.16f, 0), Vector3.up * 15, 5.5f);
            else
                ShowText("Health restored by " + player.HealthRestoreOnLevelUp, 30, Color.green, player.transform.position + new Vector3(0, 0.16f, 0), Vector3.up * 15, 5.5f);
        }
        if (GetCurrentLevel() - prevLevel > 1)
        {
            int levelGap = GetCurrentLevel() - prevLevel;
            player.CorrectMaxHitPoint(levelGap);
            player.hitpoint += (levelGap - 1) * player.HealthRestoreOnLevelUp;
            if (player.hitpoint > player.maxHitpoint)
                player.hitpoint = player.maxHitpoint;
        }
        OnHitpointChange();
    }

    //grant pesos
    public void GrantPesos(int pesosAmount)
    {
        if (pesosAmount > 0)
        {
            pesos += pesosAmount;
            ShowText("+" + pesosAmount + " pesos!", 30, Color.yellow, player.transform.position, Vector3.up * 25, 1.5f);
            AudioManager.Instance.Play("GotPesos");
            CheckIfWeaponUpgradeable();
        }
        else if (pesosAmount < 0)
        {
            pesos += pesosAmount;
            ShowText(pesosAmount + " pesos", 30, Color.red, player.transform.position - new Vector3(0 ,0.16f, 0) , Vector3.up * 25, 2.5f);
        }
    }
    public void SetPesos(int pesosAmount)
    {
        pesos = pesosAmount;
    }
    
    //death menu and respawn
    public void Respawn()
    {
        deathMenuAnim.SetTrigger("Hide");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Tutorial");
        player.Respawn();
        AudioManager.Instance.Mute("PlayerDeath");
        AudioManager.Instance.Play("TutorialLevel");
    }

    // save state
    /* INT preferedSkin
     * INT pesos
     * INT experience
     * INT weaponLevel
     */
    public void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        player.transform.position = GameObject.Find("SpawnPoint").transform.position;
        UpdateSceneMusicPlayer();
    }
    public void SaveState()
    {
        string s = "";

        s += "0" + "|";
        s += pesos.ToString() + "|";
        s += experience.ToString() + "|";
        s += weapon.weaponLevel.ToString();

        PlayerPrefs.SetString("SaveState", s);
    }
    public void LoadState(Scene s, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= LoadState;

        if (!PlayerPrefs.HasKey("SaveState"))
            return;

        string[] data = PlayerPrefs.GetString("SaveState").Split('|');

        //change prefskin
        pesos = int.Parse(data[1]);
        //experience
        experience = int.Parse(data[2]);
        if (GetCurrentLevel() != 1)
            player.SetLevel(GetCurrentLevel());
        //weapon level
        weapon.SetWeaponLevel(int.Parse(data[3]));
    }
    public void UpdateSceneMusicPlayer()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 1: AudioManager.Instance.Play("TutorialLevel");
                break;
            case 2: AudioManager.Instance.Play("RegularLevel");
                break;
            case 3: AudioManager.Instance.Play("BossLevel");
                break;
            default: Debug.Log("No music is playing!!!");
                break;
        }
    }
    /// <summary>
    /// Flips time scale between 0 & 1
    /// </summary>
    public void SetTimeScale()
    {
        switch (Time.timeScale)
        {
            case 0: Time.timeScale = 1;
                break;
            case 1: Time.timeScale = 0;
                break;
            default: return;
        }
    }
}
