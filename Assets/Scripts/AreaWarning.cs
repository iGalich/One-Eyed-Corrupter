using UnityEngine;

public class AreaWarning : MonoBehaviour
{
    private enum AreaLevel
    {
        Safe,
        Easy,
        Normal,
        Hard,
    }

    [SerializeField] private AreaLevel areaLevel;

    [SerializeField] private bool showWarning;

    private float warningDuration = 6.5f;
    private float warningCooldown = 60f;
    private float lastWarning = float.MinValue;

    private float easyAreaPitch = 1f;
    private float normalAreaPitch = 0.75f;
    private float hardAreaPitch = 0.5f;
    private float pitchChangeDuration = 0.2f;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            switch (areaLevel)
            {
                case AreaLevel.Easy:
                    AudioManager.Instance.ChangePitch(easyAreaPitch, pitchChangeDuration);
                    break;
                case AreaLevel.Normal:
                    AudioManager.Instance.ChangePitch(normalAreaPitch, pitchChangeDuration);
                    break;
                case AreaLevel.Hard:
                    AudioManager.Instance.ChangePitch(hardAreaPitch, pitchChangeDuration);
                    break;
                case AreaLevel.Safe:
                    AudioManager.Instance.ChangePitch(easyAreaPitch, 3 * pitchChangeDuration);
                    break;
            }

            if (showWarning && Time.time - lastWarning > warningCooldown)
            {
                lastWarning = Time.time;

                switch (areaLevel)
                {
                    case AreaLevel.Normal:
                      if (GameManager.instance.GetCurrentLevel() < 4)
                            GameManager.instance.ShowText("Maybe I should train more before going ahead", 30, Color.white, collision.transform.position + new Vector3(0, 0.16f, 0), Vector3.up * 15, warningDuration);
                        break;

                    case AreaLevel.Hard:
                        if (GameManager.instance.GetCurrentLevel() < 7)
                            GameManager.instance.ShowText("I sense enemies who are more powerful than me up ahead", 30, Color.white, collision.transform.position + new Vector3(0, 0.16f, 0), Vector3.up * 15, warningDuration);
                      break;

                    case AreaLevel.Safe:
                        GameManager.instance.ShowText("I sense no danger in this area", 30, Color.white, collision.transform.position + new Vector3(0, 0.16f, 0), Vector3.up * 15, warningDuration);
                        break;
                }
            }
        }
    }
}
