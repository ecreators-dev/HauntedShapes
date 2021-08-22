using UnityEngine;

namespace Assets.Script.Model
{
		public static class IMonoBehaviourExtensions
		{
				public static string GetGameObjectName(this IMonoBehaviour obj)
				{
						return obj is MonoBehaviour mb ? mb.gameObject.name : null;
				}

				public static string GetImplementationTypeName(this IMonoBehaviour obj)
				{
						return obj.GetType().Name;
				}
		}
}