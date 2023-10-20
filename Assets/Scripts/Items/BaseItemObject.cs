using System.Collections;
using System.Collections.Generic;
using Dialogue;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

abstract public class BaseItemObject : MonoBehaviour
{
    [SerializeField]
    protected Item item;

    bool IsNearPlayer { get; set; }
    bool IsInsideCamera { get; set; }

    private bool IsGot = false;

    protected abstract void OnGet(Item i);

    private PlayerManager player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        IsNearPlayer = IsNearPlayer || other.CompareTag("Player");
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsGot && IsNearPlayer && IsInsideCamera && player.CanGetSkill && !DialogueManager.instance.IsPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                IsGot = true;
                OnGet(item);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsNearPlayer = false;
        }
    }

    private void OnBecameVisible()
    {
        IsInsideCamera = true;
    }

    private void OnBecameInvisible()
    {
        IsInsideCamera = false;
    }
}
