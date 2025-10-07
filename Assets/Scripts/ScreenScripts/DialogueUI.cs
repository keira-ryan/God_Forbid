using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI textComponent;
    
    public bool IsOpen { get; private set; }
    
    private ResponseController responseController;
    private DialogueTypewriter dialogueTypewriter;

    private void Start()
    {
        dialogueTypewriter = GetComponent<DialogueTypewriter>();
        responseController = GetComponent<ResponseController>();
        CloseDialogue();
    }

    public void ShowDialogue(DialogueObject dialogueObject)
    {
        GameController.Instance.SetGameState(GameState.Dialogue);
        IsOpen = true;
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        responseController.AddResponseEvents(responseEvents);
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];

            yield return RunTypingEffect(dialogue);
            
            textComponent.text = dialogue;

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;

            yield return null;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        }
        
        if (dialogueObject.HasResponses)
        {
            responseController.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            CloseDialogue();
        }
    }

    private IEnumerator RunTypingEffect(string dialogue)
    {
        dialogueTypewriter.Run(dialogue, textComponent);

        while (dialogueTypewriter.IsRunning)
        {
            yield return null;

            if (Input.GetKeyDown(KeyCode.E))
            {
                dialogueTypewriter.Stop();
            }
        }
    }

    public void CloseDialogue()
    {
        GameController.Instance.SetGameState(GameState.Playing);
        IsOpen = false;
        dialogueBox.SetActive(false);
        textComponent.text = string.Empty;
    }
}
