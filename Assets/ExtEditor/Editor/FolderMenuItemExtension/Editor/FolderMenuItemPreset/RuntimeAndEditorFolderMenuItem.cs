using System.IO;
using System.Linq;
using UnityEditor;

namespace ExtEditor.Editor{
    public class RuntimeAndEditorFolderMenuItem {
        private static readonly string[] childrenFolders = new string[] {
"FolderName1",
"FolderName2",
"FolderName3"
        };

        [MenuItem("Assets/Create/Folders/RuntimeAndEditor", false, 31)]
        public static void CreateFolders() {
            var selectedFolders = Selection
                .GetFiltered<DefaultAsset>( SelectionMode.Assets )
                .Select( AssetDatabase.GetAssetPath )
                .Where( AssetDatabase.IsValidFolder )
                .ToArray();

            foreach (var parent in selectedFolders)
                CreateChildren(parent);
                
            AssetDatabase.Refresh();
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