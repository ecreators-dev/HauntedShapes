using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[Serializable]
		public class ShopData
		{
				public string name;
				public float price;
				public float sellPrice;
				public string description;
				public bool canToggleOnOff = true;
				public int toggleLimit = 5;
				public Texture2D shopImage;
		}
}