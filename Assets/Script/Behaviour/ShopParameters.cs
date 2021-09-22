using Assets.Script.Components;

using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[CreateAssetMenu(fileName = "Shop Item", menuName = "Game/Shop/New Item")]
		public class ShopParameters : ScriptableObject
		{
				[SerializeField] private Equipment prefab;
				[Min(1)]
				[SerializeField] private int slotSize = 1;

				[SerializeField] private string displayName;
				[SerializeField] private string description;

				[Min(0)]
				[SerializeField] private float cost = 10;
				[Min(0)]
				[SerializeField] private float sellPrice;

				[SerializeField] private bool canBeSold = true;

				[SerializeField] private bool isElectrial = true;
				[SerializeField] private bool canToggleOff = true;

				[SerializeField] private float activeSeconds;
				[SerializeField] private float coolDownSeconds;

				[SerializeField] private int playerLevelMinimum;

				[SerializeField] private Texture dragImage;
				[SerializeField] private Texture shopImageSmall;
				[SerializeField] private Texture shopImageBig;

				[SerializeField] private AudioClip switchOnSound;
				[SerializeField] private AudioClip switchOffSound;
				[SerializeField] private AudioClip huntingSound;
				[SerializeField] private AnimationWithSoundTrigger switchOnAnimation;
				[SerializeField] private AnimationWithSoundTrigger switchOffAnimation;
				[SerializeField] private AnimationWithSoundTrigger huntAnimation;

				public string DisplayName { get => displayName; }
				public string Description { get => description; }
				public float Cost { get => cost; }
				public float SellPrice { get => sellPrice; }
				public bool IsElectrial { get => isElectrial; }
				public bool CanToggleOff { get => canToggleOff; }
				public float ActiveSeconds { get => activeSeconds; }
				public float CoolDownSeconds { get => coolDownSeconds; }
				public int PlayerLevelMinimum { get => playerLevelMinimum; }
				public bool CanBeSold { get => canBeSold; }
				public Texture DragImage { get => dragImage; }
				public Texture ShopImageSmall { get => shopImageSmall; }
				public Texture ShopImageBig { get => shopImageBig; }
				public AudioClip SwitchOnSound { get => switchOnSound; }
				public AudioClip SwitchOffSound { get => switchOffSound; }
				public AudioClip HuntingSound { get => huntingSound; }
				public AnimationWithSoundTrigger SwitchOnAnimation { get => switchOnAnimation; }
				public AnimationWithSoundTrigger SwitchOffAnimation { get => switchOffAnimation; }
				public AnimationWithSoundTrigger HuntAnimation { get => huntAnimation; }
				public Equipment Prefab { get => prefab; }

				public EObjectType ObjectType => Prefab.ObjectType;

				public int SlotSize => slotSize;

				public void SetPrefab(Equipment instancePrefab)
				{
						if (instancePrefab.ShopInfo != this)
						{
								throw new ArgumentException("The equipment must have this identical shop info, first!");
						}
						prefab = instancePrefab;
				}

				public bool IsEqualTo(ShopParameters shopItem)
				{
						return DisplayName.Equals(shopItem.DisplayName);
				}
		}
}