using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [SerializeField]
    Inventory inventory;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static Inventory GetInventory()
    {
        return instance?.inventory;
    }

    public void AddItemToInventory(Item item)
    {
        inventory.AddItem(item);
    }
}
