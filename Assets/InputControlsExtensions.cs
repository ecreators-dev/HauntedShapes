using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets
{
		public static class InputControlsExtensions
		{
				private const InputActionPhase DEFAULT_STATE = InputActionPhase.Canceled;
				private const InputActionPhase ACTIVE_STATE_CHANGE = InputActionPhase.Performed;
				private const InputActionPhase DEFAULT_STATE_WAITING = InputActionPhase.Waiting;

				private static float lastTimeHold;

				public static bool IsButtonHold(this InputAction buttonAction, ref InputActionPhase value)
				{
						bool hold = buttonAction.triggered;
						// once per second log - not too much logging here
						if (hold && (lastTimeHold == 0 || (Time.time - lastTimeHold) >= 1f))
						{
								Debug.Log($"{buttonAction.name}: button held");
								lastTimeHold = Time.time;
						}
						return hold;
				}

				public static bool IsButtonReleased(this InputAction buttonAction, ref InputActionPhase value)
				{
						bool triggered = buttonAction.triggered;
						if (triggered)
						{
								Debug.Log($"{buttonAction.name}: button released");
						}
						return triggered;
				}

				public static bool IsButtonDown(this InputAction buttonAction, ref InputActionPhase value)
				{
						bool triggered = buttonAction.triggered;
						if (triggered)
						{
								Debug.Log($"{buttonAction.name}: button down");
						}
						return triggered;
				}
		}
}