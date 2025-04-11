using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ExtEditor.Editor.CopySelectMenu
{
    public static class CopySelectMenu
    {
        [MenuItem("Assets/Copy Select Menus/Copy Path")]
        public static void CopyPath()
        {
            var selectedObjects = Selection.objects;
            if (selectedObjects.Length == 0)
            {
                Debug.LogWarning("No objects selected.");
                return;
            }

            var paths = "";
            foreach (var obj in selectedObjects)
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(path)) continue;
                paths += path + "\n";
            }

            if (string.IsNullOrEmpty(paths)) return;
            EditorGUIUtility.systemCopyBuffer = paths.Trim();
            Debug.Log("Copied paths to clipboard:\n" + paths);
        }
        [MenuItem("Assets/Copy Select Menus/Copy GUID")]
        public static void CopyGuid()
        {
            var selectedObjects = Selection.objects;
            if (selectedObjects.Length == 0)
            {
                Debug.LogWarning("No objects selected.");
                return;
            }

            var guids = "";
            foreach (var obj in selectedObjects)
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(path)) continue;
                var guid = AssetDatabase.AssetPathToGUID(path);
                guids += guid + "\n";
            }

            if (string.IsNullOrEmpty(guids)) return;
            EditorGUIUtility.systemCopyBuffer = guids.Trim();
            Debug.Log("Copied GUIDs to clipboard:\n" + guids);
        }

        [MenuItem("Assets/Copy Select Menus/Select with Path from Clipboard")]
        public static void SelectWithPathFromClipboard()
        {
            var paths = EditorGUIUtility.systemCopyBuffer.Split(new[] { '\n' },
                System.StringSplitOptions.RemoveEmptyEntries);
            if (paths.Length == 0)
            {
                Debug.LogWarning("No paths in clipboard.");
                return;
            }

            var objects = new List<Object>();
            foreach (var path in paths)
            {
                var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (obj == null) continue;
                objects.Add(obj);
            }

            if (objects.Count == 0) return;
            Selection.objects = objects.ToArray();
            Debug.Log("Selected objects from clipboard paths:\n" + string.Join("\n", paths));
        }
        
        [MenuItem("Assets/Copy Select Menus/Select with GUID from Clipboard")]
        public static void SelectWithGuidFromClipboard()
        {
            var guids = EditorGUIUtility.systemCopyBuffer.Split(new[] { '\n' },
                System.StringSplitOptions.RemoveEmptyEntries);
            if (guids.Length == 0)
            {
                Debug.LogWarning("No GUIDs in clipboard.");
                return;
            }

            var objects = new List<Object>();
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path)) continue;
                var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (obj == null) continue;
                objects.Add(obj);
            }

            if (objects.Count == 0) return;
            Selection.objects = objects.ToArray();
            Debug.Log("Selected objects from clipboard GUIDs:\n" + string.Join("\n", guids));
        }
    }
}
