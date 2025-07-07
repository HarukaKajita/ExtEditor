using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ExtEditor.Editor
{
    public class BlendShapeFrameSorter : AssetPostprocessor
    {
        void OnPostprocessModel(GameObject importedModel)
        {
            // モデル内のすべてのSkinnedMeshRendererを取得
            SkinnedMeshRenderer[] skinnedMeshRenderers = importedModel.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
            {
                Mesh mesh = skinnedMeshRenderer.sharedMesh;

                if (mesh == null || mesh.blendShapeCount == 0)
                    continue;

                // すべてのブレンドシェイプの情報を保持
                var blendShapeData = new BlendShapeData[mesh.blendShapeCount];
                bool needsFix = false;

                for (int shapeIndex = 0; shapeIndex < mesh.blendShapeCount; shapeIndex++)
                {
                    // 各ブレンドシェイプのフレーム情報を取得
                    string shapeName = mesh.GetBlendShapeName(shapeIndex);
                    int frameCount = mesh.GetBlendShapeFrameCount(shapeIndex);

                    var frames = new BlendShapeFrame[frameCount];
                    for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
                    {
                        frames[frameIndex] = new BlendShapeFrame
                        {
                            Weight = mesh.GetBlendShapeFrameWeight(shapeIndex, frameIndex),
                            DeltaVertices = new Vector3[mesh.vertexCount],
                            DeltaNormals = new Vector3[mesh.vertexCount],
                            DeltaTangents = new Vector3[mesh.vertexCount]
                        };

                        mesh.GetBlendShapeFrameVertices(shapeIndex, frameIndex,
                            frames[frameIndex].DeltaVertices,
                            frames[frameIndex].DeltaNormals,
                            frames[frameIndex].DeltaTangents);
                    }

                    // ソートが必要か判定
                    bool isSorted = IsSorted(frames);
                    if (!isSorted)
                    {
                        needsFix = true;
                        frames = frames.OrderBy(f => f.Weight).ToArray();
                    }

                    blendShapeData[shapeIndex] = new BlendShapeData
                    {
                        Name = shapeName,
                        Frames = frames,
                        RequiresSorting = !isSorted
                    };
                }

                // 必要な場合にのみ修正処理を実行
                if (needsFix)
                {
                    // 全てのブレンドシェイプをクリア
                    mesh.ClearBlendShapes();

                    // 再構築
                    foreach (var data in blendShapeData)
                    {
                        foreach (var frame in data.Frames)
                        {
                            mesh.AddBlendShapeFrame(data.Name, frame.Weight, frame.DeltaVertices, frame.DeltaNormals, frame.DeltaTangents);
                        }
                    }

                    // Debug.Log($"BlendShape frames in {importedModel.name} have been sorted and rebuilt.");
                }
            }
        }

        // フレームがWeight順にソートされているか確認する
        private bool IsSorted(BlendShapeFrame[] frames)
        {
            for (int i = 1; i < frames.Length; i++)
            {
                if (frames[i - 1].Weight > frames[i].Weight)
                    return false;
            }
            return true;
        }

        // ブレンドシェイプデータ全体を保持するクラス
        private class BlendShapeData
        {
            public string Name;
            public BlendShapeFrame[] Frames;
            public bool RequiresSorting;
        }

        // ブレンドシェイプフレームデータの構造体
        private struct BlendShapeFrame
        {
            public float Weight;
            public Vector3[] DeltaVertices;
            public Vector3[] DeltaNormals;
            public Vector3[] DeltaTangents;
        }
    }
}
