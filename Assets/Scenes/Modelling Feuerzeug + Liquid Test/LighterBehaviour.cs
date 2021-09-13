using Assets.Script.Behaviour;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LighterBehaviour : EquipmentPlacable
{
		[SerializeField] private bool StartEnlit = false;
		[SerializeField] private Material liquidMat;
		[Range(0, 1)]
		[SerializeField] private float filling = 1;
		[SerializeField] private Color fillingColor;
		[SerializeField] private GameObject LightCandle;
		[SerializeField] private float endTimeoutSeconds = 120f;

		private float endTimer = 120f;
		private Shader LiquidShader;

		protected override void Start()
		{
				this.LiquidShader = liquidMat.shader;

				base.Start();

				if (StartEnlit)
				{
						ToggleOn();
				}
				else
				{
						ToggleOff();
				}
		}

		private void ToggleOff()
		{
				SetPowered(false);
				LightCandle.SetActive(false);
		}

		private void ToggleOn()
		{
				if (IsBroken is false)
				{
						if (endTimer <= 0)
						{
								endTimer = endTimeoutSeconds;
								UpdateShaderPropertyFilled(1);
						}

						SetPowered(true);
						LightCandle.SetActive(true);
				}
		}

		protected override void Update()
		{
				base.Update();

				if (IsBroken)
				{
						ToggleOff();
						return;
				}
				else if (endTimer <= 0)
				{
						endTimer = 0;
						SetBroken();
						return;
				}

				if (IsPowered)
				{
						endTimer -= Time.deltaTime;
						float progress = endTimer / endTimeoutSeconds;
						UpdateShaderPropertyFilled(progress);
				}

				if (IsPowered)
				{
						ToggleOff();
				}
				else
				{
						ToggleOn();
				}
		}

		private void UpdateShaderPropertyFilled(float progress)
		{
				int propertyId = Shader.PropertyToID("Filled");
				liquidMat.SetFloat(propertyId, progress);
		}

		public override EquipmentInfo GetEquipmentInfo()
		{
				return new EquipmentInfo
				{
						Text = ShopInfo.DisplayName
				};
		}

		public override void Interact(PlayerBehaviour sender)
		{
				if (IsTakenByPlayer)
				{
						SetPowered(!IsPowered);
				}
		}
}
