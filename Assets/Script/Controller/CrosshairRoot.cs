using UnityEngine;

namespace Assets.Script.Controller
{
		public class CrosshairRoot : MonoBehaviour
		{
				[SerializeField] private Color interaction = Color.white;
				[SerializeField] private Color locked = Color.yellow;
				[SerializeField] private Color broken = Color.red;

				public Color GetColor(ActionEnum actionType)
				{
						switch (actionType)
						{
								case ActionEnum.INTERACTIBLE:
										return interaction;
								case ActionEnum.LOCKED:
										return locked;
								case ActionEnum.BROKEN:
										return broken;
								default: 
										return Color.white;
						}
				}

				public enum ActionEnum
				{
						INTERACTIBLE,
						LOCKED,
						BROKEN
				}
		}
}