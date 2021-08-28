
using UnityEngine;

namespace Assets.Script.Behaviour
{
		public static class BehaviourExtensions
		{
				/// <summary>
				/// Seeks for any Script extending T in self and all parents
				/// </summary>
				public static bool TryGetComponentAllParent<T>(this Component script, out T result)
				{
						result = script.GetComponent<T>();
						if (result != null)
						{
								return true;
						}

						Transform current = script.transform;
						Transform parent = current.parent;
						while (parent != null)
						{
								result = script.GetComponent<T>();
								if (result != null) return true;
								current = parent;
								parent = current.parent;
						}
						return false;
				}
		}
}