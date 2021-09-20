using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private Image actorSprite;
    [SerializeField] private TMP_Text actorName;

    [SerializeField] private string playerName;

    public bool IsOpen { get; private set; }

    private TypewriterEffect typewriterEffect;
    private ResponseHandler responseHandler;

    private Sound actorTalkSound;

    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();

        CloseDialogueBox();
    }

    public void ShowDialogue(DialogueObject dialogueObject)
    {
        IsOpen = true;
        dialogueBox.SetActive(true);
        actorSprite.enabled = true;
        actorName.enabled = true;


        if (dialogueObject.actor == null) // if speaker is Player
        {
            actorName.text = "<b>" + playerName + ":</b>";
            actorSprite.sprite = GameManager.instance.player.GetPlayerSprite();
            actorTalkSound = AudioManager.Instance.FindSound("PlayerTalk");
        }
        else
        {
            actorName.text = "<b>" + dialogueObject.actor.ActorName + ":</b>";
            actorSprite.sprite = dialogueObject.actor.ActorSprite;
            actorTalkSound = dialogueObject.actor.ActorTalkSound;
        }

        GameManager.instance.CanClickInvetnory(false);

        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        responseHandler.AddResponseEvents(responseEvents);
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];

            yield return RunTypingEffect(dialogue);

            textLabel.text = dialogue;

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses)
                break;

            yield return null;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0));
        }

        if (dialogueObject.HasResponses)
            responseHandler.ShowResponses(dialogueObject.Responses);

        else
            CloseDialogueBox();
    }
    private IEnumerator RunTypingEffect(string dialogue)
    {
        typewriterEffect.Run(dialogue, textLabel, actorTalkSound);

        while (typewriterEffect.IsRunning)
        {
            yield return null;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
            {
                typewriterEffect.Stop();
            }
        }
    }
    public void CloseDialogueBox()
    {
        IsOpen = false;
        dialogueBox.SetActive(false);
        actorSprite.enabled = false;
        actorName.enabled = false;
        GameManager.instance.CanClickInvetnory(true);
        textLabel.text = string.Empty;
    }

    //private string ToBold(string line)
    //{
    //    line = 
    //}
}