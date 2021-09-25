using Assets.Script.Behaviour;

using UnityEngine;

namespace Assets.Script.Controller
{
		[DisallowMultipleComponent]
		public class HuntActionControl : MonoBehaviour, IHuntAction
		{
				public bool IsHunting => HuntingStateBean.Instance.InHunt;

				private void Update()
				{
						if (this.InputControls().DebugHuntingButton)
						{
								HuntingStateBean hunt = HuntingStateBean.Instance;
								if (hunt.InHunt is false)
								{
										hunt.StartHunt();
								}
								else
								{
										hunt.StopHunt();
								}
						}
				}
		}
}