using Assets.Script.Components;

using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public abstract class HuntEventObject : MonoBehaviour
		{
				private bool IsHunt { get; set; }

				protected bool IsHuntingActive { get; private set; }

				protected bool IsHuntingActiveChanged { get; private set; }

				protected virtual void Update()
				{
						PerformHuntUpdate();
				}

				/// <summary>
				/// Is called INSIDE Update.<br/>
				/// Updates <see cref="IsHuntingActive"/> and <see cref="IsHuntingActiveChanged"/><br/>
				/// to call <see cref="OnHuntStarts"/> or <see cref="OnHuntEnds"/>
				/// </summary>
				protected void PerformHuntUpdate()
				{
						IsHuntingActive = Beans.InHunt;
						IsHuntingActiveChanged = IsHuntingActive != IsHunt;

						if (IsHuntingActiveChanged)
						{
								if (IsHunt)
								{
										OnHuntEnds();
								}
								else
								{
										OnHuntStarts();
								}
						}

						IsHunt = IsHuntingActive;
				}

				protected virtual void OnHuntStarts() { }

				protected virtual void OnHuntEnds() { }

		}
}