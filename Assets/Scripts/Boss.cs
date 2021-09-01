using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [SerializeField] private float[] fireballSpeed = { 2.5f, -2.5f };
    [SerializeField] private float distance = 0.25f;
    [SerializeField] private Transform[] fireballs;
    [SerializeField] private float modifier = 1.5f;

    private void Update()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (fireballs[i] == null)
                continue;
            fireballs[i].position = transform.position + new Vector3(-Mathf.Cos(Time.time * fireballSpeed[i] * modifier + i) * distance,
                                                                    Mathf.Sin(Time.time * fireballSpeed[i] / modifier) * distance,
                                                                    0);
        }
    }
}
