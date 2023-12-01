using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;

    private LinkedList<Sentence> sentences;
    bool IsChoosing = false;
    [SerializeField]
    GameObject OptionSorter, OptionGameObj;

    public List<Interactable_Object> InteractableObjects;

    public List<GameObject> OldOptions;

    Interactable_Object Inter_Obj;

    #region Singleton

    private static DialogueManager _GameDialogueManagerInstance;
    public static DialogueManager GameDialogueManagerInstance
    {
        get
        {
            if (_GameDialogueManagerInstance == null)
            {
                _GameDialogueManagerInstance = FindObjectOfType<DialogueManager>();
                if (_GameDialogueManagerInstance == null)
                {
                    GameObject newGO = new GameObject("GameManagerObj");
                    _GameDialogueManagerInstance = newGO.AddComponent<DialogueManager>();
                }
                return _GameDialogueManagerInstance;
            }
            return _GameDialogueManagerInstance;
        }
    }


    #endregion 

    // Start is called before the first frame update
    void Start()
    {
        sentences = new LinkedList<Sentence>();
    }


    public void StartDialogue(Dialogue dialogue, Text NameText, Text DialogueText, GameObject OptionSorter, GameObject OptionGameObj, Interactable_Object Inter_Obj)
    {
        
        if (Inter_Obj.ConversationEnd)
        {   //Delete from list
            //InteractableObjects.RemoveAt();

            Debug.Log("Start Wave");
            Player_Script.PlayerInstance.EndConversation();
            FindObjectOfType<WaveSpawner>().EndShoping();
            return;
        }

        this.Inter_Obj = Inter_Obj;
        nameText = NameText;
        dialogueText = DialogueText;

        nameText.text = dialogue.Name;

        this.OptionSorter = OptionSorter;
        this.OptionGameObj = OptionGameObj;

        sentences.Clear();

        foreach(Sentence sentence in dialogue.sentences)
        {
            sentences.AddLast(sentence);
        }

        DisplayNextSentence();
    }


    public void DisplayNextSentence(ref bool IsTalking)
    {
        if (IsChoosing)
            return;

        if (sentences.Count == 0)
        {
            EndDialogue();
            IsTalking = false;
            Inter_Obj.ConversationEnd = true;
            return;
        }



        //deqeue
        Sentence sentence = sentences.First.Value;
        sentences.RemoveFirst();


        dialogueText.text = sentence.sentence;

        if (sentence.Options.Length > 0)
        {
            IsChoosing = true;


            foreach (Option option in sentence.Options)
            {
                GameObject obj = Instantiate(OptionGameObj, OptionSorter.transform);
                obj.GetComponent<Button>().onClick.AddListener(delegate { option.Action(optionaction); });
                obj.GetComponentInChildren<Text>().text = option.sentence;


                OldOptions.Add(obj);
            }
        }
    }

    public void DisplayNextSentence()
    {
        if (IsChoosing)
            return;

        if (sentences.Count == 0)
        {
            EndDialogue();
            //IsTalking = false;
            //ConversationEnd = true;
            return;
        }

        //deqeue
        Sentence sentence = sentences.First.Value;
        sentences.RemoveFirst();

        dialogueText.text = sentence.sentence;

        if (sentence.Options.Length > 0)
        {
            IsChoosing = true;


            foreach (Option option in sentence.Options)
            {
                GameObject obj = Instantiate(OptionGameObj, OptionSorter.transform);
                obj.GetComponent<Button>().onClick.AddListener(delegate { option.Action(optionaction); });
                obj.GetComponentInChildren<Text>().text = option.sentence;


                OldOptions.Add(obj);
            }
        }
    }

    public delegate void OptionAction(Sentence[] sentences_);
    void optionaction(Sentence[] sentences_)
    {
        if (sentences_.Length > 0)
        {
            //Add sentence
            foreach (var sentence_ in sentences_)
            {
                sentences.AddFirst(sentence_);
                //sentences.Enqueue(sentence_);
            }
        }

        foreach (GameObject OldOption in OldOptions)
        {
            Destroy(OldOption);
        }

        OldOptions.Clear();

        IsChoosing = false;
        DisplayNextSentence();
    }


    public void EndDialogue()
    {
        dialogueText.text = "CLICK E IF YOU WANT TO START NEXT WAVE";
        nameText.text = null;
        Player_Script.PlayerInstance.EndConversation();
    }

    public void LeaveConversation()
    {
        foreach (GameObject OldOption in OldOptions)
        {
            Destroy(OldOption);
        }

        OldOptions.Clear();

        foreach (var sentence in sentences)
        {
            //sentences.RemoveFirst();
        }
        dialogueText.text = null;
        nameText.text = null;

        IsChoosing = false;
        nameText = null;
        dialogueText = null;
        OptionSorter = null;
        OptionGameObj = null;
    }
}
