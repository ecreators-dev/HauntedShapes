using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets.Script.Components
{
		public abstract class Interactible : MonoBehaviour
		{
				[SerializeField] protected EObjectType identifier = EObjectType.UNDEFINED;

				public EObjectType ObjectType => identifier;

				public abstract bool CanInteract(GameObject sender);

				public abstract void Interact(GameObject sender);
		}
}
