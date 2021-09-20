using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayerSwingAnimation : MonoBehaviour
{
    private Animator anim;

    [SerializeField] private Sprite spaceKeyUp;
    [SerializeField] private Sprite spaceKeyDown;

    [SerializeField] private float swingCooldown = 2f;

    [SerializeField] private SpriteRenderer spaceKeySpriteRenderer;

    private WaitForSecondsRealtime spacekeyDownTime = new WaitForSecondsRealtime(0.3f);
   
    private float lastSwing;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        //if (Time.time - swingCooldown < lastSwing)
        //    spaceKeySpriteRenderer.sprite = spaceKeyUp;
        if (Time.time - swingCooldown > lastSwing)
            Swing();
    }
    private void Swing()
    {
        StartCoroutine(SpaceKeyCo());
        lastSwing = Time.time;
        anim.SetTrigger("Swing");
    }
    private IEnumerator SpaceKeyCo()
    {
        spaceKeySpriteRenderer.sprite = spaceKeyDown;
        yield return spacekeyDownTime;
        spaceKeySpriteRenderer.sprite = spaceKeyUp;
        yield return null;
    }
}
