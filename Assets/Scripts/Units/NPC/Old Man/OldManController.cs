using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public class OldManController : NPCController
{
    [SerializeField] private OldManDialogue dialogue;

    private GameObject canvas;
    private GameObject dialoguePart;
    private GameObject uiPromptPart;
    private TextMeshProUGUI textMeshPro;


    private int currentLine = 0;

    // TODO: bobbing animation for E prompt
    private void Start()
    {
        canvas = transform.Find("Canvas").gameObject;
        dialoguePart = canvas.transform.Find("Dialogue Part").gameObject;
        uiPromptPart = canvas.transform.Find("UI Prompt Part").gameObject;
        textMeshPro = dialoguePart.transform.Find("Dialogue").GetComponent<TextMeshProUGUI>();
    }

    // TODO: more generic way of doing this
    public void OnControlsChanged(CallbackContext context)
    {
        if (context.performed)
        {
            InputControl control = context.control;
            if (control.parent.name.Contains("Gamepad") || (control.parent.parent != null && control.parent.parent.name.Contains("Gamepad")))
            {
                dialoguePart.transform.Find("Prompt").Find("PromptBox").Find("PromptText").GetComponent<TextMeshProUGUI>().text = "A";
                uiPromptPart.transform.Find("Prompt").Find("PromptBox").Find("PromptText").GetComponent<TextMeshProUGUI>().text = "A";
            }
            else
            {
                dialoguePart.transform.Find("Prompt").Find("PromptBox").Find("PromptText").GetComponent<TextMeshProUGUI>().text = "E";
                uiPromptPart.transform.Find("Prompt").Find("PromptBox").Find("PromptText").GetComponent<TextMeshProUGUI>().text = "E";
            }
        }
    }

    public override void OnInteract()
    {
        if (!dialoguePart.activeSelf)
        {
            uiPromptPart.SetActive(false);
            dialoguePart.SetActive(true);
        }

        if (currentLine == dialogue.dialogueLines.Length)
        {
            currentLine = 0;
            uiPromptPart.SetActive(true);
            dialoguePart.SetActive(false);
            return;
        }

        textMeshPro.text = dialogue.dialogueLines[currentLine];
        currentLine++;
    }
}
