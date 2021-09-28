using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalPesosInChests : MonoBehaviour
{
    public int totalPesosInChests;

    private void Start()
    {

        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<Chest>() != null)
                totalPesosInChests += child.gameObject.GetComponent<Chest>().PesosAmount;
            else
                Debug.LogWarning("A child of all chests doesn't have a chest component: " + child.gameObject); //warning just in case
        }
    }
}
