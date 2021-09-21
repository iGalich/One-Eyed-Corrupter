using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueObject dialogueObject;

    [SerializeField] private GameObject eKeyPrefab;

    private DialogueUI dialogueUI;

    private GameObject eKey;

    private const float radius = 0.25f;

    private void Start()
    {
        dialogueUI = GameObject.Find("Canvas").GetComponent<DialogueUI>();
    }
    private void Update()
    {
        if (Vector3.Distance(transform.position, GameObject.Find("Player").transform.position) < radius && eKey == null && !dialogueUI.IsOpen)
        {
            eKey = Instantiate(eKeyPrefab, transform.position + new Vector3(0, 0.2f, 0), transform.rotation);
        }
    }
    public void UpdateDialogueObject(DialogueObject dialogueObject)
    {
        this.dialogueObject = dialogueObject;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "Player" && other.TryGetComponent(out Player player))
        {
            //eKey = Instantiate(eKeyPrefab, transform.position + new Vector3(0, 0.2f, 0), transform.rotation);
            player.Interactable = this;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "Player" && other.TryGetComponent(out Player player))
        {
            if (player.Interactable is DialogueActivator dialogueActivator && dialogueActivator == this)
            {
                Destroy(eKey);
                player.Interactable = null;
            }
        }
    }
    public void Interact(Player player)
    {
        foreach (DialogueResponseEvents responseEvents in GetComponents<DialogueResponseEvents>())
        {
            if (responseEvents.DialogueObject == dialogueObject)
            {
                player.DialogueUI.AddResponseEvents(responseEvents.Events);
                break;
            }
        }
        Destroy(eKey);
        player.DialogueUI.ShowDialogue(dialogueObject);
    }
}