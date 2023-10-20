using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

namespace Dialogue
{
#if UNITY_EDITOR
    public class DialogueNodeStyleListCSVImporter : AssetPostprocessor
    {
        public static bool IsImporting { get; set; } = false;

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string filepath in importedAssets)
            {
                if (filepath.IndexOf($"/{DialogueNodeStyleDB.CSVFilename}.csv") != -1)
                {
                    IsImporting = true;
                    TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(filepath);
                    string assetfile = filepath.Replace($"/{DialogueNodeStyleDB.CSVFilename}.csv", $"/{DialogueNodeStyleDB.DBName}.asset");
                    DialogueNodeStyleDB db = AssetDatabase.LoadAssetAtPath<DialogueNodeStyleDB>(assetfile);

                    if (db == null)
                    {
                        db = ScriptableObject.CreateInstance<DialogueNodeStyleDB>();
                        AssetDatabase.CreateAsset(db, assetfile);
                    }

                    DialogueNodeStyle[] nodeStyles = CSVSerializer.Deserialize<DialogueNodeStyle>(textAsset.text);
                    foreach (DialogueNodeStyle nodeStyle in nodeStyles)
                    {
                        nodeStyle.SetProperties();
                    }
                    db.SetNodeStyles(nodeStyles);
                    EditorUtility.SetDirty(db);
                    AssetDatabase.SaveAssets();
                    IsImporting = false;

                    return;
                }
            }
        }
    }
#endif
}
