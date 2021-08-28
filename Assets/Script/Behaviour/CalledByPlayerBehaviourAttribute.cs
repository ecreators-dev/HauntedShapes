using System;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// A marker shows, that this method is called from the <see cref="PlayerBehaviour"/>-script
		/// </summary>
		[AttributeUsage(AttributeTargets.Method)]
		public class CalledByPlayerBehaviourAttribute : Attribute
		{ }
}