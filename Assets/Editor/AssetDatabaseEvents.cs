using System;

using UnityEditor;

using UnityEngine;

namespace Assets.Script.GameMenu
{
#if UNITY_EDITOR
		public class AssetDatabaseEvents : AssetPostprocessor
		{
				public static DateTime LastUpdate { get; private set; }

				public static event Action<AssetPath> UpdatedEvent;
				public static event Action<AssetPath> AssetDeleteEvent;
				public static event Action<AssetPath> AssetImportedEvent;
				public static event Action<AssetPath> AssetMovedEvent;

				public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
				{
						foreach (string path in importedAssets)
						{
								Debug.Log($"Reimported Asset: {path}");

								AssetGuid guid = AssetDatabase.GUIDFromAssetPath(path).ToString().ToLower();
								AssetPath p = guid;
								AssetImportedEvent?.Invoke(p);
								UpdatedEvent?.Invoke(p);
						}
						foreach (string path in deletedAssets)
						{
								Debug.Log($"Deleted Asset: {path}");

								AssetGuid guid = AssetDatabase.GUIDFromAssetPath(path).ToString().ToLower();
								AssetPath p = AssetPath.Build(path, guid);
								AssetDeleteEvent?.Invoke(p);
								UpdatedEvent?.Invoke(p);
						}

						int index = 0;
						foreach (string path in movedAssets)
						{
								string fromPath = movedFromAssetPaths[index];
								Debug.Log($"Moved Asset: {fromPath} -> {path}");
								AssetGuid guid = AssetDatabase.GUIDFromAssetPath(path).ToString().ToLower();
								AssetPath p = guid;
								AssetMovedEvent?.Invoke(p);
								UpdatedEvent?.Invoke(p);
								index++;
						}
						LastUpdate = DateTime.Now;
				}
		}

#endif
}
