using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossCutscene : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private CinemachineVirtualCamera bossVcam;
    [SerializeField] private GameObject bossRoomWalls;
    [SerializeField] private FireballBoss fireballBoss;
    [SerializeField] private RectTransform bossTextCanvas;

    private bool triggerActivated;
    private bool moveCamera;

    private CinemachineImpulseSource impulseSource;

    private Coroutine textMoveCoroutine;

    private float textSpeed = 0.75f;

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        bossTextCanvas.localPosition = new Vector3(bossTextCanvas.localPosition.x, bossTextCanvas.localPosition.y, -125f);
    }
    private void FixedUpdate()
    {
        if (moveCamera)
            bossTextCanvas.localPosition = Vector3.Lerp(bossTextCanvas.localPosition, new Vector3(bossTextCanvas.localPosition.x, bossTextCanvas.localPosition.y, 100f), curve.Evaluate(textSpeed * Time.deltaTime));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player" && !triggerActivated)
        {
            triggerActivated = true;
            GameManager.instance.player.CanMove = false;
            bossVcam.Priority += 2;
            //FunctionTimer.Create(() => FindObjectOfType<Camera>().orthographic = false, 0f);
            FunctionTimer.Create(() => moveCamera = true, 2f);
            FunctionTimer.Create(Scream, 4f);
            FunctionTimer.Create(() => bossRoomWalls.SetActive(true), 2f);
            FunctionTimer.Create(() => bossVcam.Priority -= 2, 6f);
            FunctionTimer.Create(() => GameManager.instance.player.CanMove = true, 6);
            FunctionTimer.Create(() => AudioManager.Instance.Play("BossLevel"), 6f);
            FunctionTimer.Create(() => fireballBoss.CanMove = true, 6f);
            //FunctionTimer.Create(() => FindObjectOfType<Camera>().orthographic = true, 6f);)
        }
    }
    private void Scream()
    {
        fireballBoss.GetComponent<BossScreamSource>().LastScream = Time.time;
        AudioManager.Instance.Play("BossScream");
        impulseSource.GenerateImpulse(new Vector3(0.75f, 0.75f, 0));
    }
}
