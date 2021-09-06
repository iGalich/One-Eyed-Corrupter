using UnityEngine;
using TMPro;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Actor/ActorObject")]

public class ActorObject : ScriptableObject
{
    [SerializeField] private string actorName;
    [SerializeField] private Sprite actorSprite;

    public string ActorName => actorName;

    public Sprite ActorSprite => actorSprite;
}
