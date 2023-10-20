using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

namespace Dialogue
{
    public class DialogueNodeStyleDB : ScriptableObject
    {
        public const string CSVFilename = "DialogueNodeStyleList";
        public const string DBName = "DialogueNodeStyleDB";

        [SerializeField]
        DialogueNodeStyle[] nodeStyles;

        static Dictionary<int, DialogueNodeStyle> nodeStyleLookUp = new Dictionary<int, DialogueNodeStyle>();
        static bool isLoaded = false;

        public void SetNodeStyles(DialogueNodeStyle[] newNodeStyles)
        {
            nodeStyles = newNodeStyles;
            SetNodeStyleLookUp();
        }

        public static DialogueNodeStyle Find(int speakerID)
        {
            if (!isLoaded)
            {
                isLoaded = true;
                DialogueNodeStyleDB db = Resources.Load(DBName) as DialogueNodeStyleDB;
                if (db == null)
                {
                    return null;
                }
                db.SetNodeStyleLookUp();
            }
            return nodeStyleLookUp.ContainsKey(speakerID) ? nodeStyleLookUp[speakerID] : null;
        }

        private void SetNodeStyleLookUp()
        {
            foreach (DialogueNodeStyle nodeStyle in nodeStyles)
            {
                nodeStyleLookUp[nodeStyle.speakerID] = nodeStyle;
            }
        }
    }
}
