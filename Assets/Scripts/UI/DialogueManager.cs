using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] Transform dialogueUI, buttonParent;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] UnityEvent onStartDialogue, onEndDialogue;

    int index;

    DialogueData[] dialogue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.AddOnDialogueStartedListener(StartDialogue);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ShowCurrentDialogue()
    {
        DialogueData currentDialogue = dialogue[index];
        dialogueText.SetText(currentDialogue.Text);
        
        DialogueButtonData[] buttonData = currentDialogue.ButtonData;
        if (buttonData.Length == 0) return;
        for (int i = 0; i < buttonParent.childCount; i++)
        {
            GameObject button = buttonParent.GetChild(i).gameObject;
            bool isActive = i < currentDialogue.ButtonData.Length;
            button.SetActive(isActive);

            if (!isActive) continue;
            DialogueButtonData currentButtonData = buttonData[i];
            button.GetComponent<TextMeshPro>().SetText(currentButtonData.Text);
            currentButtonData.OnClick.AddListener(() => currentButtonData.OnClick.Invoke());
        }
    }

    void StartDialogue(DialogueData[] dialogue)
    {
        this.dialogue = dialogue;

        index = 0;
        ShowCurrentDialogue();
    }

    public void IncrementDialogue()
    {
        index++;

        if (index == dialogue.Length)
        {
            index = 0;
            dialogueUI.gameObject.SetActive(false);
        }
        else
        {
            ShowCurrentDialogue();
        }
    }
}

[System.Serializable]
public class DialogueData
{
    [SerializeField] string text;
    [SerializeField] DialogueButtonData[] buttonData;

    public string Text { get => text; }
    public DialogueButtonData[] ButtonData { get => buttonData; }
}

[System.Serializable]
public class DialogueButtonData
{
    [SerializeField] string text;
    [SerializeField] UnityEvent onclick;

    public string Text { get => text; }
    public UnityEvent OnClick { get => onclick; }
}