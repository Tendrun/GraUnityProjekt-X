using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDescription : MonoBehaviour
{
    public string text;
    public GameObject DescriptionText;

    //Text
    Text text1;
    GameObject ItemToDescribe;
    EventTrigger EventTrigger_;

    public void SetItemDescription(string text, GameObject ItemToDescribe)
    {
        this.text = text;
        this.ItemToDescribe = ItemToDescribe;
    }

    private void Start()
    {
        if (!EventTrigger_)
            EventTrigger_ = ItemToDescribe.AddComponent<EventTrigger>();

        //Event enter
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;

        entry.callback.AddListener((data) => { OnPointerEnter(); });
        EventTrigger_.triggers.Add(entry);

        //Event Exit
        EventTrigger.Entry Exit = new EventTrigger.Entry();

        Exit.eventID = EventTriggerType.PointerExit;
        Exit.callback.AddListener((data) => { OnPointerExit(); });
        EventTrigger_.triggers.Add(Exit);

    }

    public void OnPointerEnter()
    {
        DescriptionText = Instantiate(new GameObject("Descrpiton"), ItemToDescribe.transform);

        DescriptionText.AddComponent<Text>().raycastTarget = false;
        DescriptionText.GetComponent<Text>().text = text;
        DescriptionText.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        DescriptionText.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;

    }

    public void OnPointerExit()
    {
        Destroy(DescriptionText);
    }


    private void OnMouseOver()
    {
        Debug.Log("MouseOver");

        GameObject Description = Instantiate(new GameObject("Description"), ItemToDescribe.transform);

        Description.AddComponent<Text>().text = text;
    }

    private void OnMouseExit()
    {
        
    }
}
