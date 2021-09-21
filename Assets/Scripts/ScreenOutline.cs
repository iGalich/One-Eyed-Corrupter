using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenOutline : MonoBehaviour
{
    [SerializeField] private float denominator = 10f;

    private WaitForSecondsRealtime colorChangeTick = new WaitForSecondsRealtime(0.1f);

    private Player player;
    private Image outlineImage;

    private float num;

    private bool isChangingColor;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        outlineImage = GetComponent<Image>();
        outlineImage.color = Color.black;
    }
    private void Update()
    {
        if (player.GetCurrHealth() == 1 && !isChangingColor)
        {
            isChangingColor = true;
            num = Random.value;
            if (num < 0.27f)
                num = 0.27f;
            StartCoroutine(ChangeColorCo());
        }
        if (player.GetCurrHealth() > 1)
        {
            StopCoroutine(ChangeColorCo());
            StartCoroutine(ChangeColorToBlackCo());
            if (!isChangingColor)
                StopCoroutine(ChangeColorToBlackCo());
        }
    }
    private IEnumerator ChangeColorToBlackCo()
    {
        while (outlineImage.color.r > 0)
        {
            Color newColor = new Color(outlineImage.color.r - (num / denominator), 0, 0);
            outlineImage.color = newColor;
            yield return colorChangeTick;
        }
        if (outlineImage.color.r <= 0)
        {
            isChangingColor = false;
            yield return null;
        }
    }
    private IEnumerator ChangeColorCo()
    {
        while (outlineImage.color.r != num)
        {
            if (outlineImage.color.r < num)
            {
                Color newColor = new Color(outlineImage.color.r + (num / denominator), 0, 0);
                outlineImage.color = newColor;
                yield return colorChangeTick;
            }
            if (outlineImage.color.r >= num)
                break;
        }
        while (outlineImage.color.r >= 0)
        {
            Color newColor = new Color(outlineImage.color.r - (num / denominator), 0, 0);
            outlineImage.color = newColor;

            if (outlineImage.color.r <= 0)
            {
                outlineImage.color = Color.black;
                isChangingColor = false;
            }

            yield return colorChangeTick;
        }
    }
}
