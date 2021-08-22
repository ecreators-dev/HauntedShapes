using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[Serializable]
		public class HuntingStateBean
		{
				public delegate void HuntingStateChangedHandler(bool huntActive);
				private static HuntingStateBean instance;

				public static event Action<HuntingStateBean> HuntingStateBeanCreatedEvent;

				public static HuntingStateBean Instance { get => instance ??= new HuntingStateBean(); }


				public event HuntingStateChangedHandler HuntingStateChangedEvent;

				protected HuntingStateBean() {

						Debug.Log("Hunting Bean CREATE");
						HuntingStateBeanCreatedEvent?.Invoke(this);
				}

				public bool InHunt { get; private set; }

				public void StartHunt()
				{
						InHunt = true;
						HuntingStateChangedEvent?.Invoke(InHunt);
						Debug.Log("Hunt ON");
				}

				public void StopHunt()
				{
						InHunt = false;
						HuntingStateChangedEvent?.Invoke(InHunt);
						Debug.Log("Hunt OFF");
				}
		}
}