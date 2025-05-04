using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace ExtEditor.Editor.ExportPackageConfig
{
    [CreateAssetMenu(fileName = "ExportPackageConfig", menuName = "Scriptableobject/ExportPackageConfig")]
    public class ExportPackageConfig : ScriptableObject
    {
        public string exportPathExpression = "<ProjectPath>/ExportedPackages/<ConfigName>_<Date>";
        
        [FormerlySerializedAs("IncludeDependencies")]
        [Tooltip("依存アセットを含めるかどうか")]
        [SerializeField]
        private bool includeDependencies = true;
        
        [Tooltip("含めたいアセットを設定します")]
        [SerializeField]
        public Object[] exportEntryAssets = Array.Empty<Object>();//ユーザが指定するアセット
        [Tooltip("パッケージに含めないアセットを設定します")]
        public Object[] excludeAssets = Array.Empty<Object>();//ユーザが指定するアセット
        // フォルダの中身も含めたエントリアセットリスト
        [HideInInspector] private List<Object> _internalEntryAssets = new ();
        // 各エントリーアセットの依存アセットリスト
        [HideInInspector] private string[][] _dependenciesPerAsset = Array.Empty<string[]>();
        // 出力する全アセットのパス
        [HideInInspector] private List<string> _exportAssetPaths = new ();

        private void OnValidate()
        {
            // アセットパス
            var paths = exportEntryAssets
                .Where(obj => obj != null)
                .Distinct()
                .Select(AssetDatabase.GetAssetPath)
                .OrderBy(path => path)
                .ToArray();
            
            // 各アセット毎の依存アセットパスを取得
            _dependenciesPerAsset = new string[paths.Length][];
            _exportAssetPaths.Clear();
            _exportAssetPaths.AddRange(paths);
            _internalEntryAssets.Clear();
            _internalEntryAssets.AddRange(exportEntryAssets);
            for (var i = 0; i < paths.Length; i++)
                if (paths[i] != null)
                {
                    //pathがフォルダならそのフォルダ内のアセットを全て取得
                    if (AssetDatabase.IsValidFolder(paths[i]))
                    {
                        var filesInDirectory = AssetDatabase.FindAssets("", new[] { paths[i] })
                            .Select(AssetDatabase.GUIDToAssetPath)
                            .ToArray();
                        var dependencies = filesInDirectory.SelectMany(p => AssetDatabase.GetDependencies(p, true))
                            .Distinct()
                            .ToArray();
                        _exportAssetPaths.AddRange(filesInDirectory);
                        _internalEntryAssets.AddRange(filesInDirectory.Select(AssetDatabase.LoadMainAssetAtPath));
                        _dependenciesPerAsset[i] = dependencies;
                    }
                    else
                        _dependenciesPerAsset[i] = AssetDatabase.GetDependencies(paths[i], true);
                    
                    if(includeDependencies) _exportAssetPaths.AddRange(_dependenciesPerAsset[i]);
                }
            _exportAssetPaths = _exportAssetPaths.Distinct().OrderBy(path=>path).ToList();
            _internalEntryAssets = _internalEntryAssets.Distinct().ToList();
            
            // 除外アセットを除外する
            if (excludeAssets.Length > 0)
            {
                // excludeAssetsの要素がフォルダならそのフォルダ内のアセットを全て取得
                var excludeDirectories = excludeAssets
                    .Where(obj => obj != null)
                    .Distinct()
                    .Select(AssetDatabase.GetAssetPath)
                    .Where(AssetDatabase.IsValidFolder)
                    .ToArray();
                var excludeFilesInDirectories = excludeDirectories
                    .SelectMany(directory => AssetDatabase.FindAssets("", new[] { directory })
                        .Select(AssetDatabase.GUIDToAssetPath))
                    .ToArray();
                var excludePaths = excludeAssets
                    .Where(obj => obj != null)
                    .Distinct()
                    .Select(AssetDatabase.GetAssetPath)
                    .Where(path => !AssetDatabase.IsValidFolder(path))
                    .Concat(excludeFilesInDirectories)
                    .OrderBy(path => path)
                    .ToArray();
                _exportAssetPaths = _exportAssetPaths
                    .Where(path => !excludePaths.Contains(path))
                    .ToList();
                _internalEntryAssets = _internalEntryAssets
                    .Where(asset => !excludePaths.Contains(AssetDatabase.GetAssetPath(asset)))
                    .ToList();
            }
                
            // パッケージマネージャーのアセットを除外する
            {
                _exportAssetPaths = _exportAssetPaths
                    .Where(path => !path.StartsWith("Packages/"))
                    .ToList();
                _internalEntryAssets = _internalEntryAssets
                    .Where(asset => !AssetDatabase.GetAssetPath(asset).StartsWith("Packages/"))
                    .ToList();
            }
        }
#if UNITY_EDITOR
        // 出力先ファイルパス
        private string ExportFilePath()
        {
            var expressions = new Dictionary<string, string>
            {
                {"<ProjectPath>", Application.dataPath[..^7]},
                {"<AssetsPath>", Application.dataPath},
                {"<ConfigName>", this.name},
                {"<Date>", DateTime.Now.ToString("yyyyMMdd")},
                {"<Time>", DateTime.Now.ToString("HHmmss")}
            };
            // expressionsの適用
            var pathExpression = exportPathExpression;
            foreach (var expression in expressions)
                pathExpression = pathExpression.Replace(expression.Key, expression.Value);
            
            //ディレクトリの区切り文字を揃える
            pathExpression = pathExpression.Replace("/", Path.DirectorySeparatorChar.ToString());
            pathExpression = pathExpression.Replace("\\", Path.DirectorySeparatorChar.ToString());
            
            //illegal charを除去
            var splits = pathExpression.Split(Path.DirectorySeparatorChar);
            var illegalChars = Path.GetInvalidFileNameChars();
            // ドライブの場合はスキップ
            for(var i = 1; i < splits.Length; i++)
            {
                foreach (var illegalChar in illegalChars)
                    if(splits[i].Contains(illegalChar))
                        splits[i] = splits[i].Replace(illegalChar.ToString(), "");
            }
            pathExpression = string.Join(Path.DirectorySeparatorChar.ToString(), splits);
            pathExpression += ".unitypackage";
            pathExpression = pathExpression.Replace(EditorApplication.applicationPath, "");
            pathExpression = Path.GetFullPath(pathExpression);
            return pathExpression;
        }
        
        private void MakeDirectoryIfNotExists(string path)
        {
            var directoryName = Path.GetDirectoryName(path);
            if (directoryName != null && !Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
        }
        /// <summary>
        /// UnityPackage を出力する
        /// </summary>
        private void ExportPackage()
        {
            OnValidate();
            
            // 出力先のディレクトリを作成
            MakeDirectoryIfNotExists(ExportFilePath());
            // Export
            AssetDatabase.ExportPackage(
                _exportAssetPaths.ToArray(),
                ExportFilePath(),
                ExportPackageOptions.Recurse | ExportPackageOptions.Interactive
            );

            Debug.Log($"UnityPackageを書き出しました: {ExportFilePath()}");
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
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(config.exportPathExpression)), new GUIContent("出力するパッケージのパス"));
                
                // クリックしたらエクスプローラーで開くボタン
                var style = new GUIStyle(GUI.skin.button) { wordWrap = true };// 長いパスを折り返す
                if (GUILayout.Button($"エクスプローラーで開く\n{config.ExportFilePath()}", style))
                {
                    // 出力先のディレクトリを作成
                   config.MakeDirectoryIfNotExists(config.ExportFilePath());
                   var directoryName = Path.GetDirectoryName(config.ExportFilePath());
                    System.Diagnostics.Process.Start("explorer.exe", directoryName);
                }
                
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(config.includeDependencies)), new GUIContent("依存アセットを含める"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(config.exportEntryAssets)), new GUIContent("パッケージに含めるアセット"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(config.excludeAssets)), new GUIContent("パッケージから除外するアセット"), true);
                serializedObject.ApplyModifiedProperties();
                EditorGUILayout.Space();

                if (GUILayout.Button("unitypackageをパッケージを書き出す"))
                    config.ExportPackage();

                EditorGUILayout.Space();
                // 線を引く
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.Space();

                if (config._exportAssetPaths is { Count: > 0 })
                {
                    EditorGUILayout.LabelField("出力パッケージに含まれるアセット一覧:", EditorStyles.boldLabel);
                    // 全選択ボタン
                    if (GUILayout.Button("全選択")) Selection.objects = config._exportAssetPaths.Select(AssetDatabase.LoadMainAssetAtPath).ToArray();
                    
                    EditorGUILayout.LabelField("エントリーアセット一覧:", EditorStyles.boldLabel);
                    // 各エントリーアセットのパスを表示
                    foreach (var asset in config._internalEntryAssets)
                    {
                        // アセットのパスを表示
                        if (GUILayout.Button("• " + AssetDatabase.GetAssetPath(asset), EditorStyles.label))
                        {
                            // クリックでアセットを選択可能
                            Selection.activeObject = asset;
                            EditorGUIUtility.PingObject(asset);
                        }
                    }
                    // 各依存アセットのパスを表示
                    EditorGUILayout.LabelField("依存アセット一覧:", EditorStyles.boldLabel);
                    var dependencyPaths = config._exportAssetPaths
                        .Except(config._internalEntryAssets.Select(AssetDatabase.GetAssetPath))
                        .Distinct()
                        .OrderBy(path => path)
                        .ToArray();
                    foreach (var path in dependencyPaths)
                    {
                        // アセットのパスを表示
                        if (GUILayout.Button("• " + path, EditorStyles.label))
                        {
                            // クリックでアセットを選択可能
                            var asset = AssetDatabase.LoadMainAssetAtPath(path);
                            Selection.activeObject = asset;
                            EditorGUIUtility.PingObject(asset);
                        }
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
