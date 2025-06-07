using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace ExtEditor.SelectionDataEditor
{
    public class SelectionDataWindow : EditorWindow
    {
        private List<SelectionData> allSelectionDataAssets = new List<SelectionData>();
        private Vector2 scrollPositionList;
        private Vector2 scrollPositionDetails;
        private string searchText = "";
        private SelectionData selectedDataInWindow = null;

        // For creating combined SelectionData
        private List<SelectionData> sourcesForCombined = new List<SelectionData>();
        private CombineOperation currentCombineOperation = CombineOperation.Union;
        private CombinationMode currentCombinationMode = CombinationMode.Dynamic;
        private string newCombinedName = "New Combined Selection";
        private bool creatingCombined = false;


        [MenuItem("Tools/ExtEditor/Selection Data Window")]
        public static void ShowWindow()
        {
            GetWindow<SelectionDataWindow>("Selection Data");
        }

        void OnEnable()
        {
            RefreshSelectionDataList();
            EditorApplication.projectChanged += RefreshSelectionDataList;
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        void OnDisable()
        {
            EditorApplication.projectChanged -= RefreshSelectionDataList;
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        void OnUndoRedo()
        {
            RefreshSelectionDataList(); // Refresh list on undo/redo
            Repaint();
        }


        void RefreshSelectionDataList()
        {
            allSelectionDataAssets.Clear();
            string[] guids = AssetDatabase.FindAssets("t:SelectionData");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SelectionData data = AssetDatabase.LoadAssetAtPath<SelectionData>(path);
                if (data != null)
                {
                    allSelectionDataAssets.Add(data);
                }
            }
            allSelectionDataAssets = allSelectionDataAssets
                .OrderByDescending(sd => sd.isStarred)
                .ThenBy(sd => string.IsNullOrEmpty(sd.description) ? sd.name : sd.description)
                .ToList();
            Repaint();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Selection Data Management", EditorStyles.boldLabel);

            DrawToolbar();

            EditorGUILayout.BeginHorizontal(); // Main layout: List | Details

            DrawSelectionDataList();
            DrawDetailView();

            EditorGUILayout.EndHorizontal();

            if (creatingCombined)
            {
                DrawCreateCombinedPopup();
            }
        }

        void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("New Object-based", EditorStyles.toolbarButton))
            {
                CreateNewSelectionData(SelectionMode.Objects, null, CombinationMode.Static, CombineOperation.Union, "New Object Selection");
                creatingCombined = false;
            }
            if (GUILayout.Button("New Query-based", EditorStyles.toolbarButton))
            {
                CreateNewSelectionData(SelectionMode.Query, null, CombinationMode.Static, CombineOperation.Union, "New Query Selection");
                creatingCombined = false;
            }
            if (GUILayout.Button("New Combined...", EditorStyles.toolbarButton))
            {
                sourcesForCombined.Clear();
                newCombinedName = "New Combined Selection";
                creatingCombined = true; // Show the popup
            }
            GUILayout.FlexibleSpace();
            searchText = EditorGUILayout.TextField(searchText, EditorStyles.toolbarSearchField, GUILayout.Width(200));
            if (GUILayout.Button(EditorGUIUtility.IconContent("Refresh"), EditorStyles.toolbarButton, GUILayout.Width(30)))
            {
                RefreshSelectionDataList();
            }
            EditorGUILayout.EndHorizontal();
        }

        void DrawSelectionDataList()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.4f), GUILayout.ExpandHeight(true)); // List Pane
            scrollPositionList = EditorGUILayout.BeginScrollView(scrollPositionList, EditorStyles.helpBox);

            List<SelectionData> filteredList = string.IsNullOrEmpty(searchText)
                ? allSelectionDataAssets
                : allSelectionDataAssets.Where(sd =>
                    (string.IsNullOrEmpty(sd.description) ? sd.name : sd.description).ToLowerInvariant().Contains(searchText.ToLowerInvariant()) ||
                    sd.name.ToLowerInvariant().Contains(searchText.ToLowerInvariant())).ToList();

            foreach (SelectionData data in filteredList)
            {
                bool isSelected = selectedDataInWindow == data;
                GUIStyle itemStyle = isSelected ? new GUIStyle(EditorStyles.helpBox) { fontStyle = FontStyle.Bold } : new GUIStyle(EditorStyles.helpBox);
                if(isSelected) itemStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/projecttab_selected@2x.png") as Texture2D;


                EditorGUILayout.BeginHorizontal(itemStyle);
                string displayName = string.IsNullOrEmpty(data.description) ? data.name : data.description;
                GUILayout.Label(data.isStarred ? "★" : " ", GUILayout.Width(20));

                string typeIndicator = "";
                if (data.sourceSelectionData != null && data.sourceSelectionData.Count > 0) typeIndicator = "[C]";
                else if (data.selectionMode == SelectionMode.Query) typeIndicator = "[Q]";
                else typeIndicator = "[O]";
                GUILayout.Label(typeIndicator, GUILayout.Width(25));

                if (GUILayout.Button(displayName, EditorStyles.label))
                {
                    selectedDataInWindow = data;
                    EditorGUIUtility.PingObject(data); // Ping but don't make it active selection to keep window focus
                }

                if (GUILayout.Button("Apply", GUILayout.Width(60)))
                {
                    ApplySelectionFromWindow(data);
                }
                if (GUILayout.Button(EditorGUIUtility.IconContent("editicon.sml"), GUILayout.Width(30))) // Edit
                {
                    Selection.activeObject = data; // Select in project to show in Inspector
                    EditorGUIUtility.PingObject(data);
                }
                if (GUILayout.Button("Dup", GUILayout.Width(40)))
                {
                    DuplicateSelectionData(data);
                    break;
                }
                if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), GUILayout.Width(30))) // Delete
                {
                    if (EditorUtility.DisplayDialog("Delete Selection Data", $"Are you sure you want to delete '{displayName}'?", "Delete", "Cancel"))
                    {
                        DeleteSelectionData(data);
                        break;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        void DrawDetailView()
        {
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)); // Details Pane
            scrollPositionDetails = EditorGUILayout.BeginScrollView(scrollPositionDetails, EditorStyles.helpBox);

            EditorGUILayout.LabelField("Selected Item Details", EditorStyles.boldLabel);
            if (selectedDataInWindow != null)
            {
                EditorGUILayout.LabelField("Name/Desc:", selectedDataInWindow.description);
                 EditorGUI.BeginChangeCheck();
                bool newStarred = EditorGUILayout.Toggle("Starred:", selectedDataInWindow.isStarred);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(selectedDataInWindow, "Toggle Starred Status");
                    selectedDataInWindow.isStarred = newStarred;
                    EditorUtility.SetDirty(selectedDataInWindow);
                    RefreshSelectionDataList(); // To re-sort
                }

                EditorGUILayout.LabelField("Type:", selectedDataInWindow.selectionMode.ToString());

                if (selectedDataInWindow.sourceSelectionData != null && selectedDataInWindow.sourceSelectionData.Count > 0)
                {
                    EditorGUILayout.LabelField("Combination:", $"{selectedDataInWindow.combineOperation} ({selectedDataInWindow.combinationMode})");
                    EditorGUILayout.LabelField("Sources:", EditorStyles.miniBoldLabel);
                    foreach(var source in selectedDataInWindow.sourceSelectionData)
                    {
                        if(source != null) GUILayout.Label($"- {source.name}"); else GUILayout.Label("- <Missing Source>");
                    }
                }
                else if (selectedDataInWindow.selectionMode == SelectionMode.Query)
                {
                    EditorGUILayout.LabelField("Query:", selectedDataInWindow.unitySearchQuery);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Effective Objects:", EditorStyles.boldLabel);
                List<UnityEngine.Object> effectiveObjects = selectedDataInWindow.GetEffectiveSelectedObjects();
                if (effectiveObjects.Count == 0)
                {
                    EditorGUILayout.LabelField(" (No objects in selection)");
                }
                foreach (var obj in effectiveObjects)
                {
                    if (obj == null) continue;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(obj, obj.GetType(), false); // AllowSceneObjects true by default
                    if (GUILayout.Button("Select Only", GUILayout.Width(80)))
                    {
                        Undo.RecordObjects(Selection.objects, "Before Selecting Single Item");
                        Selection.objects = new UnityEngine.Object[] { obj };
                        EditorGUIUtility.PingObject(obj);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Select an item from the list to see details.", MessageType.Info);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        void DrawCreateCombinedPopup()
        {
            Rect popupRect = new Rect(position.width / 2 - 200, position.height / 2 - 150, 400, 300);
            GUILayout.BeginArea(popupRect, "Create Combined SelectionData", EditorStyles.helpBox); // helpBox provides a background

            EditorGUILayout.LabelField("Configure Combined SelectionData", EditorStyles.boldLabel);
            newCombinedName = EditorGUILayout.TextField("Name/Description:", newCombinedName);
            currentCombineOperation = (CombineOperation)EditorGUILayout.EnumPopup("Operation:", currentCombineOperation);
            currentCombinationMode = (CombinationMode)EditorGUILayout.EnumPopup("Mode:", currentCombinationMode);

            EditorGUILayout.LabelField("Select Source SelectionData Assets:", EditorStyles.boldLabel);
            // Simple list for adding sources - could be improved with a drag-and-drop area or object pickers
            for (int i = 0; i < sourcesForCombined.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                sourcesForCombined[i] = (SelectionData)EditorGUILayout.ObjectField(sourcesForCombined[i], typeof(SelectionData), false);
                if (GUILayout.Button("-", GUILayout.Width(25)))
                {
                    sourcesForCombined.RemoveAt(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add Source Slot"))
            {
                sourcesForCombined.Add(null);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Create Combined SelectionData", GUILayout.Height(30)))
            {
                List<SelectionData> validSources = sourcesForCombined.Where(s => s != null).ToList();
                if (validSources.Count > 0)
                {
                    CreateNewSelectionData(SelectionMode.Objects, validSources, currentCombinationMode, currentCombineOperation, newCombinedName);
                    creatingCombined = false; // Close popup
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Please add at least one valid source SelectionData.", "OK");
                }
            }
            if (GUILayout.Button("Cancel"))
            {
                creatingCombined = false; // Close popup
            }
            GUILayout.EndArea();
        }


        private void CreateNewSelectionData(SelectionMode mode, List<SelectionData> sources, CombinationMode combMode, CombineOperation combOp, string desc)
        {
            SelectionData newItem = CreateInstance<SelectionData>();
            Undo.RegisterCreatedObjectUndo(newItem, $"Create New {desc}");

            newItem.selectionMode = mode;
            newItem.description = desc;

            if (sources != null && sources.Count > 0)
            {
                newItem.sourceSelectionData.AddRange(sources);
                newItem.combinationMode = combMode;
                newItem.combineOperation = combOp;
                if (combMode == CombinationMode.Static)
                {
                    newItem.PopulateFromDynamicSources(); // This records its own undo and sets dirty
                }
            }
            else if (mode == SelectionMode.Objects)
            {
                if (Selection.objects.Length > 0)
                {
                    Undo.RecordObject(newItem, "Populate New SelectionData from Current");
                    newItem.selectedObjects.AddRange(Selection.objects.Where(o => o != newItem)); // Prevent self-reference
                }
            }

            string path = AssetDatabase.GenerateUniqueAssetPath($"Assets/ExtEditor/Editor/SelectionDataEditor/{SanitizeFileName(desc)}.asset");
            AssetDatabase.CreateAsset(newItem, path);
            EditorUtility.SetDirty(newItem); // Ensure all changes are marked
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(); // Important to make the new asset visible

            RefreshSelectionDataList();
            selectedDataInWindow = newItem; // Select the new item in our window
            Selection.activeObject = newItem; // Make it active in inspector
            EditorGUIUtility.PingObject(newItem);
        }

        private string SanitizeFileName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "NewSelectionData";
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name;
        }


        private void ApplySelectionFromWindow(SelectionData data)
        {
            if (data == null) return;
            Undo.RecordObjects(Selection.objects, "Before Applying Selection from Window");
            List<UnityEngine.Object> effectiveObjects = data.GetEffectiveSelectedObjects();
            Selection.objects = effectiveObjects.ToArray();
            Debug.Log($"Window applied selection: {effectiveObjects.Count} objects selected from '{data.name}'.");
             if (Selection.activeObject != null) EditorGUIUtility.PingObject(Selection.activeObject);
            else if (effectiveObjects.Count > 0) EditorGUIUtility.PingObject(effectiveObjects[0]);
        }

        private void DuplicateSelectionData(SelectionData originalData)
        {
            string path = AssetDatabase.GetAssetPath(originalData);
            string newPath = AssetDatabase.GenerateUniqueAssetPath(path);

            if (AssetDatabase.CopyAsset(path, newPath))
            {
                SelectionData duplicatedData = AssetDatabase.LoadAssetAtPath<SelectionData>(newPath);
                Undo.RegisterCreatedObjectUndo(duplicatedData, "Duplicate SelectionData");

                Undo.RecordObject(duplicatedData, "Rename Duplicated SelectionData");
                duplicatedData.description = string.IsNullOrEmpty(originalData.description) ? originalData.name + " (Copy)" : originalData.description + " (Copy)";
                // If it was a static combined, the selectedObjects are already copied.
                // If it was dynamic, the sourceSelectionData list is copied.
                EditorUtility.SetDirty(duplicatedData);
                AssetDatabase.SaveAssets(); // Save before refresh
                AssetDatabase.Refresh(); // Refresh to make sure Unity knows about the new asset properly

                RefreshSelectionDataList();
                selectedDataInWindow = duplicatedData;
                Selection.activeObject = duplicatedData;
                EditorGUIUtility.PingObject(duplicatedData);
            }
        }

        private void DeleteSelectionData(SelectionData dataToDelete)
        {
            string path = AssetDatabase.GetAssetPath(dataToDelete);
            AssetDatabase.DeleteAsset(path); // is implicitly handled by Undo.DeleteObjectUndo for ScriptableObjects

            AssetDatabase.Refresh(); // Refresh after potential deletion
            RefreshSelectionDataList();
            if (selectedDataInWindow == dataToDelete)
            {
                selectedDataInWindow = null;
            }
        }
    }
}
