using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dialogue
{
    [System.Serializable]
    public class DialogueNode : ScriptableObject
    {
        [SerializeField]
        int speakerID;
        [SerializeField]
        string speakerName;
        [SerializeField, TextArea]
        string text;
        [SerializeField]
        List<string> children = new List<string>();
        [SerializeField]
        Rect rect = new Rect(0, 0, 200, 200);

        GUIStyle style;

        public Vector2 TextScroll { get; set; } = Vector2.zero;
        public bool CanDrag { get; set; } = true;

        private void Awake()
        {
            if (style == null)
            {
                style = DialogueNodeStyle.Default();
            }
        }

        private void OnValidate()
        {
            SetStyle();
        }

        public int GetSpeakerID()
        {
            return speakerID;
        }

        public string GetSpeakerName()
        {
            return speakerName;
        }

        public string GetText()
        {
            return text;
        }

        public List<string> GetChildren()
        {
            return children;
        }

        // 急遽用意
        // 選択肢や分岐など今回は用意しないため
        public string GetChild()
        {
            if (children.Count == 0)
            {
                return null;
            } else
            {
                return children[0];
            }
        }

        public bool HasChild(DialogueNode childNode)
        {
            return children.Contains(childNode.name);
        }

        public Rect GetRect()
        {
            return rect;
        }

        public GUIStyle GetStyle()
        {
            return style;
        }

#if UNITY_EDITOR
        public static DialogueNode Create(DialogueNode parentNode)
        {
            DialogueNode newNode = Create();
            if (parentNode != null)
            {
                newNode.rect.position = new Vector2(parentNode.GetRect().x + 200, parentNode.GetRect().y + parentNode.GetChildren().Count * 100);
                parentNode.children.Add(newNode.name); ;
            }
            return newNode;
        }

        public static DialogueNode Create()
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            return newNode;
        }

        public void SetSpeakerID(int newSpeakerID)
        {
            if (speakerID == newSpeakerID)
            {
                return;
            }

            Undo.RecordObject(this, "Update Dialogue Node Speaker ID");
            speakerID = newSpeakerID;
            SetStyle();
            EditorUtility.SetDirty(this);
        }

        public void SetSpeakerName(string newSpeakerName)
        {
            if (speakerName == newSpeakerName)
            {
                return;
            }

            Undo.RecordObject(this, "Update Dialogue Node Speaker Name");
            speakerName = newSpeakerName;
            EditorUtility.SetDirty(this);
        }

        public void SetText(string newText)
        {
            if (text == newText)
            {
                return;
            }

            Undo.RecordObject(this, "Update Dialogue Text");
            text = newText;
            EditorUtility.SetDirty(this);
        }

        public void AddChild(DialogueNode childNode)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(childNode.name);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(DialogueNode childNode)
        {
            if (HasChild(childNode))
            {
                Undo.RecordObject(this, "Remove Dialogue Link");
                children.Remove(childNode.name);
                EditorUtility.SetDirty(this);
            }
        }

        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }

        public void SetStyle()
        {
            DialogueNodeStyle nodeStyle = DialogueNodeStyleDB.Find(speakerID);

            if (nodeStyle == null)
            {
                style = DialogueNodeStyle.Default();
            }
            else
            {
                style = nodeStyle.GetStyle();
            }
        }
#endif
    }
}
