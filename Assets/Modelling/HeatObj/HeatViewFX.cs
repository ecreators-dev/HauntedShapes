using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[Serializable, VolumeComponentMenu("Post-processing/Custom/Heat View FX")]
public class HeatViewFX : CustomPostProcessVolumeComponent, IPostProcessComponent
{
		[Tooltip("Controls the intensity of the effect.")]
		public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

		[Tooltip("Welches Material tragen die Hitzequellen")]
		public Material heatMaterialToSeek;
		
		private bool effectActive = true;

		public bool IsActive() => effectActive;

		public override void Setup()
		{
				base.Setup();
		}

		public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
		{
				// HDUtils.DrawFullScreen(cmd, m_Material, destination);
		}

		public override void Cleanup()
		{
				base.Cleanup();
		}

		public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

		
		/* example code
// example code
// Material m_Material;

// public bool IsActive() => m_Material != null && intensity.value > 0f;

// public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

public override void Setup()
{
		if (Shader.Find("Hidden/Shader/GrayScale") != null)
				m_Material = new Material(Shader.Find("Hidden/Shader/GrayScale"));
}

public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
{
		if (m_Material is null)
				return;

		m_Material.SetFloat("_Intensity", intensity.value);
		m_Material.SetTexture("_InputTexture", source);
		HDUtils.DrawFullScreen(cmd, m_Material, destination);
}

public override void Cleanup()
{
		CoreUtils.Destroy(m_Material);
}
		*/
}
