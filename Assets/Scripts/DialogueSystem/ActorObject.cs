using UnityEngine;
using TMPro;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Actor/ActorObject")]

public class ActorObject : ScriptableObject
{
    [SerializeField] private string actorName;
    [SerializeField] private Sprite actorSprite;
    [SerializeField] private Sound actorTalkSound;

    public string ActorName => actorName;

    public Sprite ActorSprite => actorSprite;
    public Sound ActorTalkSound => actorTalkSound;
}
