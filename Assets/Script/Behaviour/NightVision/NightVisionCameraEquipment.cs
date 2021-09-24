using System;

using UnityEngine;

namespace Assets.Script.Behaviour.NightVision
{
		/// <summary>
		/// Pivot on Camera is 0,0,0, but the 1st child has the face offset - the reference in hand is invisible
		/// </summary>
		[DisallowMultipleComponent]
		public class NightVisionCameraEquipment : EquipmentPlacable
		{
				[SerializeField] private ShopParameters shopInfo;
				[SerializeField] private Material screenOn;
				[SerializeField] private Material screenOff;
				[SerializeField] private MeshRenderer Renderer;

				protected override void Start()
				{
						SetShopInfo(shopInfo, this);
						base.Start();

						ToggleOff();
				}

				private void ToggleOff()
				{
						SetPowered(false, true);
				}

				private void ToggleOn()
				{
						SetPowered(true, true);
				}

				protected override void Update()
				{
						base.Update();

						Renderer.material = IsPowered ? screenOn : screenOff;
				}

				public override EquipmentInfo GetEquipmentInfo()
				{
						return new EquipmentInfo
						{
								Text = $"{ShopInfo.DisplayName}",
								TimerText = null
						};
				}

				public override bool CanInteract(PlayerBehaviour sender)
				{
						return base.CanInteract(sender) && IsUserOrNotTaken(sender);
				}

				protected override void Interact(PlayerBehaviour sender)
				{
						if (IsTakenByPlayer || IsPlaced)
						{
								TogglePowered();
						}
				}
		}
}