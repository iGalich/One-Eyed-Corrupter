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

    public bool IsOpen { get; private set; }

    private TypewriterEffect typewriterEffect;
    private ResponseHandler responseHandler;

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
        actorName.text = "<b>" + dialogueObject.actor.ActorName + ":</b>";
        actorSprite.sprite = dialogueObject.actor.ActorSprite;
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
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }

        if (dialogueObject.HasResponses)
            responseHandler.ShowResponses(dialogueObject.Responses);

        else
            CloseDialogueBox();
    }
    private IEnumerator RunTypingEffect(string dialogue)
    {
        typewriterEffect.Run(dialogue, textLabel);

        while (typewriterEffect.IsRunning)
        {
            yield return null;

            if (Input.GetKeyDown(KeyCode.Space))
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