using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretFound : MonoBehaviour
{
    private bool playedOnce;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player" && !playedOnce)
        {
            playedOnce = true;
            AudioManager.Instance.Play("SecretFound");
        }
    }
}
