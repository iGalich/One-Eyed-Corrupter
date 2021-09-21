using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchChestSprite : MonoBehaviour
{
    [SerializeField] private Sprite[] chestSprites;

    private Image chestImage;

    private int spriteIndex = 0;

    private void Start()
    {
        chestImage = GetComponent<Image>();
    }
    public void OnInventoryClick()
    {
        switch (spriteIndex)
        {
            case 0:
                spriteIndex++;
                break;
            case 1:
                spriteIndex--;
                break;
            default:
                Debug.Log("Might be an error here");
                break;
        }

        chestImage.sprite = chestSprites[spriteIndex];

        
    }
}
