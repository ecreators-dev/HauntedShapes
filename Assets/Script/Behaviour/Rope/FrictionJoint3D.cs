using UnityEngine;

namespace Assets.Script.Behaviour.Rope
{
		public class FrictionJoint3D : MonoBehaviour
		{

				[Range(0, 1)]
				[SerializeField] private float friction;

				protected Rigidbody Rigidbody { get; private set; }

				private void Awake()
				{
						Rigidbody = GetComponent<Rigidbody>();
				}

				private void FixedUpdate()
				{
						Rigidbody.velocity = Rigidbody.velocity * (1 - friction);
						Rigidbody.angularVelocity = Rigidbody.angularVelocity * (1 - friction);
				}
		}
}