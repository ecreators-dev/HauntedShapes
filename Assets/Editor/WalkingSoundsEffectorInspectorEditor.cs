using Assets.Script.Behaviour;

using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Assets.Editor
{
		[CustomEditor(typeof(WalkingSoundsEffector))]
		public class WalkingSoundsEffectorInspectorEditor : UnityEditor.Editor
		{
				private List<string> lastFoundImplementations;
				private float lastUpdate;
				private bool namesFoldOutState = true;

				public override void OnInspectorGUI()
				{
						base.OnInspectorGUI();

						EditorGUILayout.HelpBox($"Only effective for MonoBehaviours with interface {nameof(IStepSoundProvider)}", MessageType.Info, true);

						EditorGUILayout.HelpBox("*You will get an error in console, if interface is not used in scene yet (2 seconds update)", MessageType.Warning);

						// lastUpdate is 0 || Time.realtimeSinceStartup - lastUpdate > 2
						if (true)
						{
								var any = Object.FindObjectsOfType<MonoBehaviour>();
								List<string> withProvider = (from cmp in any
																						 let p = cmp.GetComponent<IStepSoundProvider>()
																						 where p is { }
																						 let m = p as MonoBehaviour
																						 let name = m.GetType().Name
																						 let parent = m.gameObject.name
																						 let n = $"{parent} / {name}"
																						 orderby n ascending
																						 select n).ToList();

								if (withProvider.Count is 0)
								{
										Debug.LogError($"Scene is missing MonoBehaviour with implementation of {nameof(IStepSoundProvider)} for {nameof(WalkingSoundsEffector)} '{(target is WalkingSoundsEffector ws ? ws.gameObject.name : "<?>")}'");
								}

								lastFoundImplementations = withProvider.Distinct().ToList();

								namesFoldOutState = EditorGUILayout.Foldout(namesFoldOutState, $"Implementations of {nameof(IStepSoundProvider)}", true);
								if (namesFoldOutState)
								{
										foreach (string name in lastFoundImplementations)
										{
												GUILayout.Label(name);
										}
								}
								lastUpdate = Time.realtimeSinceStartup;
						}
				}
		}
}