using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        Dictionary<string, DialoguePlayer> dialoguePlayers = new Dictionary<string, DialoguePlayer>();

        Transform transformCache;

        [SerializeField]
        DialogueUI dialogueUI;

        public static DialogueManager instance;

        public Dialogue startDialogue;

        public bool IsPlaying { get; set; } = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                CreateDialoguePlayer(startDialogue).Play();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            transformCache = transform;
        }

        public DialoguePlayer CreateDialoguePlayer(Dialogue dialogue)
        {
            string name = dialogue.name;

            if (!dialoguePlayers.ContainsKey(name))
            {
                GameObject obj = new GameObject(name);
                obj.transform.parent = transformCache;
                DialoguePlayer dialoguePlayer = obj.AddComponent(typeof(DialoguePlayer)) as DialoguePlayer;
                dialoguePlayer.ID = name;
                dialoguePlayer.SetDialogue(dialogue);
                dialoguePlayer.SetDialogueUI(dialogueUI);

                // どんな時もリセットをかけるように
                // TODO: 修正したい
                foreach (KeyValuePair<string, DialoguePlayer> usingDialoguePlayer in dialoguePlayers)
                {
                    Destroy(usingDialoguePlayer.Value.gameObject);
                }

                dialoguePlayers[name] = dialoguePlayer;

                return dialoguePlayer;
            }

            return null;
        }

        public void RemoveDialoguePlayer(DialoguePlayer dialoguePlayer)
        {
            dialoguePlayers.Remove(dialoguePlayer.ID);
        }
    }
}
