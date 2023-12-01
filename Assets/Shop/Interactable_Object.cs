using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Interactable_Object : MonoBehaviour
{
    DialogueManager DialogMan;

    [SerializeField]
    Dialogue dialogue;

    public Text nameText, dialogueText;
    public GameObject OptionSorter, OptionGameObj;

    public bool ConversationEnd;

    private void Start()
    {
        DialogMan = DialogueManager.GameDialogueManagerInstance;
        DialogMan.InteractableObjects.Add(GetComponent<Interactable_Object>());
    }

    public void StartDialogue()
    {
        DialogMan.StartDialogue(dialogue, nameText, dialogueText, OptionSorter, OptionGameObj, GetComponent<Interactable_Object>());
    }

    public void NextSentence(ref bool IsTalking)
    {
        DialogMan.DisplayNextSentence(ref IsTalking);
    }

    public void Leave()
    {
        DialogMan.LeaveConversation();
    }

    public void EndDialogue()
    {
        DialogMan.EndDialogue();
    }
}
