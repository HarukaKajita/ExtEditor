using UnityEngine;
using UnityEditor.AssetImporters;
using System.IO;

[ScriptedImporter(1, "md")]
public class MarkdownImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        string fileContent = File.ReadAllText(ctx.assetPath);
        TextAsset textAsset = new TextAsset(fileContent);

        ctx.AddObjectToAsset("main obj", textAsset);
        ctx.SetMainObject(textAsset);
    }
}
