using Assets.Script.Components;

using System;

namespace Assets.Script.Behaviour
{
		public abstract class HuntEventObject : Interactible
		{
				protected HuntingStateBean HuntInfo { get; private set; }

				protected bool IsHuntActive => HuntInfo?.InHunt ?? false;
				private bool IsHunt { get; set; }

				protected virtual void Update()
				{
						HuntInfo ??= HuntingStateBean.Instance;

						if (IsHunt != IsHuntActive)
						{
								if (IsHuntActive)
								{
										OnHuntActivate();
								}
								else
								{
										OnHuntEnding();
								}
						}
						IsHunt = IsHuntActive;
				}

				protected abstract void OnHuntEnding();

				protected abstract void OnHuntActivate();

		}
}