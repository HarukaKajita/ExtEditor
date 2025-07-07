using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

// ScriptableObject to store renaming patterns and settings
public class BatchRenamer : ScriptableObject
{
    public string prefix = "";
    public string suffix = "";
    public string searchString = "";
    public string replacementString = "";
}

// Custom editor window for batch renaming
public class BatchRenamerEditor : EditorWindow
{
    private BatchRenamer renamer;
    private Vector2 scrollPosition;

    [MenuItem("Tools/Batch Renamer")]
    public static void ShowWindow()
    {
        GetWindow<BatchRenamerEditor>("Batch Renamer");
    }

    private void OnEnable()
    {
        // Try to load existing settings or create new ones
        renamer = AssetDatabase.LoadAssetAtPath<BatchRenamer>("Assets/ExtEditor/Editor/BatchRename/BatchRenamerSettings.asset");
        if (renamer == null)
        {
            renamer = CreateInstance<BatchRenamer>();
            // It's good practice to save the ScriptableObject asset if you want to persist its settings
            // For now, we'll just keep it in memory until the editor decides to save it or the user explicitly saves it.
        }
    }

    private void OnGUI()
    {
        if (renamer == null)
        {
            EditorGUILayout.HelpBox("BatchRenamer settings could not be loaded or created.", MessageType.Error);
            if (GUILayout.Button("Try Re-initialize"))
            {
                OnEnable(); // Attempt to re-initialize
            }
            return;
        }

        GUILayout.Label("Batch Renaming Settings", EditorStyles.boldLabel);

        renamer.prefix = EditorGUILayout.TextField("Prefix", renamer.prefix);
        renamer.suffix = EditorGUILayout.TextField("Suffix", renamer.suffix);
        renamer.searchString = EditorGUILayout.TextField("Search String", renamer.searchString);
        renamer.replacementString = EditorGUILayout.TextField("Replacement String", renamer.replacementString);

        EditorGUILayout.Space();
        GUILayout.Label("Preview (Selected Assets):", EditorStyles.boldLabel);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150));
        Object[] selectedAssets = Selection.objects;
        if (selectedAssets.Length == 0)
        {
            EditorGUILayout.HelpBox("Select assets in the Project window to see a preview.", MessageType.Info);
        }
        else
        {
            foreach (Object obj in selectedAssets)
            {
                string originalName = obj.name;
                string path = AssetDatabase.GetAssetPath(obj);
                string currentExtension = Path.GetExtension(path);
                string nameWithoutExtension = Path.GetFileNameWithoutExtension(path);

                string newName = nameWithoutExtension;

                if (!string.IsNullOrEmpty(renamer.searchString))
                {
                    newName = newName.Replace(renamer.searchString, renamer.replacementString);
                }
                newName = renamer.prefix + newName + renamer.suffix + currentExtension;

                EditorGUILayout.LabelField(originalName + "  ->  " + newName);
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        if (GUILayout.Button("Apply Renaming to Selected Assets"))
        {
            if (selectedAssets.Length > 0)
            {
                ApplyRenaming(selectedAssets);
            }
            else
            {
                Debug.LogWarning("No assets selected to rename.");
            }
        }
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Save Settings"))
        {
            SaveSettings();
        }
    }

    private void ApplyRenaming(Object[] assetsToRename)
    {
        AssetDatabase.StartAssetEditing();
        try
        {
            foreach (Object obj in assetsToRename)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                string originalName = obj.name;
                string currentExtension = Path.GetExtension(path);
                string nameWithoutExtension = Path.GetFileNameWithoutExtension(path);

                string newName = nameWithoutExtension;

                if (!string.IsNullOrEmpty(renamer.searchString))
                {
                    newName = newName.Replace(renamer.searchString, renamer.replacementString);
                }
                newName = renamer.prefix + newName + renamer.suffix; // Extension will be added by AssetDatabase.RenameAsset

                if (string.IsNullOrEmpty(newName.Trim()))
                {
                    Debug.LogError($"New name for asset '{originalName}' would be empty or whitespace. Skipping rename.");
                    continue;
                }
                
                string errorMessage = AssetDatabase.RenameAsset(path, newName);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Debug.LogError($"Error renaming asset {originalName}: {errorMessage}");
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
        }
        Debug.Log("Renaming process completed for selected assets.");
    }

    private void SaveSettings()
    {
        string path = "Assets/ExtEditor/Editor/BatchRename/BatchRenamerSettings.asset";
        BatchRenamer existingAsset = AssetDatabase.LoadAssetAtPath<BatchRenamer>(path);
        if (existingAsset == null)
        {
            AssetDatabase.CreateAsset(renamer, path);
        }
        else
        {
            EditorUtility.CopySerialized(renamer, existingAsset);
            AssetDatabase.SaveAssets();
        }
        AssetDatabase.Refresh();
        Debug.Log("Batch Renamer settings saved to " + path);
    }
}
