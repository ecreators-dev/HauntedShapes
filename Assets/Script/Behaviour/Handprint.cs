
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Assets.Script.Behaviour
{
		public class Handprint : MonoBehaviour
		{
				[SerializeField] private DecalProjector handProjector;
				[SerializeField] private bool handVisible;

				public bool IsHandVisible => handVisible;

				private void Update()
				{
						handProjector.enabled = IsHandVisible;
				}
		}
}