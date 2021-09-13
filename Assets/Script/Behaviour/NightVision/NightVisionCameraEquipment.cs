using Assets.Script.Behaviour.FirstPerson;

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
				[SerializeField] private NightVisionCameraModelHandEquipment handModelPrefab;

				private bool instanced;

				protected override void Start()
				{
						base.Start();
						SetShopInfo(shopInfo, this);
				}

				protected override void Update()
				{
						base.Update();

						if (Transform.parent == null)
						{
								instanced = false;
						}
				}

				public override EquipmentInfo GetEquipmentInfo()
				{
						return new EquipmentInfo
						{
								Text = ShopInfo.DisplayName,
								TimerText = null
						};
				}

				protected override void OnEquip()
				{
						base.OnEquip();

						// camera need to be a child of player camera
						// it need also to be in Hand! - now it is in Hand
						// put to camera and put camera to hand

						if (instanced is false)
						{
								instanced = true;
								NightVisionCameraModelHandEquipment instance = Instantiate(handModelPrefab, CameraMoveType.Instance.GetCamera().transform);
								instance.SetReference(this);

								// orientate with main camera
								instance.transform.rotation = Quaternion.identity;
								instance.transform.localPosition = Vector3.zero;
						}
				}

				public override void Interact(PlayerBehaviour sender)
				{
						// on ground or in hand
						if (IsTakenByPlayer)
						{

						}
				}
		}

		/// <summary>
		/// A model in hand to let night vision camera stay model, but hide rendering surface to avoid camera at face and in hand.
		/// Its only a reference for intacting with camera true model in placer camera
		/// </summary>
		[DisallowMultipleComponent]
		public class NightVisionCameraModelHandEquipment : EquipmentPlacable
		{
				[SerializeField] private MeshRenderer Renderer;
				private NightVisionCameraEquipment Camera;

				public override EquipmentInfo GetEquipmentInfo()
				{
						return Camera.GetEquipmentInfo();
				}

				protected override void OnPerformedDrop()
				{
						base.OnPerformedDrop();

						// this reference need to be the camera again
						Camera.transform.SetParent(null);
						Camera.DropItemRotated(Camera.User, true);

						Destroy(gameObject);
				}

				public override void Interact(PlayerBehaviour sender)
				{
						Camera.Interact(sender);
				}

				public void SetReference(NightVisionCameraEquipment camera)
				{
						this.Camera = camera;
						DisableVisible();
				}

				private void DisableVisible()
				{
						Renderer.enabled = false;
				}
		}
}