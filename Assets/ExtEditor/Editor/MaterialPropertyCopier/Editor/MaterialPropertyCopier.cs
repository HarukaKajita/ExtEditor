using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class MaterialPropertyCopierEditor : EditorWindow
{
    private Material sourceMaterial;
    private List<Material> targetMaterials = new List<Material>();
    private List<ShaderPropertyInfo> shaderProperties = new List<ShaderPropertyInfo>();
    private Dictionary<string, CopiedProperty> copiedProperties = new Dictionary<string, CopiedProperty>();

    private Vector2 scrollPositionProperties;
    private Vector2 scrollPositionTargets;

    private class ShaderPropertyInfo
    {
        public string displayName;
        public string propertyName;
        public ShaderUtil.ShaderPropertyType propertyType;
        public bool isSelected;

        // Constructor for convenience
        public ShaderPropertyInfo(string dispName, string propName, ShaderUtil.ShaderPropertyType type)
        {
            displayName = dispName;
            propertyName = propName;
            propertyType = type;
            isSelected = false;
        }
    }

    private class CopiedProperty
    {
        public string propertyName;
        public ShaderUtil.ShaderPropertyType propertyType;
        public object value; // Color, Vector4, float, int (for texture instanceID)

        public CopiedProperty(string name, ShaderUtil.ShaderPropertyType type, object val)
        {
            propertyName = name;
            propertyType = type;
            value = val;
        }
    }

    [MenuItem("Tools/Material Property Copier")]
    public static void ShowWindow()
    {
        GetWindow<MaterialPropertyCopierEditor>("Material Property Copier");
    }

    void OnGUI()
    {
        GUILayout.Label("Material Property Copier", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Source Material
        GUILayout.Label("Source Material", EditorStyles.label);
        Material newSourceMaterial = (Material)EditorGUILayout.ObjectField(sourceMaterial, typeof(Material), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        if (newSourceMaterial != sourceMaterial)
        {
            sourceMaterial = newSourceMaterial;
            PopulateShaderProperties();
            copiedProperties.Clear(); // Clear copied properties if source changes
        }

        EditorGUILayout.Space();

        if (sourceMaterial == null)
        {
            EditorGUILayout.HelpBox("Assign a Source Material to see its properties.", MessageType.Info);
            return;
        }

        // Properties List
        GUILayout.Label("Shader Properties (Source: " + sourceMaterial.name + ")", EditorStyles.label);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Select All", GUILayout.Width(100)))
        {
            SetAllPropertiesSelection(true);
        }
        if (GUILayout.Button("Select None", GUILayout.Width(100)))
        {
            SetAllPropertiesSelection(false);
        }
        EditorGUILayout.EndHorizontal();

        scrollPositionProperties = EditorGUILayout.BeginScrollView(scrollPositionProperties, GUILayout.MinHeight(150), GUILayout.MaxHeight(300), GUILayout.ExpandHeight(false));
        if (shaderProperties.Count == 0)
        {
            EditorGUILayout.HelpBox("No properties found or material shader is null.", MessageType.Warning);
        }
        else
        {
            for (int i = 0; i < shaderProperties.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                shaderProperties[i].isSelected = EditorGUILayout.Toggle(shaderProperties[i].isSelected, GUILayout.Width(20));
                EditorGUILayout.LabelField(shaderProperties[i].displayName, shaderProperties[i].propertyName);
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        // Copy Button
        if (GUILayout.Button("Copy Selected Properties from Source"))
        {
            CopySelectedProperties();
        }

        EditorGUILayout.Space();

        // Target Materials
        GUILayout.Label("Target Materials", EditorStyles.label);
        
        // Button to add selected materials from project
        if (GUILayout.Button("Add Selected Materials from Project", GUILayout.Height(30)))
        {
            AddSelectedMaterialsToTargets();
        }
        
        scrollPositionTargets = EditorGUILayout.BeginScrollView(scrollPositionTargets, GUILayout.MinHeight(100), GUILayout.MaxHeight(200), GUILayout.ExpandHeight(false));
        if (targetMaterials.Count == 0)
        {
            EditorGUILayout.HelpBox("Add materials to the list above or by selecting them in the Project window and clicking 'Add Selected Materials'.", MessageType.Info);
        }
        else
        {
            for (int i = 0; i < targetMaterials.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                targetMaterials[i] = (Material)EditorGUILayout.ObjectField(targetMaterials[i], typeof(Material), false);
                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    targetMaterials.RemoveAt(i);
                    i--; // Adjust index after removal
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
        
        // Drag and drop area for target materials
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag & Drop Materials Here");
        ProcessDragAndDrop(dropArea);


        EditorGUILayout.Space();

        // Paste Button
        if (GUILayout.Button("Paste Copied Properties to Target(s)"))
        {
            if (targetMaterials.Count == 0)
            {
                Debug.LogWarning("Material Property Copier: No target materials specified.");
                EditorUtility.DisplayDialog("No Targets", "Please add target materials before pasting.", "OK");
            }
            else if (copiedProperties.Count == 0)
            {
                Debug.LogWarning("Material Property Copier: No properties have been copied yet.");
                EditorUtility.DisplayDialog("No Properties Copied", "Please copy properties from the source material first.", "OK");
            }
            else
            {
                PastePropertiesToTargets();
            }
        }
    }

    private void ProcessDragAndDrop(Rect dropArea)
    {
        Event evt = Event.current;
        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    break;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    bool newMaterialAdded = false;
                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        if (draggedObject is Material mat)
                        {
                            if (!targetMaterials.Contains(mat))
                            {
                                targetMaterials.Add(mat);
                                newMaterialAdded = true;
                            }
                        }
                    }
                    if(newMaterialAdded) Repaint(); // Refresh UI if new materials were added
                }
                break;
        }
    }


    private void AddSelectedMaterialsToTargets()
    {
        Material[] selectedMats = Selection.GetFiltered<Material>(SelectionMode.Assets);
        bool newMaterialAdded = false;
        foreach (Material mat in selectedMats)
        {
            if (!targetMaterials.Contains(mat))
            {
                targetMaterials.Add(mat);
                newMaterialAdded = true;
            }
        }
        if(newMaterialAdded) Debug.Log($"Material Property Copier: Added {selectedMats.Length} material(s) to targets.");
        else if (selectedMats.Length > 0) Debug.Log("Material Property Copier: Selected material(s) already in target list.");
        else Debug.Log("Material Property Copier: No materials selected in Project window.");
    }


    void PopulateShaderProperties()
    {
        shaderProperties.Clear();
        if (sourceMaterial == null || sourceMaterial.shader == null)
        {
            return;
        }

        Shader shader = sourceMaterial.shader;
        for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
        {
            ShaderUtil.ShaderPropertyType type = ShaderUtil.GetPropertyType(shader, i);
            // Supported types: Color, Vector, Float, Range, Texture
            if (type == ShaderUtil.ShaderPropertyType.Color ||
                type == ShaderUtil.ShaderPropertyType.Vector ||
                type == ShaderUtil.ShaderPropertyType.Float ||
                type == ShaderUtil.ShaderPropertyType.Range ||
                type == ShaderUtil.ShaderPropertyType.TexEnv) // TexEnv is used for Textures
            {
                string propName = ShaderUtil.GetPropertyName(shader, i);
                string dispName = ShaderUtil.GetPropertyDescription(shader, i);
                shaderProperties.Add(new ShaderPropertyInfo(dispName, propName, type));
            }
        }
        // Sort by display name for better readability
        shaderProperties = shaderProperties.OrderBy(p => p.displayName).ToList();
    }

    void SetAllPropertiesSelection(bool selected)
    {
        foreach (var propInfo in shaderProperties)
        {
            propInfo.isSelected = selected;
        }
    }

    void CopySelectedProperties()
    {
        if (sourceMaterial == null)
        {
            Debug.LogError("Material Property Copier: Source material is null.");
            EditorUtility.DisplayDialog("Error", "Source material is not assigned.", "OK");
            return;
        }

        copiedProperties.Clear();
        int count = 0;
        foreach (var propInfo in shaderProperties)
        {
            if (propInfo.isSelected)
            {
                if (!sourceMaterial.HasProperty(propInfo.propertyName))
                {
                    Debug.LogWarning($"Material Property Copier: Source material '{sourceMaterial.name}' (Shader: '{sourceMaterial.shader.name}') does not have property '{propInfo.propertyName}' listed. This might indicate a shader change or an issue with property population. Skipping this property.");
                    continue;
                }

                object value = null;
                switch (propInfo.propertyType)
                {
                    case ShaderUtil.ShaderPropertyType.Color:
                        value = sourceMaterial.GetColor(propInfo.propertyName);
                        break;
                    case ShaderUtil.ShaderPropertyType.Vector:
                        value = sourceMaterial.GetVector(propInfo.propertyName);
                        break;
                    case ShaderUtil.ShaderPropertyType.Float:
                    case ShaderUtil.ShaderPropertyType.Range: // Range is just a float with UI hints
                        value = sourceMaterial.GetFloat(propInfo.propertyName);
                        break;
                    case ShaderUtil.ShaderPropertyType.TexEnv: // Texture
                        Texture tex = sourceMaterial.GetTexture(propInfo.propertyName);
                        value = tex; // Can be null
                        break;
                }
                if (value != null)
                {
                    copiedProperties[propInfo.propertyName] = new CopiedProperty(propInfo.propertyName, propInfo.propertyType, value);
                    count++;
                }
            }
        }
        Debug.Log($"Material Property Copier: Copied {count} properties from {sourceMaterial.name}.");
        EditorUtility.DisplayDialog("Properties Copied", $"Copied {count} properties from '{sourceMaterial.name}'.", "OK");
    }

    void PastePropertiesToTargets()
    {
        if (targetMaterials.Count == 0 || copiedProperties.Count == 0)
        {
            Debug.LogWarning("Material Property Copier: No target materials or no properties copied.");
            return;
        }

        List<Material> modifiedMaterials = new List<Material>();
        foreach (var targetMat in targetMaterials)
        {
            if (targetMat == null) continue;
            
            bool changed = false;
            foreach (var copiedPropEntry in copiedProperties)
            {
                CopiedProperty cp = copiedPropEntry.Value;
                if (targetMat.HasProperty(cp.propertyName))
                {
                    // Check if shader property type matches, important for SetTexture vs SetColor etc.
                    ShaderUtil.ShaderPropertyType targetPropType = GetShaderPropertyType(targetMat.shader, cp.propertyName);
                    if (targetPropType == cp.propertyType) // Basic type check
                    {
                        if(!changed) // Only add to modifiedMaterials if we are actually going to change it
                        {
                             modifiedMaterials.Add(targetMat);
                             changed = true; // ensure we only add once and call RecordObject once per material
                        }
                        switch (cp.propertyType)
                        {
                            case ShaderUtil.ShaderPropertyType.Color:
                                targetMat.SetColor(cp.propertyName, (Color)cp.value);
                                break;
                            case ShaderUtil.ShaderPropertyType.Vector:
                                targetMat.SetVector(cp.propertyName, (Vector4)cp.value);
                                break;
                            case ShaderUtil.ShaderPropertyType.Float:
                            case ShaderUtil.ShaderPropertyType.Range:
                                targetMat.SetFloat(cp.propertyName, (float)cp.value);
                                break;
                            case ShaderUtil.ShaderPropertyType.TexEnv:
                                targetMat.SetTexture(cp.propertyName, (Texture)cp.value);
                                break;
                        }
                    }
                    else
                    {
                         Debug.LogWarning($"Material Property Copier: Property '{cp.propertyName}' type mismatch in target '{targetMat.name}'. Source type: {cp.propertyType}, Target type: {targetPropType}. Skipping this property for this material.");
                    }
                }
                else
                {
                    Debug.LogWarning($"Material Property Copier: Target material '{targetMat.name}' (Shader: '{targetMat.shader.name}') does not have property '{cp.propertyName}'. Skipping this property for this material.");
                }
            }
        }
        
        if(modifiedMaterials.Count > 0)
        {
            Undo.RecordObjects(modifiedMaterials.ToArray(), "Paste Material Properties");
            Debug.Log($"Material Property Copier: Pasted properties to {modifiedMaterials.Count} material(s).");
            EditorUtility.DisplayDialog("Properties Pasted", $"Pasted properties to {modifiedMaterials.Count} material(s). Check console for any warnings.", "OK");
        }
        else
        {
            Debug.Log("Material Property Copier: No properties were pasted. Targets might not have the copied properties or types mismatch.");
            EditorUtility.DisplayDialog("Properties Not Pasted", "No properties were pasted. Targets might not have the copied properties or types mismatch. Check console for details.", "OK");
        }
    }
    
    // Helper to get property type for a specific material's shader
    private ShaderUtil.ShaderPropertyType GetShaderPropertyType(Shader shader, string propertyName)
    {
        if (shader == null) return (ShaderUtil.ShaderPropertyType)(-1); // Invalid type

        for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
        {
            if (ShaderUtil.GetPropertyName(shader, i) == propertyName)
            {
                return ShaderUtil.GetPropertyType(shader, i);
            }
        }
        return (ShaderUtil.ShaderPropertyType)(-1); // Property not found
    }
}
