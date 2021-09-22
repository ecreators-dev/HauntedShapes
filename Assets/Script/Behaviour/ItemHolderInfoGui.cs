using Assets.Script.Components;

using TMPro;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(ItemHolder))]
		public class ItemHolderInfoGui : MonoBehaviour
		{
				[SerializeField] private TMP_Text infoText;
				[SerializeField] private TMP_Text timerText;

				private IItemHolder ItemHolder { get; set; }

				private void Awake()
				{
						ItemHolder = GetComponent<ItemHolder>();
				}

				private void Update()
				{
						if(ItemHolder.CurrentItem is IEquipment item)
						{
								EquipmentInfo info = item.GetEquipmentInfo();
								if (info is { })
								{
										ShowEquipmentInfo(info);
								}
								else
								{
										HideEquipmentInfo();
								}
						}
						else
						{
								HideEquipmentInfo();
						}
				}

				private void ShowEquipmentInfo(EquipmentInfo info)
				{
						if (info.Text != null)
						{
								infoText.gameObject.SetActive(true);
								infoText.text = info.Text;
						}
						else
						{
								infoText.gameObject.SetActive(false);
						}

						if (info.TimerText != null)
						{
								timerText.gameObject.SetActive(true);
								timerText.text = info.TimerText;
						}
						else
						{
								timerText.gameObject.SetActive(false);
						}
				}

				private void HideEquipmentInfo()
				{
						infoText.gameObject.SetActive(false);
						timerText.gameObject.SetActive(false);
				}
		}
}