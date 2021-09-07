
using UnityEditor;

using UnityEngine;

namespace Assets.Script.GameMenu
{
#if UNITY_EDITOR
		public partial class HauntedShapesGameEditorWindow
		{
				private abstract class Gui<T, I>
						where T : EditorWindow
						where I : Gui<T, I>
				{
						public delegate void OnGui(T owner, I component);

						private readonly OnGui builder;

						protected Gui(OnGui build)
						{
								this.builder = build;
						}

						public bool Enabled { get; set; } = true;

						public int RootIntentDelta { get; set; }

						public void OnGUI(T owner)
						{
								GUI.enabled = Enabled;
								builder(owner, this as I);
								GUI.enabled = true;
						}
				}
		}

#endif
}
