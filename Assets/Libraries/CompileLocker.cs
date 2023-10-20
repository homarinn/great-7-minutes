using UnityEngine;
using UnityEditor;

/// <summary>
/// This editor utility can lock/unlock unity script compile from menu item.
/// See more https://raspberly.hateblo.jp/entry/InvalidateUnityCompile
/// </summary>
public static class CompileLocker
{
    [MenuItem("Compile/Lock %l", false, 1)]
    static void Lock()
    {

        var menuPath = "Compile/Lock";
        var isLocked = Menu.GetChecked(menuPath);

        if (isLocked)
        {
            Debug.Log("Compile Unlocked");
            EditorApplication.UnlockReloadAssemblies();
            Menu.SetChecked(menuPath, false);
        }
        else
        {
            Debug.Log("Compile Locked");
            EditorApplication.LockReloadAssemblies();
            Menu.SetChecked(menuPath, true);
        }
    }
}