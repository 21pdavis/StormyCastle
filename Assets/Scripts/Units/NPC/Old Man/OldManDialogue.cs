using UnityEngine;

[CreateAssetMenu(fileName = "OldManDialogue", menuName = "ScriptableObjects/OldManDialogue", order = 1)]
public class OldManDialogue : ScriptableObject
{
    [TextArea(2, 10)]
    public string[] dialogueLines;
}
