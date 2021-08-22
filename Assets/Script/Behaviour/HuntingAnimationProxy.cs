
using UnityEngine;

namespace Assets.Script.Behaviour
{
		public partial class FlashlightBehaviour
		{
				public sealed class HuntingAnimationProxy
				{
						private readonly Animator animation;
						private readonly string triggerNameBoolean;

						public HuntingAnimationProxy(Animator animation, string triggerNameBoolean)
						{
								this.animation = animation;
								this.triggerNameBoolean = triggerNameBoolean;
						}

						public void StartHuntAnimation()
						{
								animation.SetBool(triggerNameBoolean, true);
						}

						public void StopHuntAnimation()
						{
								animation.SetBool(triggerNameBoolean, false);
						}
				}
		}
}