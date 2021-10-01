using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMenu : MonoBehaviour
{
    // text fields
    [SerializeField] private Text levelText, hitpointText, pesosText, upgradeCostText, xpText;

    // logic
    private int currentCharacterSelection = 0;

    [SerializeField] private Image characterSelectionSprite, weaponSprite;

    [SerializeField] private RectTransform xpBar;

    private bool menuIsOpen;

    private void Update()
    {
        if (GameManager.instance.IsInventoryClickable() && menuIsOpen)
            GameManager.instance.CanClickInvetnory(false);
    }
    // character selection
    public void OnArrowClick (bool right)
    {
        if (right)
        {
            currentCharacterSelection++;

            //if reaches beyond last
            if (currentCharacterSelection == GameManager.instance.playerSprites.Count)
                currentCharacterSelection = 0;

            OnSelectionChange();
        }
        else
        {
            currentCharacterSelection--;

            //if reaches beyond last
            if (currentCharacterSelection < 0)
                currentCharacterSelection = GameManager.instance.playerSprites.Count - 1;

            OnSelectionChange();
        }
    }

    private void OnSelectionChange()
    {
        characterSelectionSprite.sprite = GameManager.instance.playerSprites[currentCharacterSelection];
        GameManager.instance.player.SwapSprite(currentCharacterSelection);
    }
    // weapon upgrade
    public void OnUpgradeClick()
    {
        if (GameManager.instance.TryUpgradeWeapon())
            UpdateMenu();
    }
    public void SetMenuIsOpen(bool isOpen)
    {
        menuIsOpen = isOpen;
    }

    // update character information
    public void UpdateMenu()
    { 
        int currLevel = GameManager.instance.GetCurrentLevel();
        //weapon
        weaponSprite.sprite = GameManager.instance.weaponSprites[GameManager.instance.weapon.weaponLevel + 1];
        if (GameManager.instance.weapon.weaponLevel == GameManager.instance.weaponPrices.Count)
            upgradeCostText.text = "MAX";
        else
            upgradeCostText.text = GameManager.instance.weaponPrices[GameManager.instance.weapon.weaponLevel].ToString();


        //meta
        levelText.text = currLevel.ToString();
        hitpointText.text = GameManager.instance.player.hitpoint.ToString() + " / " + GameManager.instance.player.maxHitpoint.ToString();
        pesosText.text = GameManager.instance.pesos.ToString();

        //xp bar
        if (currLevel == GameManager.instance.xpTable.Count)
        {
            xpText.text = GameManager.instance.experience.ToString() + " total experience points";
            xpBar.localScale = Vector3.one;
        }
        else
        {
            int prevLevelXp = GameManager.instance.GetXpToLevel(currLevel - 1);
            int currLevelXp = GameManager.instance.GetXpToLevel(currLevel);

            int diff = currLevelXp - prevLevelXp;
            int currXpIntoLevel = GameManager.instance.experience - prevLevelXp;

            float completionRatio = (float)currXpIntoLevel / (float)diff;
            xpBar.localScale = new Vector3(completionRatio, 1, 1);
            xpText.text = currXpIntoLevel.ToString() + " / " + diff;
        }
    }
    public void CloseOpenInventory()
    {
        switch (menuIsOpen)
        {
            case true:
                GameManager.instance.GetInventoryMenuAnim().SetTrigger("hide");
                GameManager.instance.CanClickInvetnory(true);
                menuIsOpen = false;
                break;

        }
    }
}
