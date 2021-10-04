using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWalls : MonoBehaviour
{
    [SerializeField] private FireballBoss fireballBoss;

    private void Update()
    {
        if (fireballBoss.CheckIsDead())
            Destroy(gameObject);
    }
}
