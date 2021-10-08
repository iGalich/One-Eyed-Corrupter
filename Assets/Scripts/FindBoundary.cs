using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FindBoundary : MonoBehaviour
{
    public static FindBoundary Instance;

    [SerializeField] private CinemachineVirtualCamera vcam;

    public void GetBoundary()
    {
        var bounds = GameObject.Find("CreditsBounds");
        if (bounds != null)
        {
            vcam.GetComponent<CinemachineConfiner>().m_BoundingShape2D = bounds.GetComponent<BoxCollider2D>();
        }
    }
}
