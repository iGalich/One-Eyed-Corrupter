using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingTextManager : MonoBehaviour
{
    public GameObject textContainer;
    public GameObject textPrefab;

    private List<FloatingText> floatingTexts = new List<FloatingText>();

    //private void Start()
    //{
    //    DontDestroyOnLoad(gameObject);
    //}
    private void Update()
    {
        foreach (FloatingText txt in floatingTexts)
        {
            txt.UpdateFloatingText();
            StartCoroutine(FadeTextCo(txt));
        }
    }
    private IEnumerator FadeTextCo(FloatingText txt)
    {
        while (txt.txt.color.a > 0)
        {
            Color newColor = new Color(txt.txt.color.r, txt.txt.color.g, txt.txt.color.b, txt.txt.color.a - 0.001f);
            txt.txt.color = newColor;
            yield return new WaitForSeconds(txt.duration / 10f);
        }
        if (txt.txt.color.a <= 0)
        {
            txt.txt.color = new Color(txt.txt.color.r, txt.txt.color.g, txt.txt.color.b, 0);
            txt.Hide();
            yield return null;
        }
    }
    public void Show(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        FloatingText floatingText = GetFloatingText();

        floatingText.txt.text = msg;
        floatingText.txt.fontSize = fontSize;
        floatingText.txt.color = color;

        floatingText.go.transform.position = Camera.main.WorldToScreenPoint(position); //transfer world space to screen space

        floatingText.motion = motion;

        floatingText.duration = duration;

        floatingText.Show();
    }
    private FloatingText GetFloatingText()
    {
        FloatingText txt = floatingTexts.Find(t => !t.active);

        if (txt == null)
        {
            txt = new FloatingText();
            txt.go = Instantiate(textPrefab);
            txt.go.transform.SetParent(textContainer.transform);
            txt.txt = txt.go.GetComponent<Text>();

            floatingTexts.Add(txt);
        }

        return txt;
    }
}
