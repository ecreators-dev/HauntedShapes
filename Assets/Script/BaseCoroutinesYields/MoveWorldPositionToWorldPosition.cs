using System;
using System.Collections;

using UnityEngine;

namespace Assets.Script.BaseCoroutinesYields
{
		public class MoveWorldPositionToWorldPosition : CoroutineCommand
		{
				private readonly Transform moveable;
				private readonly float toleranceDistance;
				private readonly float speed;
				private readonly Action stepAction;
				private readonly Action endAction;
				private readonly Func<Vector3> target;

				public MoveWorldPositionToWorldPosition(Transform moveable, float toleranceDistance, float speed, Action stepAction = null, Action endAction = null)
				{
						this.moveable = moveable;
						this.toleranceDistance = toleranceDistance;
						this.speed = speed;
						this.stepAction = stepAction;
						this.endAction = endAction;
				}

				public MoveWorldPositionToWorldPosition(Transform moveable, Vector3 fixedPosition, float speed = 3, Action stepAction = null, Action endAction = null, float toleranceDistance = 0.01f)
						: this(moveable, toleranceDistance, speed, stepAction, endAction)
				{
						this.target = () => fixedPosition;
				}

				public MoveWorldPositionToWorldPosition(Transform moveable, Transform moveablePosition, float speed = 3, Action stepAction = null, Action endAction = null, float toleranceDistance = 0.01f)
						: this(moveable, toleranceDistance, speed, stepAction, endAction)
				{
						this.target = () => moveablePosition.position;
				}

				public static implicit operator Func<IEnumerator>(MoveWorldPositionToWorldPosition moving)
				{
						return moving.Run;
				}

				public override IEnumerator Run()
				{
						yield return new WaitForEndOfFrame();

						while (Vector3.Distance(moveable.position, target()) > toleranceDistance)
						{
								moveable.position = Vector3.Lerp(moveable.position, target(), speed * Time.deltaTime);
								stepAction?.Invoke();
								yield return null;
						}
						moveable.position = target();
						endAction?.Invoke();
						yield break;
				}
		}
}
