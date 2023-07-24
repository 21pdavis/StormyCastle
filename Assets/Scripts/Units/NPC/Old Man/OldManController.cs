using TMPro;
using UnityEngine;

public class OldManController : NPCController
{
    [SerializeField] private OldManDialogue dialogue;

    private GameObject canvas;
    private TextMeshProUGUI textMeshPro;

    private int currentLine = 0;

    // TODO: bobbing animation on prompt
    private void Start()
    {
        canvas = transform.Find("Canvas").gameObject;
        textMeshPro = canvas.transform.Find("Dialogue").GetComponent<TextMeshProUGUI>();
    }

    public override void OnInteract()
    {
        if (!canvas.activeSelf)
        {
            canvas.SetActive(true);
        }

        if (currentLine == dialogue.dialogueLines.Length)
        {
            currentLine = 0;
            canvas.SetActive(false);
            return;
        }

        textMeshPro.text = dialogue.dialogueLines[currentLine];
        currentLine++;
    }
}
