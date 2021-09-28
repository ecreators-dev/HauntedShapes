using UnityEngine;

namespace Assets.Script.Behaviour.Rope
{
		[RequireComponent(typeof(Rigidbody))]
		public class DistanceJoint3D : MonoBehaviour
		{
				[SerializeField] private Transform connectedRigidbody;
				[SerializeField] private bool determineDistanceOnStart = true;
				[SerializeField] private float distance;
				[SerializeField] private float spring = 0.1f;
				[SerializeField] private float damper = 5f;

				protected Rigidbody Rigidbody { get; private set; }
				public Transform ConnectedRigidbody { get => connectedRigidbody; set => connectedRigidbody = value; }
				public float Spring { get => spring; set => spring = value; }
				public bool DetermineDistanceOnStart { get; set; }
				public float Damper { get => damper; set => damper = value; }
				public float Distance { get => distance; set => distance = value; }

				private void Awake()
				{
						Rigidbody = GetComponent<Rigidbody>();
				}

				private void Start()
				{
						if (determineDistanceOnStart && connectedRigidbody != null)
								distance = Vector3.Distance(Rigidbody.position, connectedRigidbody.position);
				}

				private void FixedUpdate()
				{

						var connection = Rigidbody.position - connectedRigidbody.position;
						var distanceDiscrepancy = distance - connection.magnitude;

						Rigidbody.position += distanceDiscrepancy * connection.normalized;

						var velocityTarget = connection + (Rigidbody.velocity + Physics.gravity * spring);
						var projectOnConnection = Vector3.Project(velocityTarget, connection);
						Rigidbody.velocity = (velocityTarget - projectOnConnection) / (1 + damper * Time.fixedDeltaTime);
				}
		}
}