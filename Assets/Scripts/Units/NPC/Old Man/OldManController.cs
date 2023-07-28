using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;
using static Helpers;

public class OldManController : NPCController
{
    [SerializeField] private OldManDialogue dialogue;

    public GameObject canvas;
    public GameObject dialoguePart;
    public GameObject uiPromptPart;
    private GameObject player;
    private AudioSource[] gruntSources;
    private TextMeshProUGUI textMeshPro;

    private int lastPlayed = -1; // start lastPlayed at -1 so the first time it's truly random and 0 isn't excluded
    private int currentDialogueLine = 0;

    // TODO: bobbing animation for E prompt
    private void Start()
    {
        gruntSources = GetComponents<AudioSource>();
        textMeshPro = dialoguePart.transform.Find("Dialogue").GetComponent<TextMeshProUGUI>();
        player = GameObject.Find("Player");
    }

    // TODO: more generic way of doing this, can use some functional programming
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
        // look at player
        FlipSprite(transform.position - player.transform.position, transform, flipSpriteOnly: true);

        // switch dialogue
        if (!dialoguePart.activeSelf)
        {
            uiPromptPart.SetActive(false);
            dialoguePart.SetActive(true);
        }

        if (currentDialogueLine == dialogue.dialogueLines.Length)
        {
            currentDialogueLine = 0;
            uiPromptPart.SetActive(true);
            dialoguePart.SetActive(false);
            return;
        }

        textMeshPro.text = dialogue.dialogueLines[currentDialogueLine];
        currentDialogueLine++;

        // play random grunt audio clip
        int randomIndex = GetRandomExcluding(minInclusive: 0, maxExclusive: gruntSources.Length, excludedNumber: lastPlayed);
        foreach (var source in gruntSources)
        {
            source.Stop();
        }
        gruntSources[randomIndex].Play();
        lastPlayed = randomIndex;
    }
}
