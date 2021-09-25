using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets
{
		public static class InputControlsExtensions
		{
				private static float lastTimeHold;

				public static bool IsButtonHold(this InputAction buttonAction)
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

				public static bool IsButtonReleased(this InputAction buttonAction)
				{
						bool triggered = buttonAction.triggered;
						if (triggered)
						{
								Debug.Log($"{buttonAction.name}: button released");
						}
						return triggered;
				}

				public static bool IsButtonDown(this InputAction buttonAction)
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