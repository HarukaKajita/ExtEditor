using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ExtEditor.Editor.ExportPackageConfig
{
    [CreateAssetMenu(fileName = "ExportPackageConfig", menuName = "Scriptableobject/ExportPackageConfig")]
    public class ExportPackageConfig : ScriptableObject
    {
        [Header("Export Settings")] [Tooltip("含めたいアセットを設定します")]
        public Object[] assetsToExport;

        [Tooltip("依存アセットを含めるかどうか")] public bool includeDependencies = true;

        [Tooltip("出力するパッケージファイル名（.unitypackage 拡張子は不要）")]
        public string exportFileName = "NewExport";

        [Tooltip("出力先のディレクトリパス（Assets からの相対パス）")]
        public string exportDirectoryRelativePath = "ExportedPackages";

        [HideInInspector] public string[] assetPathsIncluded;

        /// <summary>
        private void OnValidate()
        {
            var paths = assetsToExport
                .Where(obj => obj != null)
                .Select(AssetDatabase.GetAssetPath)
                .Distinct()
                .ToArray();
            
            assetPathsIncluded = includeDependencies
                ? AssetDatabase.GetDependencies(paths, true)
                : paths;
            // ソート
            assetPathsIncluded = assetPathsIncluded
                .OrderBy(path => path)
                .ToArray();
        }
#if UNITY_EDITOR
        /// <summary>
        /// UnityPackage を出力する
        /// </summary>
        public void ExportPackage()
        {
            if (assetsToExport == null || assetsToExport.Length == 0)
            {
                Debug.LogWarning("エクスポート対象のアセットが設定されていません。");
                return;
            }

            // アセットパスを取得
            string[] paths = assetsToExport
                .Where(obj => obj != null)
                .Select(AssetDatabase.GetAssetPath)
                .Distinct()
                .ToArray();

            if (paths.Length == 0)
            {
                Debug.LogWarning("有効なアセットが見つかりませんでした。");
                return;
            }

            // 出力先のディレクトリを組み立てる
            string fullExportDir = Path.Combine(Application.dataPath, exportDirectoryRelativePath);
            if (!Directory.Exists(fullExportDir))
            {
                Directory.CreateDirectory(fullExportDir);
                Debug.Log($"出力先ディレクトリを作成しました: {fullExportDir}");
            }

            // 出力先ファイルパス
            string exportFilePath = Path.Combine(fullExportDir, exportFileName + ".unitypackage");

            // Export
            AssetDatabase.ExportPackage(
                paths,
                exportFilePath,
                includeDependencies ? ExportPackageOptions.IncludeDependencies : ExportPackageOptions.Default
            );

            Debug.Log($"UnityPackageを書き出しました: {exportFilePath}");

            // 書き出されるアセット一覧を更新
            assetPathsIncluded = includeDependencies
                ? AssetDatabase.GetDependencies(paths, true)
                : paths;
        }
#endif
        
#if UNITY_EDITOR
        [CustomEditor(typeof(ExportPackageConfig))]
        public class ExportPackageConfigEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                ExportPackageConfig config = (ExportPackageConfig)target;

                serializedObject.Update();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("assetsToExport"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("includeDependencies"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("exportFileName"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("exportDirectoryRelativePath"));

                EditorGUILayout.Space();

                if (GUILayout.Button("Export UnityPackage"))
                {
                    config.ExportPackage();
                }

                EditorGUILayout.Space();

                if (config.assetPathsIncluded != null && config.assetPathsIncluded.Length > 0)
                {
                    EditorGUILayout.LabelField("含まれるアセット一覧:", EditorStyles.boldLabel);
                    foreach (string path in config.assetPathsIncluded)
                    {
                        EditorGUILayout.LabelField("• " + path);
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
