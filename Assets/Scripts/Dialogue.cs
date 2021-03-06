using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    [TextArea(9, 1)] [SerializeField] string[] dialogueText;

    // this is the text that will repeat if the player hasn't completed the quest
    [TextArea(9, 1)] [SerializeField] string repeatingText;

    // this is the text that will appear once the player complete's the quest
    [TextArea(9, 1)] [SerializeField] string successText;

    // Get methods for object information
    public string[] GetDialogueText() { return dialogueText; }
    public string GetRepeatingText() { return repeatingText; }
    public string GetSuccessText() { return successText; }


}
