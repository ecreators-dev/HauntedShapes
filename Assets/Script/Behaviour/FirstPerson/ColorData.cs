
using System;

using UnityEngine;

namespace Assets.Script.Behaviour.FirstPerson
{
		[Serializable]
		public struct ColorData
		{
				public float R { get; set; }
				public float G { get; set; }
				public float B { get; set; }
				public float A { get; set; }

				public Color GetColor() => new Color(R, G, B, A);

				public void SetColor(Color color)
				{
						R = color.r;
						G = color.g;
						B = color.b;
						A = color.a;
				}
		}
}
