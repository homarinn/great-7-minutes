using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI AISpeaker;

    [SerializeField]
    TextMeshProUGUI AIText;

    public AudioClip messageSound;
    public AudioSource audioSource;

    public DialoguePlayer CurrentDialoguePlayer { get; set; }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowSpeaker(string name)
    {
        if (AISpeaker == null)
        {
            return;
        }

        AISpeaker.text = name;
    }

    public void ShowText(string text)
    {
        if (AIText == null || text == "")
        {
            return;
        }

        AIText.text = text;
        PlayMessageSound();
    }

    public void RestText()
    {
        AIText.text = "";
    }

    public void SetMessageSound(AudioClip audioClip)
    {
        messageSound = audioClip;
    }

    private void PlayMessageSound()
    {
        audioSource.PlayOneShot(messageSound);
    }
}
