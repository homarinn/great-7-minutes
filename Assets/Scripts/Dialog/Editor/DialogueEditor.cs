using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        const float canvasSize = 4000;
        const float backgroundSize = 50;

        Dialogue selectedDialogue;
        [NonSerialized]
        DialogueNode draggingNode;
        [NonSerialized]
        Vector2 draggingNodeOffset;
        [NonSerialized]
        DialogueNode creatingNode;
        [NonSerialized]
        DialogueNode deletingNode;
        [NonSerialized]
        DialogueNode linkingParentNode;
        Vector2 scrollPosition;
        [NonSerialized]
        bool isDraggingCanvas = false;
        [NonSerialized]
        Vector2 draggingCanvasOffset;

        bool IsDraggingNode { get { return draggingNode != null; } }
        bool IsRepaint { get { return Event.current.type == EventType.Repaint; } }
        bool IsHoverLastLayout { get { return GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition); } }

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;

            if (dialogue != null)
            {
                ShowEditorWindow();
                return true;
            }

            return false;
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected");
            }
            else
            {
                if (DialogueNodeStyleListCSVImporter.IsImporting)
                {
                    return;
                }

                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);
                Texture2D backgroundTex = Resources.Load("background") as Texture2D;
                Rect textCoords = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTex, textCoords);

                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }

                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();

                if (creatingNode != null)
                {
                    selectedDialogue.CreateNode(creatingNode);
                    DrawConnections(creatingNode);
                    creatingNode = null;
                }

                if (deletingNode != null)
                {
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.x *= 0.8f;
                controlPointOffset.y = 0;

                Handles.DrawBezier(
                    startPosition, endPosition,
                    startPosition + controlPointOffset,
                    endPosition - controlPointOffset,
                    Color.white, null, 4f);
            }
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && !IsDraggingNode)
            {
                Vector2 draggingStartPoint = Event.current.mousePosition + scrollPosition;
                draggingNode = GetNodeAtPoint(draggingStartPoint);

                if (IsDraggingNode)
                {
                    if (!draggingNode.CanDrag)
                    {
                        draggingNode = null;
                        return;
                    }

                    draggingNodeOffset = draggingNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                }
                else
                {
                    isDraggingCanvas = true;
                    draggingCanvasOffset = draggingStartPoint;
                    Selection.activeObject = selectedDialogue;
                }

            }
            else if (Event.current.type == EventType.MouseDrag && IsDraggingNode)
            {
                draggingNode.SetPosition(Event.current.mousePosition + draggingNodeOffset);

                GUI.changed = true;
                Repaint();
            }
            else if (Event.current.type == EventType.MouseDrag && isDraggingCanvas)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
                Repaint();
            }
            else if (Event.current.type == EventType.MouseUp && IsDraggingNode)
            {
                draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && isDraggingCanvas)
            {
                isDraggingCanvas = false;
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.GetRect().Contains(point))
                {
                    foundNode = node;
                }
            }
            return foundNode;
        }

        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.GetRect(), node.GetStyle());
            EditorGUI.BeginChangeCheck();

            EditorGUIUtility.labelWidth = 40;
            node.SetSpeakerID(EditorGUILayout.DelayedIntField("ID:", node.GetSpeakerID()));
            UpdateNodeCanDrag(node);

            node.SetSpeakerName(EditorGUILayout.DelayedTextField("Name:", node.GetSpeakerName()));
            if (node.CanDrag)
            {
                UpdateNodeCanDrag(node);
            }

            EditorGUILayout.LabelField("Text:");
            node.TextScroll = EditorGUILayout.BeginScrollView(node.TextScroll, GUILayout.Height(75));

            GUIStyle style = new GUIStyle(EditorStyles.textArea);
            style.wordWrap = true;
            node.SetText(EditorGUILayout.TextArea(node.GetText(), style, GUILayout.Height(150)));
            if (node.CanDrag)
            {
                UpdateNodeCanDrag(node);
            }
            EditorGUILayout.EndScrollView();
            if (node.CanDrag)
            {
                UpdateNodeCanDrag(node);
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("x"))
            {
                deletingNode = node;
            }

            DrawLinkButtons(node);

            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("link"))
                {
                    linkingParentNode = node;
                }
            }
            else if (linkingParentNode == node)
            {
                if (GUILayout.Button("cancel"))
                {
                    linkingParentNode = null;
                }
            }
            else if (linkingParentNode.HasChild(node))
            {
                if (GUILayout.Button("unlink"))
                {
                    linkingParentNode.RemoveChild(node);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("child"))
                {
                    linkingParentNode.AddChild(node);
                    linkingParentNode = null;
                }
            }
        }

        private void UpdateNodeCanDrag(DialogueNode node)
        {
            if (IsRepaint)
            {
                if (IsHoverLastLayout)
                {
                    node.CanDrag = false;
                }
                else
                {
                    node.CanDrag = true;
                }
            }
        }
    }

}
