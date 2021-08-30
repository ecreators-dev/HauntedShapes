using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(Light))]
		public class DotsProjectorEquipment : Equipment
		{
				[SerializeField] private MeshRenderer meshRenderer;
				[SerializeField] private Rigidbody RigidBody;
				[SerializeField] private ShopParameters shopInfo;

				private Light Light { get; set; }

				private void Awake()
				{
						Light = GetComponent<Light>();
				}

				private void Start()
				{
						SetShopInfo(shopInfo, this);
				}

				public override bool CanInteract(PlayerBehaviour sender)
				{
						return false;
				}

				public override void Interact(PlayerBehaviour sender)
				{
						// not enabled yet!
				}

				protected override void OnEquip()
				{
						DisableFalling();
						MakeVisible();
				}

				protected override void OnHuntStart()
				{
						// nothing, may be less light?
				}

				protected override void OnHuntStop()
				{
						// nothing, light normal?
				}

				protected override void OnInventory()
				{
						MakeInvisible();
				}

				private void MakeInvisible()
				{
						meshRenderer.enabled = false;
				}

				private void MakeVisible()
				{
						meshRenderer.enabled = true;
				}

				protected override void OnOwnerOwnedEquipment()
				{

				}

				protected override void OnPickedUp()
				{
						DisableFalling();
				}

				private void DisableFalling()
				{
						RigidBody.isKinematic = true;
				}

				private void EnableFalling()
				{
						RigidBody.isKinematic = false;
				}

				protected override void PerformDrop()
				{
						EnableFalling();
						MakeVisible();
				}

				protected override void OnEditMode_ToggleOn() => this.ToggleOn();

				protected override void OnEditMode_ToggleOff() => ToggleOff();

				private void ToggleOn() => Light.enabled = true;

				private void ToggleOff() => Light.enabled = false;
		}
}