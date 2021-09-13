using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq; //used for List.OrderBy

public class OptionsScreen : MonoBehaviour
{
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private List<ResolutionItem> resolutions = new List<ResolutionItem>();
    [SerializeField] private TMP_Text resLabel;

    private int selectedResolution;

    /// <summary>
    /// Makes sure resolution list is ordered at the start of the game, so check isn't noticable
    /// </summary>
    private void Awake()
    {
        resolutions = resolutions.OrderBy(e => e.CalTotalPixels()).ToList();
    }
    private void Start()
    {
        fullscreenToggle.isOn = Screen.fullScreen; //checks if screen is fullscreend or not

        if (QualitySettings.vSyncCount != 0) //makes sure vsyncs is off
            QualitySettings.vSyncCount = 0;

        bool foundRes = false;
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (Screen.width == resolutions[i].horizontal && Screen.height == resolutions[i].vertical)
            {
                foundRes = true;
                selectedResolution = i;
                UpdateResLabel();
            }
        }
        if (!foundRes)
        {
            ResolutionItem newRes = new ResolutionItem();
            newRes.horizontal = Screen.width;
            newRes.vertical = Screen.height;

            resolutions.Add(newRes);
            resolutions = resolutions.OrderBy(e => e.CalTotalPixels()).ToList();
            for (int i = 0; i < resolutions.Count; i++)
            {
                if (Screen.width == resolutions[i].horizontal && Screen.height == resolutions[i].vertical)
                {
                    selectedResolution = i;
                    UpdateResLabel();
                }
            }
        }
    }
    public void ResoutionLeft()
    {
        selectedResolution--;
        if (selectedResolution < 0)
            selectedResolution = 0;
        UpdateResLabel();
    }
    public void ResolutionRight()
    {
        selectedResolution++;
        if (selectedResolution > resolutions.Count - 1)
            selectedResolution = resolutions.Count - 1;
        UpdateResLabel();
    }
    public void ApplyGraphics()
    {
        Debug.Log("Graphical changes have been applied"); //simple debug.log since fullscreen isn't toggled on in unity
        //Screen.fullScreen = fullscreenToggle.isOn;
        Screen.SetResolution(resolutions[selectedResolution].horizontal, resolutions[selectedResolution].vertical, fullscreenToggle.isOn);
    }
    public void UpdateResLabel()
    {
        resLabel.text = resolutions[selectedResolution].horizontal.ToString() + " X " + resolutions[selectedResolution].vertical.ToString();
    }
}

[System.Serializable]
public class ResolutionItem
{
    public int horizontal, vertical;
    public int CalTotalPixels()
    {
        return horizontal * vertical;
    }
}
