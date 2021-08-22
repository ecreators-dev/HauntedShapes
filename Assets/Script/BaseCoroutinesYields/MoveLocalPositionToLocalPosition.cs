using System;
using System.Collections;

using UnityEngine;

namespace Assets.Script.BaseCoroutinesYields
{
		public class MoveLocalPositionToLocalPosition : CoroutineCommand
		{
				private readonly Transform moveable;
				private readonly float toleranceDistance;
				private readonly float speed;
				private readonly Action stepAction;
				private readonly Action endAction;
				private readonly Func<Vector3> target;

				public MoveLocalPositionToLocalPosition(Transform moveable, float toleranceDistance, float speed, Action stepAction = null, Action endAction = null)
				{
						this.moveable = moveable;
						this.toleranceDistance = toleranceDistance;
						this.speed = speed;
						this.stepAction = stepAction;
						this.endAction = endAction;
				}

				public MoveLocalPositionToLocalPosition(Transform moveable, Vector3 fixedPosition, float speed = 3, Action stepAction = null, Action endAction = null, float toleranceDistance = 0.01f) 
						: this(moveable, toleranceDistance, speed, stepAction, endAction)
				{
						this.target = () => fixedPosition;
				}

				public MoveLocalPositionToLocalPosition(Transform moveable, Transform moveablePosition, float speed = 3, Action stepAction = null, Action endAction = null, float toleranceDistance = 0.01f) 
						: this(moveable, toleranceDistance, speed, stepAction, endAction)
				{
						this.target = () => moveablePosition.localPosition;
				}

				public static implicit operator Func<IEnumerator>(MoveLocalPositionToLocalPosition moving)
				{
						return moving.Run;
				}

				public override IEnumerator Run()
				{
						yield return new WaitForEndOfFrame();

						while (Vector3.Distance(moveable.localPosition, target()) > toleranceDistance)
						{
								moveable.localPosition = Vector3.Lerp(moveable.localPosition, target(), speed * Time.deltaTime);
								stepAction?.Invoke();
								yield return null;
						}
						moveable.localPosition = target();
						endAction?.Invoke();
						yield break;
				}
		}
}
