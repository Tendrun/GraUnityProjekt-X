using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ItemPanel : MonoBehaviour
{
    [SerializeField]
    List<GameObject> items = new List<GameObject>();


    private void OnEnable()
    {
        Dictionary<int, Item> itemsList = new Dictionary<int, Item>(Player_Script.PlayerInstance.ShowItems());

        for (int i = 0; i < itemsList.Count; i++)
        {
            GameObject obj = Instantiate(new GameObject("Item " + i), gameObject.transform);
            obj.AddComponent<Image>();
            obj.GetComponent<Image>().sprite = itemsList[i].ItemSprite;
            ItemDescription ItemDesc = obj.AddComponent<ItemDescription>();
            ItemDesc.SetItemDescription(itemsList[i].Descrpiton, obj);
            items.Add(obj);
        }
    }

    private void OnDisable()
    {
        if (items.Count == 0) return;

        for (int i = 0; i < items.Count; i++)
        {
            Destroy(items[i]);
        }        
        items.Clear();
    }
}
