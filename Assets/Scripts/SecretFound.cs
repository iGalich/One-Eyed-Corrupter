using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretFound : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            AudioManager.Instance.Play("SecretFound");
        }
    }
}
