using Assets.Script.Model;

using UnityEngine;

namespace Assets.Script.Behaviour.GhostTypes
{
		[RequireComponent(typeof(Rigidbody))]
		public class Candle : MonoBehaviour, IPforteTrigger, IToggleInteractable
		{
				public Transform lightParent;

				public PforteTriggerTypeEnum TriggerType => PforteTriggerTypeEnum.KERZE;

				public string GameObjectName => this.GetGameObjectName();
				public string ImplementationTypeName => this.GetImplementationTypeName();

				public bool IsPickable => transform.parent == null;

				private Rigidbody RigidBody { get; set; }

				private void Awake()
				{
						RigidBody = GetComponent<Rigidbody>();
				}

#if UNITY_EDITOR
				private void OnValidate()
				{
						this.ValidateInteractibleWithStaticSettings();
				}
#endif

				public void ToggleOff()
				{
						lightParent.gameObject.SetActive(false);
				}

				public void ToggleOn()
				{
						lightParent.gameObject.SetActive(true);
				}

				public void TouchClickUpdate()
				{
						// pick up
				}

				public void TouchOverUpdate()
				{
						// hover
				}

				public void Drop()
				{
						RigidBody.isKinematic = false;
				}

				public void OnPickup(PlayerBehaviour player)
				{
						RigidBody.isKinematic = true;
				}
		}
}