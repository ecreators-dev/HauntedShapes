using System;

using UnityEngine;

namespace Assets.Script.GameMenu
{
		public static class GUISizeHelper
		{
				public static Vector2 CalculateSize(string title, Func<GUISkin, GUIStyle> skin, out GUIStyle style)
				{
						GUIContent content = new GUIContent(title);

						style = skin(GUI.skin);
						style.alignment = TextAnchor.MiddleCenter;

						// Compute how large the button needs to be.
						Vector2 size = style.CalcSize(content);
						return size;
				}
		}
}
