using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic; // Required for List

namespace ExtEditor.SelectionDataEditor
{
    [CustomEditor(typeof(SelectionData))]
    public class SelectionDataEditor : UnityEditor.Editor
    {
        SerializedProperty selectionModeProp;
        SerializedProperty selectedObjectsProp;
        SerializedProperty unitySearchQueryProp;
        SerializedProperty isStarredProp;
        SerializedProperty descriptionProp;
        SerializedProperty sourceSelectionDataProp;
        SerializedProperty combinationModeProp;
        SerializedProperty combineOperationProp;

        void OnEnable()
        {
            selectionModeProp = serializedObject.FindProperty("selectionMode");
            selectedObjectsProp = serializedObject.FindProperty("selectedObjects");
            unitySearchQueryProp = serializedObject.FindProperty("unitySearchQuery");
            isStarredProp = serializedObject.FindProperty("isStarred");
            descriptionProp = serializedObject.FindProperty("description");
            sourceSelectionDataProp = serializedObject.FindProperty("sourceSelectionData");
            combinationModeProp = serializedObject.FindProperty("combinationMode");
            combineOperationProp = serializedObject.FindProperty("combineOperation");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SelectionData data = (SelectionData)target;

            EditorGUILayout.PropertyField(descriptionProp);
            EditorGUILayout.PropertyField(isStarredProp);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(selectionModeProp);
            if (EditorGUI.EndChangeCheck())
            {
                // If switching to static combined, and sources exist, populate from dynamic
                if (data.selectionMode == SelectionMode.Objects &&
                    data.sourceSelectionData != null && data.sourceSelectionData.Count > 0 &&
                    data.combinationMode == CombinationMode.Static)
                {
                    data.PopulateFromDynamicSources();
                }
            }


            if (data.selectionMode == SelectionMode.Objects)
            {
                // For combined data, selectedObjects might be read-only if dynamic, or editable if static
                bool isDynamicCombined = data.sourceSelectionData != null && data.sourceSelectionData.Count > 0 && data.combinationMode == CombinationMode.Dynamic;

                GUI.enabled = !isDynamicCombined;
                EditorGUILayout.PropertyField(selectedObjectsProp, true);
                GUI.enabled = true;
                if(isDynamicCombined)
                {
                    EditorGUILayout.HelpBox("Object list is dynamically generated. To edit, change to Static mode or modify source SelectionData.", MessageType.Info);
                }

            }
            else // SelectionMode.Query
            {
                EditorGUILayout.PropertyField(unitySearchQueryProp);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Combine Selection", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(sourceSelectionDataProp, true);
            if(EditorGUI.EndChangeCheck())
            {
                 // If sources were added and it's static, populate
                if (data.sourceSelectionData != null && data.sourceSelectionData.Count > 0 &&
                    data.combinationMode == CombinationMode.Static)
                {
                    data.PopulateFromDynamicSources();
                }
                 serializedObject.ApplyModifiedProperties(); // Apply changes to sourceSelectionData list
                 serializedObject.Update(); // Re-fetch data
            }


            if (data.sourceSelectionData != null && data.sourceSelectionData.Count > 0)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(combinationModeProp);
                if(EditorGUI.EndChangeCheck())
                {
                    // If switching to Static, populate from dynamic sources
                    if (data.combinationMode == CombinationMode.Static)
                    {
                        data.PopulateFromDynamicSources();
                    }
                    else // Switching to Dynamic
                    {
                        // Optionally clear selectedObjects if switching to dynamic, or leave them as a 'cache'
                        // For now, let's clear them to make it less confusing.
                        Undo.RecordObject(data, "Switch to Dynamic Combination");
                        data.selectedObjects.Clear();
                        EditorUtility.SetDirty(data);
                    }
                }
                EditorGUILayout.PropertyField(combineOperationProp);

                if (data.combinationMode == CombinationMode.Dynamic)
                {
                    if (GUILayout.Button("Cache Dynamic Selection to Static List"))
                    {
                        data.PopulateFromDynamicSources(); // This already records undo
                    }
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Apply Stored Selection"))
            {
                ApplySelection(data);
            }

            if (data.selectionMode == SelectionMode.Objects && (data.sourceSelectionData == null || data.sourceSelectionData.Count == 0))
            {
                 if (GUILayout.Button("Set Selection From Current Editor Selection"))
                 {
                     SetSelectionFromCurrent(data);
                 }
            }
            else if (data.selectionMode == SelectionMode.Query)
            {
                if (GUILayout.Button("Test Query & Select Results"))
                {
                    ApplySelection(data); // ApplySelection now handles queries
                }
            }


            serializedObject.ApplyModifiedProperties();
        }

        private void ApplySelection(SelectionData data)
        {
            Undo.RecordObjects(Selection.objects, "Before Applying Selection");
            List<UnityEngine.Object> effectiveObjects = data.GetEffectiveSelectedObjects();
            Selection.objects = effectiveObjects.ToArray();
            Debug.Log($"Applied selection: {effectiveObjects.Count} objects selected from '{data.name}'.");
            if (Selection.activeObject != null) EditorGUIUtility.PingObject(Selection.activeObject);
            else if (effectiveObjects.Count > 0) EditorGUIUtility.PingObject(effectiveObjects[0]);
        }

        private void SetSelectionFromCurrent(SelectionData data)
        {
            Undo.RecordObject(data, "Set Selection From Current");
            data.selectedObjects.Clear();
            data.selectedObjects.AddRange(Selection.objects.Where(o => o != data)); // Prevent self-reference
            EditorUtility.SetDirty(data);
            Debug.Log($"SelectionData '{data.name}' updated with {data.selectedObjects.Count} currently selected objects.");
        }
    }
}
