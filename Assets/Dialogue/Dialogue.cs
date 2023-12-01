using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue : MonoBehaviour
{
    public string Name;

    [SerializeField]
    public Sentence[] sentences;
}


[System.Serializable]
public class Sentence
{
    [TextArea(3, 10)]
    public string sentence;
    public Option[] Options;


}
[System.Serializable]
public class Option
{
    public string sentence;

    public Sentence[] AddSentences;

    public void Action(DialogueManager.OptionAction optionaction)
    {
        optionaction(AddSentences);
    }
}