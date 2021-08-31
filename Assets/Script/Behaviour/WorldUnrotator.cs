using UnityEngine;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// Keeps the Transform in World unrotated. That allowes a pickup item to stay unrotated during been picked up.
		/// </summary>
		[ExecuteAlways]
		public class WorldUnrotator : MonoBehaviour
		{
				[SerializeField] private bool unlockX = false;
				[SerializeField] private bool unlockY = false;
				[SerializeField] private bool unlockZ = false;

				private Transform Transform { get; set; }

				private void Awake()
				{
						Transform = transform;
				}

				private void LateUpdate()
				{
						Vector3 eulerOld = Transform.eulerAngles;
						Vector3 euler = Vector3.zero;
						if(unlockX) euler.x = eulerOld.x;
						if(unlockY) euler.y = eulerOld.y;
						if(unlockZ) euler.z = eulerOld.z;
						Transform.eulerAngles = euler;
				}
		}
}