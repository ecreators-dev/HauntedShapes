using Assets.Script.Model;

using UnityEngine;

namespace Assets.Script.Behaviour.GhostTypes
{
		[RequireComponent(typeof(Rigidbody))]
		public class Candle : Equipment, IPforteTrigger
		{
				[SerializeField] private Transform lightOnOff;
				[SerializeField] private MeshRenderer visualMesh;
				[SerializeField] private Collider visualCollider;
				[SerializeField] private ShopParameters shopInfo;
				[SerializeField] private Animator animator;

				private bool reversedAnimation;
				private float animationSpeedEffector;

				public PforteTriggerTypeEnum TriggerType => PforteTriggerTypeEnum.KERZE;

				private Rigidbody RigidBody { get; set; }

				private bool IsActive => lightOnOff.gameObject.activeSelf;

				private void Awake()
				{
						RigidBody = GetComponent<Rigidbody>();
				}

				private void Start()
				{
						SetShopInfo(shopInfo, this);
						reversedAnimation = Random.Range(0, 100) % 2 == 0;
						animationSpeedEffector = Random.Range(0.9f, 2.5f);
						animator.SetFloat("Effector", animationSpeedEffector);
						animator.SetBool("Reverse", reversedAnimation);
				}

#if UNITY_EDITOR
				private void OnValidate()
				{
						this.ValidateInteractibleWithStaticSettings();
				}
#endif

				public void ToggleOff() => lightOnOff.gameObject.SetActive(false);

				public void ToggleOn() => lightOnOff.gameObject.SetActive(true);

				public void Drop()
				{
						// must be visible!
						if (visualMesh.enabled is false)
						{
								Debug.LogWarning("Cannot drop: is not visible");
								return;
						}

						RigidBody.isKinematic = false;
				}

				protected override void OnEquip()
				{
						MakeVisible();
						DisableFalling();
				}

				private void MakeVisible()
				{
						visualCollider.enabled = true;
						visualMesh.enabled = true;
				}

				private void DisableFalling() => RigidBody.isKinematic = true;

				protected override void OnInventory()
				{
						MakeInvisible();
				}

				private void MakeInvisible()
				{
						visualCollider.enabled = false;
						visualMesh.enabled = false;
				}

				protected override void OnOwnerOwnedEquipment()
				{
						// when paid and owner set: nothing
				}

				protected override void OnPickedUp()
				{
						DisableFalling();
				}

				protected override void PerformDrop()
				{
						Drop();
				}

				public override bool CanInteract(PlayerBehaviour sender)
				{
						return User == null || User == sender;
				}

				public override void Interact(PlayerBehaviour senderIgnored)
				{
						if (IsActive)
						{
								ToggleOff();
						}
						else
						{
								ToggleOn();
						}
				}

				protected override void OnHuntStart()
				{
						// stays on / off
				}

				protected override void OnHuntStop()
				{
						// stays on / off
				}

				protected override void OnEditMode_ToggleOn() => ToggleOn();

				protected override void OnEditMode_ToggleOff() => ToggleOff();
		}
}