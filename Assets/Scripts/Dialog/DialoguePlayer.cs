using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking.Types;

namespace Dialogue
{
    public class DialoguePlayer : MonoBehaviour
    {
        public string ID { get; set; }

        Dialogue dialogue;
        DialogueNode currentDialogueNode;
        DialogueUI dialogueUI;

        const float showCharInterval = 0.05f;
        const float showTextTimeMagnification = 0.02f;

        bool isShowTextInProgress = false;
        float showingCurrentTextTime = 0f;
        int currentTextCharacterCount = 0;
        float showCurretTextTimeWhenAuto = 0f;

        bool isPlayed = false;

        public UnityEvent onFinishEvent;

        private void OnEnable()
        {
            onFinishEvent = new UnityEvent();
        }

        // Update is called once per frame
        void Update()
        {
            if (!isPlayed)
            {
                return;
            }

            if (dialogue.IsAutoPlay)
            {
                showingCurrentTextTime += Time.deltaTime;
                if (showingCurrentTextTime >= showCurretTextTimeWhenAuto)
                {
                    Next();
                }
            } else if (Input.GetKeyDown(KeyCode.Return))
            {
                Next();
            }
        }

        public void Play()
        {
            if (dialogue == null || dialogueUI == null)
            {
                return;
            }
                
            DialogueManager.instance.IsPlaying = true;

            SetCurrentDialogueNode(dialogue.GetRootNode());
            ShowDialogueUI();
            isPlayed = true;
        }

        private void OnDestroy()
        {
            DialogueManager.instance.RemoveDialoguePlayer(this);
        }

        public void SetDialogue(Dialogue newDialogue)
        {
            dialogue = newDialogue;
        }

        public void SetDialogueUI(DialogueUI newDialogueUI)
        {
            dialogueUI = newDialogueUI;
            dialogueUI.CurrentDialoguePlayer = this;
            dialogueUI.gameObject.SetActive(true);
        }

        private void ShowDialogueUI()
        {
            dialogueUI.ShowSpeaker(currentDialogueNode.GetSpeakerName());
            ShowTextInDialogueUI();
        }

        private void ShowTextInDialogueUI()
        {
            string text = currentDialogueNode.GetText();

            if (dialogue.IsCharByChar)
            {
                StartCoroutine(ShowTextCharacterByCharacter(text));
            } else
            {
                dialogueUI.ShowText(text);
            }
        }

        private IEnumerator ShowTextCharacterByCharacter(string text)
        {
            int characterIndex = 0;

            isShowTextInProgress = true;
            while (isShowTextInProgress && characterIndex <= currentTextCharacterCount)
            {
                dialogueUI.ShowText(text.Substring(0, characterIndex));
                characterIndex++;

                yield return new WaitForSeconds(showCharInterval);
            }
            isShowTextInProgress = false;
        }

        public void Next()
        {
            string nextDialogueNodeID = currentDialogueNode.GetChild();

            if (nextDialogueNodeID == null)
            {
                OnFinish();
                return;
            }

            SetCurrentDialogueNode(dialogue.GetDialogueNode(nextDialogueNodeID));

            ShowDialogueUI();
        }

        private void SetCurrentDialogueNode(DialogueNode dialogueNode)
        {
            currentDialogueNode = dialogueNode;
            currentTextCharacterCount = currentDialogueNode.GetText().Length;

            if (dialogue.IsAutoPlay)
            {
                showingCurrentTextTime = 0f;
                showCurretTextTimeWhenAuto = Dialogue.AutoPlayInterval + currentTextCharacterCount * showTextTimeMagnification;
            }
        }

        private void OnFinish()
        {
            dialogueUI.CurrentDialoguePlayer = null;
            dialogueUI.RestText();
            dialogueUI.gameObject.SetActive(false);
            onFinishEvent.Invoke();

            DialogueManager.instance.IsPlaying = false;

            Destroy(gameObject);
        }
    }

}