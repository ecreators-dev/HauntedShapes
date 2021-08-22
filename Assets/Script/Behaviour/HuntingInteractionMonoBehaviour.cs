
using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public abstract class HuntingInteractionMonoBehaviour : MonoBehaviour
		{
				public HuntingStateBean Hunting { get; private set; }

				protected virtual void Start()
				{
						HuntingStateBean.HuntingStateBeanCreatedEvent -= OnCreate;
						HuntingStateBean.HuntingStateBeanCreatedEvent += OnCreate;

						void OnCreate(HuntingStateBean obj)
						{
								RegisterHuntingStateBean(obj);
						}

						RegisterHuntingStateBean(null);

						void OnHuntStateChanged(bool huntActive)
						{
								if (huntActive)
								{
										OnHuntStart();
								}
								else
								{
										OnHuntStopped();
								}
						}

						void RegisterHuntingStateBean(HuntingStateBean obj)
						{
								Hunting = obj ?? HuntingStateBean.Instance;
								Hunting.HuntingStateChangedEvent -= OnHuntStateChanged;
								Hunting.HuntingStateChangedEvent += OnHuntStateChanged;
						}
				}

				protected virtual void OnHuntStopped()
				{ }

				protected virtual void OnHuntStart()
				{ }


				[ContextMenu("Hunt Start (Playmode)")]
				public void StartHunt()
				{
						HuntingStateBean.Instance.StartHunt();
				}

				[ContextMenu("Hunt Stop (Playmode)")]
				public void StopHunt()
				{
						HuntingStateBean.Instance.StopHunt();
				}
		}
}