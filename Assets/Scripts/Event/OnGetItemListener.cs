using System.Collections;
using System.Collections.Generic;
using Dialogue;
using UnityEngine;
using UnityEngine.Events;

public class OnGetItemListener : BaseGameEventListener<Item, OnGetItem, ItemUnityEvent> { }
[System.Serializable]
public class ItemUnityEvent : UnityEvent<Item> { };
