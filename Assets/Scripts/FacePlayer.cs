using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    private void Update()
    {
        Vector3 characterScale = transform.localScale;
        if (GameManager.instance.player.transform.position.x < transform.position.x && characterScale.x > 0)
            characterScale.x *= -1;
        if (GameManager.instance.player.transform.position.x > transform.position.x)
            characterScale.x = Mathf.Abs(characterScale.x);
        transform.localScale = characterScale;
    }
}
