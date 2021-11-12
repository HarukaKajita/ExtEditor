using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace ExtEditor.Editor {
    [ScriptedImporter(1, "fmi")]
    public class FolderMenuItemImporter : ScriptedImporter {

        public override void OnImportAsset(AssetImportContext ctx) {
            var selfPath = ctx.assetPath;
            var parent = Path.GetDirectoryName(selfPath);
            var menuItemName = Path.GetFileNameWithoutExtension(selfPath);
            var scriptFilePath = parent + "/" + menuItemName + "FolderMenuItem.cs";
            
            var lines = File.ReadAllLines(ctx.assetPath).ToList();
            
            // Debug.Log(selfPath);
            // Debug.Log(parent);
            // Debug.Log(scriptFilePath);
            // Debug.Log(lines);
            
            //Assets/Create/Folderのpriorityが20なのでその下に表示させるために+21でオフセットしておく。
            //https://blog.redbluegames.com/guide-to-extending-unity-editors-menus-b2de47a746db
            var priority = int.Parse(lines[0])+21;
            lines.RemoveAt(0);
            var folderListCode = string.Join(",\n",lines.Select(str => "\"" + str + "\"").ToList());

            string scriptTemplatePath = parent + "/ScriptTemplate.txt";
            string templateText = File.ReadAllText(scriptTemplatePath);
            templateText = templateText.Replace("FOLDERMENUITEMCLASSNAME", menuItemName+"FolderMenuItem");
            templateText = templateText.Replace("FOLDERMENUITEMNAME", menuItemName);
            templateText = templateText.Replace("FOLDERLIST", folderListCode);
            templateText = templateText.Replace("PRIORITY", priority.ToString());
            
            if(File.Exists(scriptFilePath)) File.Delete(scriptFilePath);
            StreamWriter sw = File.CreateText(scriptFilePath);
            sw.Write(templateText);
            sw.Flush();
            sw.Close();
            
            var scriptAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(scriptFilePath);
            //AddObjectToAssetするとC#スクリプトとして機能しなくなる
            //ctx.AddObjectToAsset("MainAsset", scriptAsset);
            //ctx.SetMainObject(scriptAsset);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(scriptAsset));
            AssetDatabase.Refresh();
        }
    }
}