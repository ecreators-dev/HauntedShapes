using System;

using UnityEngine;

namespace Assets.Script.Controller
{
		/// <summary>
		/// This field will be readonly in inspector
		/// </summary>
		[Serializable]
		public class ReadOnlyAttribute : PropertyAttribute
		{ }
}