using System.Collections.Generic;

using UnityEngine;

namespace Assets.Script.Behaviour.Rope
{
		public class RopeRoot : MonoBehaviour
		{
				[SerializeField] private float rigidbodyMass = 1f;
				[SerializeField] private float colliderRadius = 0.1f;
				[SerializeField] private float jointSpring = 0.1f;
				[SerializeField] private float jointDamper = 5f;
				[SerializeField] private Vector3 rotationOffset;
				[SerializeField] private Vector3 positionOffset;

				protected List<Transform> CopySource { get; private set; } = new List<Transform>();
				protected List<Transform> CopyDestination { get; private set; } = new List<Transform>();
				protected GameObject RigidBodyContainer { get; private set; }

				private void Awake()
				{
						if (RigidBodyContainer == null)
								RigidBodyContainer = new GameObject("RopeRigidbodyContainer");

				}

				private void Start()
				{
						AddChildren(transform);
				}

				private void AddChildren(Transform parent)
				{
						for (int i = 0; i < parent.childCount; i++)
						{
								var representative = parent.GetChild(i);
								//rigidbody
								var childRigidbody = representative.GetComponent<Rigidbody>() ?? representative.gameObject.AddComponent<Rigidbody>();
								childRigidbody.useGravity = true;
								childRigidbody.isKinematic = false;
								childRigidbody.freezeRotation = true;
								childRigidbody.mass = rigidbodyMass;

								//collider
								var collider = representative.GetComponent<SphereCollider>() ?? representative.gameObject.AddComponent<SphereCollider>();
								collider.center = Vector3.zero;
								collider.radius = colliderRadius;

								//DistanceJoint
								var joint = representative.GetComponent<DistanceJoint3D>() ?? representative.gameObject.AddComponent<DistanceJoint3D>();
								joint.ConnectedRigidbody = parent;
								joint.DetermineDistanceOnStart = true;
								joint.Spring = jointSpring;
								joint.Damper = jointDamper;
								joint.DetermineDistanceOnStart = false;
								joint.Distance = Vector3.Distance(parent.position, representative.position);

								//add copy source
								CopySource.Add(representative.transform);
								CopyDestination.Add(representative);

								AddChildren(representative);
						}
				}

				private void Update()
				{
						for (int i = 0; i < CopySource.Count; i++)
						{
								CopyDestination[i].position = CopySource[i].position + positionOffset;
								CopyDestination[i].rotation = CopySource[i].rotation * Quaternion.Euler(rotationOffset);
						}
				}
		}
}