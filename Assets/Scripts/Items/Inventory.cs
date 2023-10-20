using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[CreateAssetMenu(fileName = "Inventory", menuName = "Inventory"), System.Serializable]
public class Inventory : ScriptableObject
{
    [SerializeField]
    Dictionary<string, Item> items = new Dictionary<string, Item>();
    [SerializeField]
    Dictionary<string, int> itemCountTable = new Dictionary<string, int>();

    public void AddItem(Item item) 
    {
        if (items.ContainsKey(item.name))
        {
            itemCountTable[item.name] += 1;
        } else
        {
            items[item.name] = item;
            itemCountTable[item.name] = 1;
        }
    }

    public void AddItem(Item item, int count)
    {
        if (items.ContainsKey(item.name))
        {
            itemCountTable[item.name] += count;
        }
        else
        {
            items[item.name] = item;
            itemCountTable[item.name] = count;
        }
    }

    public void UseItem(Item item)
    {
        if (!items.ContainsKey(item.name))
        {
            return;
        }

        itemCountTable[item.name] -= 1;

        if (itemCountTable[item.name] == 0)
        {
            items.Remove(item.name);
            itemCountTable.Remove(item.name);
        }
    }

    public void UseItem(Item item, int count)
    {
        if (!items.ContainsKey(item.name))
        {
            return;
        }

        itemCountTable[item.name] -= count;

        if (itemCountTable[item.name] <= 0)
        {
            items.Remove(item.name);
            itemCountTable.Remove(item.name);
        }
    }

    public Item FindItem(string itemName)
    {
        return items.ContainsKey(itemName) ? items[itemName] : null;
    }

    public int GetCount(Item item)
    {
        if (itemCountTable.ContainsKey(item.name))
        {
            return itemCountTable[item.name];
        } else
        {
            return 0;
        }
    }
}
