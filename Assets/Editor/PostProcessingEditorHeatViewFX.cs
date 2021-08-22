
using UnityEditor.Rendering;

namespace Assets.Editor
{
		[VolumeComponentEditor(typeof(HeatViewFX))]
		public class PostProcessingEditorHeatViewFX : VolumeComponentEditor
		{
				SerializedDataParameter intensityProperty;

				public override bool hasAdvancedMode => false;

				public override void OnEnable()
				{
						base.OnEnable();
						var owner = new PropertyFetcher<HeatViewFX>(serializedObject);
						intensityProperty = Unpack(owner.Find(fx => fx.intensity));
				}

				public override void OnInspectorGUI()
				{
						// display:

						PropertyField(intensityProperty);
				}
		}
}