using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    public static Stamina Instance { get; private set; }

    [SerializeField] private GameObject entireStaminaBar;

    [SerializeField] private RectTransform frontStaminaBar;
    [SerializeField] private RectTransform backStaminaBar;

    [SerializeField] private float staminaRegenCooldown = 0.1f;
    [SerializeField] private float shakeAmount;

    private const int staminaCost = 100;

    private float staminaRatio;
    private float currStamina;
    private const float maxStamina = 100f;

    private bool reacharging = false;
    private bool shakingBar;

    private WaitForSeconds regenTick;

    private void Awake()
    {
        Instance = this;
        if (Instance == null)
            Instance = this;
    }
    private void Start()
    {
        regenTick = new WaitForSeconds(staminaRegenCooldown);
        currStamina = maxStamina;
    }
    private void Update()
    {
        SyncBar();
        if (!reacharging && currStamina != maxStamina)
        {
            reacharging = true;
            StartCoroutine(Recharge());
        }
        if (shakingBar)
        {
            iTween.ShakePosition(entireStaminaBar, Vector3.one * shakeAmount, 1f);
        }
    }
    private void SyncBar()
    {
        if (backStaminaBar.localScale.y != frontStaminaBar.localScale.y)
            backStaminaBar.localScale = new Vector3(backStaminaBar.localScale.x, Mathf.Lerp(backStaminaBar.localScale.y, frontStaminaBar.localScale.y, Time.unscaledDeltaTime), backStaminaBar.localScale.z);
    }
    public void OnStaminaUse()
    {
        currStamina -= staminaCost;
        if (currStamina < 0)
            currStamina = 0;
        staminaRatio = currStamina / maxStamina;
        frontStaminaBar.localScale = new Vector3(1, staminaRatio, 1);
    }
    public bool CheckStamina()
    {
        if (currStamina - staminaCost >= 0)
            return true;
        else return false;
    }
    private IEnumerator Recharge()
    {
        //yield return regenTick;

        while (currStamina < maxStamina)
        {
            currStamina += maxStamina / 100f * 2;
            staminaRatio = currStamina / maxStamina;
            frontStaminaBar.localScale = new Vector3(1, staminaRatio, 1);
            yield return regenTick;
        }
        reacharging = false;
    }
    public void ShakeBar()
    {
        StartCoroutine(ShakeCo());
    }
    private IEnumerator ShakeCo()
    {
        if (shakingBar == false)
            shakingBar = true;

        yield return new WaitForSeconds(0.25f);

        shakingBar = false;
    }
}
