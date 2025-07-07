using System.Linq;
using UnityEditor;

namespace ExtEditor.Editor
{
	public static class ReserializeMenuItem
	{
		[MenuItem("Assets/Reserialize")]
		public static void Reserialize()
		{
			var objects = UnityEditor.Selection.objects;
			var paths = objects.Select(AssetDatabase.GetAssetPath);
			AssetDatabase.ForceReserializeAssets(paths, ForceReserializeAssetsOptions.ReserializeAssetsAndMetadata);
		}
	}
}
