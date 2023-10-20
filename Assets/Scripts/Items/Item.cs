using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Item", menuName = "Item")]
public class Item : ScriptableObject
{

    public string Name;

    public enum Type
    {
        SKILLITEM,
    }

    public Type type;
}
