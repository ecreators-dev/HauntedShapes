using Assets.Script.Behaviour;

using System.Collections.Generic;

using UnityEngine;

namespace Assets.Script.Controller
{
		public class ShopSingleton : MonoBehaviour
		{
				[Tooltip("These items are used as prefabs!")]
				[SerializeField] private List<ShopParameters> shopItems;
		}
}