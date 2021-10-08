using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PesosUI : MonoBehaviour
{
    private TextMeshProUGUI pesosAmount;

    private void Start()
    {
        pesosAmount = GetComponent<TextMeshProUGUI>();
        pesosAmount.text = GameManager.instance.pesos.ToString();
    }
    public void UpdatePesos()
    {
        pesosAmount.text = GameManager.instance.pesos.ToString();
    }
}
