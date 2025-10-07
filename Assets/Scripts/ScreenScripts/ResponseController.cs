using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResponseController : MonoBehaviour
{
    [SerializeField] private RectTransform responseBox;
    [SerializeField] private RectTransform responseButtonTemplate;
    [SerializeField] private RectTransform responseContainer;
    
    private DialogueUI dialogueUI;
    private ResponseEvent[]  responseEvents;
    
    List<GameObject> tempResponses = new List<GameObject>();

    private void Start()
    {
        dialogueUI = GetComponent<DialogueUI>();
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        this.responseEvents = responseEvents;
    }

    public void ShowResponses(Response[] responses)
    {
        float responseBoxHeight = 0;

        for (int i = 0; i < responseEvents.Length; i++)
        {
            Response response = responses[i];
            int responseIndex = i;
            
            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
            responseButton.gameObject.SetActive(true);
            responseButton.GetComponentInChildren<TextMeshProUGUI>().text = response.ResponseText;
            responseButton.GetComponentInChildren<Button>().onClick.AddListener(() => OnPickedResposne(response, responseIndex));
            
            tempResponses.Add(responseButton);
            
            responseBoxHeight += responseButtonTemplate.sizeDelta.y;
        }
        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
        responseBox.gameObject.SetActive(true);
    }

    private void OnPickedResposne(Response response, int responseIndex)
    {
        responseBox.gameObject.SetActive(false);

        foreach (GameObject responseButton in tempResponses)
        {
            Destroy(responseButton);
        }
        tempResponses.Clear();

        if (responseEvents != null && responseIndex <= responseEvents.Length)
        {
            responseEvents[responseIndex].OnPickedResponse?.Invoke();
        }
        responseEvents = null;

        if (response.DialogueObject)
        {
            dialogueUI.ShowDialogue(response.DialogueObject);
        }
        else
        {
            dialogueUI.CloseDialogue();
        }
        
        dialogueUI.ShowDialogue(response.DialogueObject);
    }
    
}
