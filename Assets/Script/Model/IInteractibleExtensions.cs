using UnityEngine;

namespace Assets.Script.Model
{
		public static class IInteractibleExtensions
		{
				/// <summary>
				/// Place inside of unity event method OnValidate()
				/// </summary>
				/// <param name="sourceScript">use "this"</param>
				public static void ValidateInteractibleWithStaticSettings(this MonoBehaviour sourceScript)
				{
#if UNITY_EDITOR
						if (Application.isPlaying is false && sourceScript.gameObject is { } && sourceScript.gameObject.isStatic && sourceScript is IInteractible interactible)
						{
								Debug.LogError($"{interactible.GameObjectName} must not be static! It implements {nameof(IInteractible)}");
						}
#endif
				}
		}
}