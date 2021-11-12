using System.IO;
using System.Linq;
using UnityEditor;

namespace ExtEditor.Editor{
    public class AllFoldersFolderMenuItem {
        private static readonly string[] childrenFolders = new string[] {
"Textures",
"Materials",
"Shaders",
"Models",
"Prefabs",
"Scenes",
"Scripts",
"Editor"
        };

        [MenuItem("Assets/Create/Folders/AllFolders", false, 22)]
        public static void CreateFolders() {
            var selectedFolders = Selection
                .GetFiltered<DefaultAsset>( SelectionMode.Assets )
                .Select( AssetDatabase.GetAssetPath )
                .Where( AssetDatabase.IsValidFolder )
                .ToArray();

            foreach (var parent in selectedFolders)
                CreateChildren(parent);
        }

        private static void CreateChildren(string parentFolder) {
            foreach (var childrenFolder in childrenFolders)
                CreateChildFolder(parentFolder, childrenFolder);
        }

        private static void CreateChildFolder(string parentFolder, string childFolder) {
            string newFolder = parentFolder + "/" + childFolder + "/";
            var exits = Directory.Exists(newFolder);
            if (!exits) Directory.CreateDirectory(newFolder);
            AssetDatabase.ImportAsset(newFolder);
        }
    }
}