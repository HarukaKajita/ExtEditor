using UnityEditor;
using UnityEngine;
using System.IO; // Required for Path.GetExtension

[CustomEditor(typeof(TextAsset))]
public class MarkdownImporterEditor : Editor
{
    private Vector2 scrollPosition;
    private GUIStyle h1Style;
    private GUIStyle h2Style;
    private GUIStyle normalStyle;
    private GUIStyle listItemStyle;

    void OnEnable()
    {
        // Initialize styles here to ensure they are ready and to customize them
        h1Style = new GUIStyle(EditorStyles.label)
        {
            fontSize = 18, // Larger font for H1
            fontStyle = FontStyle.Bold,
            richText = true,
            wordWrap = true // Enable word wrap
        };

        h2Style = new GUIStyle(EditorStyles.label)
        {
            fontSize = 15, // Slightly smaller bold font for H2
            fontStyle = FontStyle.Bold,
            richText = true,
            wordWrap = true
        };
        
        normalStyle = new GUIStyle(EditorStyles.label)
        {
            richText = true,
            wordWrap = true 
        };
        
        listItemStyle = new GUIStyle(EditorStyles.label)
        {
            richText = true,
            wordWrap = true
        };
    }

    public override void OnInspectorGUI()
    {
        TextAsset textAsset = (TextAsset)target;
        string assetPath = AssetDatabase.GetAssetPath(textAsset);

        if (string.IsNullOrEmpty(assetPath) || !Path.GetExtension(assetPath).Equals(".md", System.StringComparison.OrdinalIgnoreCase))
        {
            // Draw the default inspector for non-.md TextAssets or if path is invalid
            base.OnInspectorGUI();
            return;
        }

        // Custom inspector for .md files
        EditorGUILayout.LabelField("Markdown Preview:", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        string fileContent = textAsset.text;
        string[] lines = fileContent.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(Mathf.Min(300, lines.Length * EditorGUIUtility.singleLineHeight + 20))); // Adjust height dynamically or set a fixed max

        foreach (string line in lines)
        {
            if (line.StartsWith("# "))
            {
                EditorGUILayout.SelectableLabel(line.Substring(2), h1Style, GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.5f));
            }
            else if (line.StartsWith("## "))
            {
                EditorGUILayout.SelectableLabel(line.Substring(3), h2Style, GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.3f));
            }
            else if (line.StartsWith("- ") || line.StartsWith("* "))
            {
                EditorGUILayout.SelectableLabel("• " + line.Substring(2), listItemStyle, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            }
            else if (string.IsNullOrWhiteSpace(line)) // Handle empty lines as spacing
            {
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
            }
            else
            {
                EditorGUILayout.SelectableLabel(line, normalStyle, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            }
        }

        EditorGUILayout.EndScrollView();
    }
}
