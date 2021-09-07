using UnityEngine;

namespace Assets.Modelling.HeatObj
{
		[ExecuteAlways]
		[RequireComponent(typeof(Camera))]
		[DisallowMultipleComponent]
		public class ClassicCamera : MonoBehaviour
		{
				private void OnPreCull()
				{
						HeatSource.LIGHTS.ForEach(entry =>
						{
								entry.light.renderingLayerMask = entry.mask;
								if (HeatSource.INFRARED_VIEW_ACTIVE)
								{
										entry.light.enabled = false;
								}
						});
				}
		}
}