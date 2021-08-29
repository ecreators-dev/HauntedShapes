using UnityEditor;

using UnityEngine;

namespace Assets.Editor
{
		public class SnapToGroundEditor : EditorSnapToGround
		{
				[MenuItem("Game/Auswahl auf den Boden stellen")]
				public static void GroundAllInSelection()
				{
						foreach (Transform selfTransform in Selection.transforms)
						{
								GroundTransform(selfTransform);
						}
				}
		}
}