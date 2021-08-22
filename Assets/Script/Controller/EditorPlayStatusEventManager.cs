
using System;

using UnityEditor;

using UnityEngine;

namespace Assets.Script.Controller
{
#if UNITY_EDITOR
		[InitializeOnLoad]
		public static class EditorPlayStatusEventManager
		{
				public static event Action PlayModeExitEvent;
				public static void OnPlayModeExit(this Component _, Action handler)
				{
						PlayModeExitEvent += handler;
				}

				static EditorPlayStatusEventManager()
				{
						EditorApplication.playModeStateChanged += ModeChanged;
				}

				private static void ModeChanged(PlayModeStateChange action)
				{
						if (action is PlayModeStateChange.ExitingPlayMode)
						{
								Debug.Log("Exiting playmode.");
								PlayModeExitEvent?.Invoke();
						}
				}
		}
#endif
}