
using UnityEngine;

namespace Assets.Script.Behaviour
{
		public class Room : MonoBehaviour
		{
				public BoxCollider basePlaneGhostorbs;

				private void Awake()
				{
						// no trigger, no force - for Y only
						basePlaneGhostorbs.enabled = false;
				}
		}
}