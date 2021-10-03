using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundExit : MonoBehaviour
{
    private Button backgroundExitButton;

    private void Start()
    {
        backgroundExitButton = GetComponent<Button>();
        backgroundExitButton.enabled = false;
    }
    public void EnableBackgroundExit()
    {
        backgroundExitButton.enabled = true;
    }
    public void DisableBackgroundExit()
    {
        backgroundExitButton.enabled = false;
    }
}
