using System;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// A marker, represents this method is called by <see cref="Equipment"/>-script.
		/// </summary>
		[AttributeUsage(AttributeTargets.Method)]
		public class CalledByEquipmentBehaviourAttribute : Attribute
		{ }
}