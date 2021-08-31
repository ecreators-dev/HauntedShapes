using UnityEngine;

namespace Assets.Script.Behaviour.FirstPerson
{
		public interface ICrosshairUI
		{
				(bool actualHit, RaycastHit hit) GetHitPointLastUpdate();
				(bool hit, Vector3 point) UpdateHitPointFarAway(Camera camera);
		}
}