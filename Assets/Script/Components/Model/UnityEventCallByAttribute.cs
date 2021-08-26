using System;

using UnityEngine;

namespace Assets.Script.Components
{
		/// <summary>
		/// Defines a marker to provide a reference to another monobahaviour that invokes this method
		/// </summary>
		[AttributeUsage(AttributeTargets.Method)]
		public class UnityEventCallByAttribute : Attribute
		{
				public UnityEventCallByAttribute(Type componentType)
				{
						VerifyTypeIsComponent(componentType);
				}

				private static void VerifyTypeIsComponent(Type componentType)
				{
						if (!typeof(MonoBehaviour).IsAssignableFrom(componentType))
						{
								Debug.LogError("This given type must be a type of MonoBehaviour, but was " + componentType.ToString());
						}
				}
		}
}