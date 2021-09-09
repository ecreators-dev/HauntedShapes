using UnityEngine;

namespace Assets.Script.DLL.DebugArrowDrawer
{
		public static class DrawArrow
		{
				public static void ForGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float length = 1)
				{
						Vector3 end = pos + direction * length;
						Gizmos.DrawLine(pos, end);

						Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
						Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
						Gizmos.DrawRay(end, right * arrowHeadLength);
						Gizmos.DrawRay(end, left * arrowHeadLength);
				}

				public static void ForGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float length = 1)
				{
						Gizmos.color = color;
						Vector3 end = pos + direction * length;
						Gizmos.DrawLine(pos, end);

						Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
						Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
						Gizmos.DrawRay(end, right * arrowHeadLength);
						Gizmos.DrawRay(end, left * arrowHeadLength);
				}

				public static void ForDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float length = 1)
				{
						Vector3 end = pos + direction * length;
						Debug.DrawLine(pos, end);

						Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
						Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
						Debug.DrawRay(end, right * arrowHeadLength);
						Debug.DrawRay(end, left * arrowHeadLength);
				}

				public static void ForDebug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float length = 1)
				{
						Vector3 end = pos + direction * length;
						Debug.DrawLine(pos, end, color);

						Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
						Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
						Debug.DrawRay(end, right * arrowHeadLength, color);
						Debug.DrawRay(end, left * arrowHeadLength, color);
				}
		}
}