using Assets.Script.DLL.DebugArrowDrawer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[ExecuteInEditMode]
		public class DebugShowDirection : MonoBehaviour
		{
				private Transform Transform { get; set; }

				private void Awake()
				{
						Transform = transform;
				}

				private void OnDrawGizmos()
				{
						DrawArrow.ForDebug(Transform.position, Transform.forward, Color.blue);
				}
		}
}
