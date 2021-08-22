using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// Keeps the Transform with local position, but rotated fixed in World
		/// </summary>
		[ExecuteAlways]
		public class WorldConstraint : MonoBehaviour
		{
				[SerializeField] private bool fixedTilt = true;
				[SerializeField] private bool fixedRotate = false;
				[SerializeField] private bool fixedRoll = true;

				private Transform Transform { get; set; }

				private void Awake()
				{
						Transform = transform;
				}

				// at least override
				private void LateUpdate()
				{
						Vector3 worldEuler = Transform.eulerAngles;
						if (fixedTilt)
						{
								worldEuler.x = 0;
						}
						if (fixedRotate)
						{
								worldEuler.y = 0;
						}
						if (fixedRoll)
						{
								worldEuler.z = 0;
						}
						Transform.eulerAngles = worldEuler;
				}
		}
}