using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        public const float AutoPlayInterval = 3f;

        [SerializeField]
        List<DialogueNode> nodes = new List<DialogueNode>();

        public bool IsCharByChar = false;
        public bool IsAutoPlay = false;

        Dictionary<string, DialogueNode> nodeLookUp = new Dictionary<string, DialogueNode>();

        private void OnValidate()
        {
            nodeLookUp.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                if (node != null)
                {
                    nodeLookUp[node.name] = node;
                }
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            if (nodes.Count == 0)
            {
                return null;
            } else
            {
                return nodes[0];
            }
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            List<DialogueNode> result = new List<DialogueNode>();

            foreach (string childID in parentNode.GetChildren())
            {
                if (nodeLookUp.ContainsKey(childID))
                {
                    yield return nodeLookUp[childID];
                }
            }
        }

        public DialogueNode GetDialogueNode(string nodeID)
        {
            if (nodeID == null || nodeID == "")
            {
                return null;
            }

            if (nodeLookUp.ContainsKey(nodeID))
            {
                return nodeLookUp[nodeID];
            } else
            {
                return null;
            }
        }

#if UNITY_EDITOR
        public void CreateNode(DialogueNode parentNode)
        {
            DialogueNode newNode = DialogueNode.Create(parentNode);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialogue Node");
            AddNode(newNode);
        }

        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialogue Node");
            CleanDanglingChildren(nodeToDelete);
            nodes.Remove(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
            OnValidate();
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete);
            }
        }
#endif
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                DialogueNode newNode = DialogueNode.Create();
                AddNode(newNode);
            }

            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if (node != null)
                    {
                        if (AssetDatabase.GetAssetPath(node) == "")
                        {
                            AssetDatabase.AddObjectToAsset(node, this);
                        }
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}
