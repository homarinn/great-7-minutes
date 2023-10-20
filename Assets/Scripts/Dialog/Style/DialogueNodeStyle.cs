using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

namespace Dialogue
{
    [System.Serializable]
    public class DialogueNodeStyle
    {
        public int speakerID;
        public string speakerName;

        [NonSerialized]
        public string backgroundPath;
        [NonSerialized]
        public float[] textColor;
        [NonSerialized]
        public int[] padding;
        [NonSerialized]
        public int[] border;

        public Texture2D Background;
        public Color TextColor;
        public RectOffset Padding;
        public RectOffset Border;

        public static GUIStyle Default()
        {
            DialogueNodeStyle nodeStyle = new DialogueNodeStyle();
            return nodeStyle.GetStyle();
        }

        public GUIStyle GetStyle()
        {
            GUIStyle style = new GUIStyle();
            style.normal.background = GetBackground();
            style.normal.textColor = GetTextColor();
            style.padding = GetPadding();
            style.border = GetBorder();
            return style;
        }

        private Texture2D GetBackground()
        {
            if (Background == null)
            {
                return EditorGUIUtility.Load("node0") as Texture2D;
            }

            return Background;
        }

        private Color GetTextColor()
        {
            if (TextColor == null)
            {
                return Color.white;
            }

            return TextColor;
        }


        private RectOffset GetPadding()
        {
            if (Padding == null)
            {
                return new RectOffset(20, 20, 20, 20);
            }

            return Padding;
        }


        private RectOffset GetBorder()
        {
            if (Border == null)
            {
                return new RectOffset(12, 12, 12, 12);
            }

            return Border;
        }

        private Color GetColor(float[] floats)
        {
            if (floats.Length == 3)
            {
                return new Color(floats[0], floats[1], floats[2]);
            }
            else
            {
                return new Color(floats[0], floats[1], floats[2], floats[3]);
            }
        }

        private RectOffset GetRectOffset(int[] ints)
        {
            return new RectOffset(ints[0], ints[1], ints[2], ints[3]);
        }

        public void SetProperties()
        {
            SetBacground(backgroundPath);
            SetTextColor(textColor);
            SetPadding(padding);
            SetBorder(border);
        }

        private void SetBacground(string path)
        {
            if (path != null && path != "")
            {
                Background = EditorGUIUtility.Load(backgroundPath) as Texture2D;
            }
        }

        private void SetTextColor(float[] floats)
        {
            if (floats != null && floats.Length != 0)
            {
                TextColor = GetColor(floats);
            }
        }

        public void SetPadding(int[] ints)
        {
            if (ints != null && ints.Length != 0)
            {
                Padding = GetRectOffset(ints);
            }
        }

        public void SetBorder(int[] ints)
        {
            if (ints != null && ints.Length != 0)
            {
                Border = GetRectOffset(ints);
            }
        }
    }
}
