using System;

namespace Assets.Script.Behaviour
{
		public interface IHeatUp
		{
				bool FullyCooled { get; }

				event Action<IHeatUp> HeatedUpEvent;
				event Action<IHeatUp> CooledDownEvent;

				void CoolDown();
				void HeatUp();
		}
}