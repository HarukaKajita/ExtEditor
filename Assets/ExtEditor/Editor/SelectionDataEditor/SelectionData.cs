using UnityEngine;
using UnityEditor; // Required for AssetDatabase and SearchService
using System.Collections.Generic;
using System.Linq; // Required for Enumerable methods

namespace ExtEditor.SelectionDataEditor
{
    // Enums (SelectionMode, CombinationMode, CombineOperation) remain the same
    // [CreateAssetMenu(...)] attribute remains the same
    public enum SelectionMode { Objects, Query }
    public enum CombinationMode { Static, Dynamic }
    public enum CombineOperation { Union, Intersection, Difference }

    [CreateAssetMenu(fileName = "NewSelectionData", menuName = "ExtEditor/Selection Data")]
    public class SelectionData : ScriptableObject
    {
        // Fields remain the same:
        [Tooltip("How this selection is defined.")]
        public SelectionMode selectionMode = SelectionMode.Objects;
        [Tooltip("Direct list of selected objects. Used if selectionMode is Objects, or if combinationMode is Static.")]
        public List<UnityEngine.Object> selectedObjects = new List<UnityEngine.Object>();
        [Tooltip("UnitySearch query string. Used if selectionMode is Query.")]
        public string unitySearchQuery = "";
        [Tooltip("Is this SelectionData starred to appear at the top of lists?")]
        public bool isStarred = false;
        [Tooltip("Optional description for this selection data.")]
        public string description = "";
        [Header("Combined Selection Settings")]
        [Tooltip("Source SelectionData assets if this is a combined selection.")]
        public List<SelectionData> sourceSelectionData = new List<SelectionData>();
        [Tooltip("How this combined selection behaves.")]
        public CombinationMode combinationMode = CombinationMode.Static;
        [Tooltip("The operation to perform on the source selections.")]
        public CombineOperation combineOperation = CombineOperation.Union;

        public List<UnityEngine.Object> GetEffectiveSelectedObjects(HashSet<SelectionData> visited = null)
        {
            if (visited == null)
            {
                visited = new HashSet<SelectionData>();
            }

            if (visited.Contains(this))
            {
                Debug.LogWarning($"Cycle detected in combined SelectionData: {name}. Returning empty list to prevent infinite loop.", this);
                return new List<UnityEngine.Object>(); // Cycle detected
            }
            visited.Add(this);

            List<UnityEngine.Object> result = new List<UnityEngine.Object>();

            if (sourceSelectionData != null && sourceSelectionData.Count > 0) // Combined
            {
                if (combinationMode == CombinationMode.Static)
                {
                    result.AddRange(selectedObjects.Where(o => o != null));
                }
                else // Dynamic
                {
                    if (sourceSelectionData.Count == 0) return result;

                    // Initialize with the first source's objects
                    var firstSource = sourceSelectionData[0];
                    if (firstSource != null)
                    {
                        result.AddRange(firstSource.GetEffectiveSelectedObjects(new HashSet<SelectionData>(visited)).Where(o => o != null));
                    }

                    for (int i = 1; i < sourceSelectionData.Count; i++)
                    {
                        var currentSource = sourceSelectionData[i];
                        if (currentSource == null) continue;

                        List<UnityEngine.Object> currentSourceObjects = currentSource.GetEffectiveSelectedObjects(new HashSet<SelectionData>(visited)).Where(o => o != null).ToList();

                        switch (combineOperation)
                        {
                            case CombineOperation.Union:
                                result = result.Union(currentSourceObjects).ToList();
                                break;
                            case CombineOperation.Intersection:
                                result = result.Intersect(currentSourceObjects).ToList();
                                break;
                            case CombineOperation.Difference:
                                result = result.Except(currentSourceObjects).ToList();
                                break;
                        }
                    }
                }
            }
            else // Not combined
            {
                if (selectionMode == SelectionMode.Objects)
                {
                    result.AddRange(selectedObjects.Where(o => o != null));
                }
                else // Query
                {
                    result.AddRange(ExecuteUnitySearchQuery(unitySearchQuery));
                }
            }

            // Remove self-references if any object in result is this SelectionData asset itself
            result.RemoveAll(item => item == this);
            return result.Distinct().ToList(); // Ensure distinct objects
        }

        public void PopulateFromDynamicSources()
        {
            if (sourceSelectionData == null || sourceSelectionData.Count == 0) return;

            Undo.RecordObject(this, "Populate SelectionData from Dynamic Sources");
            selectedObjects.Clear();
            // Need to create a new HashSet for this specific call to GetEffectiveSelectedObjects
            // to avoid interference if this method is called from within a GetEffectiveSelectedObjects chain.
            selectedObjects.AddRange(GetEffectiveSelectedObjects(new HashSet<SelectionData>()));
            EditorUtility.SetDirty(this);
        }

        private List<UnityEngine.Object> ExecuteUnitySearchQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<UnityEngine.Object>();
            }

            List<UnityEngine.Object> queryResults = new List<UnityEngine.Object>();
            // The UnitySearch API can be complex and version-dependent.
            // This is a simplified approach. For more robust search, deeper integration with SearchService might be needed.
            // Example: "ref:12345abcde" or "t:Texture"
            // For simplicity, we'll try to load assets by path/guid if the query looks like one,
            // otherwise, we'll use FindAssets for type searches.

            // This is a very basic interpretation of a query.
            // A full UnitySearch query parser is out of scope for this example.
            var assets = new List<UnityEngine.Object>();
            string[] guids;

            if (query.StartsWith("t:"))
            {
                guids = AssetDatabase.FindAssets(query);
            }
            else
            {
                // Try to treat the query as a path or name for FindAssets
                // This won't support complex UnitySearch queries directly here.
                guids = AssetDatabase.FindAssets(query);
            }

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                if (obj != null)
                {
                    queryResults.Add(obj);
                }
            }
            if(queryResults.Count == 0 && !query.Contains(":")) // If no results and not a type query, try loading by path directly
            {
                 UnityEngine.Object objByPath = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(query);
                 if(objByPath != null) queryResults.Add(objByPath);
            }


            Debug.Log($"Executed query '{query}', found {queryResults.Count} results.");
            return queryResults;
        }
    }
}
