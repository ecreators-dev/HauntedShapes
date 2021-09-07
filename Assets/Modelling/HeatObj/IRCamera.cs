using UnityEngine;

namespace Assets.Modelling.HeatObj
{
		[ExecuteAlways]
		[RequireComponent(typeof(Camera))]
		[DisallowMultipleComponent]
		public class IRCamera : MonoBehaviour
		{
				private Camera Camera { get; set; }

				private void Awake()
				{
						Camera = GetComponent<Camera>();
				}

				private void OnPreCull()
				{
						HeatSource.INFRARED_VIEW_ACTIVE = Camera.enabled;
						HeatSource.LIGHTS.ForEach(entry =>
						{
								entry.light.renderingLayerMask = ~0;
								entry.light.enabled = HeatSource.INFRARED_VIEW_ACTIVE;
						});
				}

				private void OnPostRender()
				{
						HeatSource.LIGHTS.ForEach(entry =>
						{
								entry.light.renderingLayerMask = entry.mask;
								entry.light.enabled = false;
						});
				}
		}
}