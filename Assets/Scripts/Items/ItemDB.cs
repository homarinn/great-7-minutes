using System.Collections;
using System.Collections.Generic;
using Dialogue;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDB", menuName = "ItemDB")]
public class ItemDB : ScriptableObject
{
    [SerializeField]
    List<Item> items = new List<Item>();
    readonly Dictionary<string, Item> itemLookUp = new Dictionary<string, Item>();

    public static ItemDB instance;

    private void OnEnable()
    {
        foreach(Item item in items)
        {
            itemLookUp[item.name] = item;
        }

        instance = this;
    }

    public Dictionary<string, Item> GetItems()
    {
        return itemLookUp;
    }
}
