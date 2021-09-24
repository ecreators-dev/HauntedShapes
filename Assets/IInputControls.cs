using UnityEngine;

namespace Assets
{
		public interface IInputControls
		{
				Vector2 InputAxis { get; }
				float Horizonal { get; }
				float Vertical { get; }
				Vector2 MouseDelta { get; }
				float MouseDeltaX { get; }
				float MouseDeltaY { get; }

				bool ExitGameButton { get; }
				bool EditorStopCamera { get; }

				bool InteractButtonPressed { get; }

				bool DropEquipmentButtonPressed { get; }

				bool DebugHuntToggleOnOff { get; }

				bool IsEnabled { get; }

				bool CrosshairTargetInteractionButtonPressed { get; }

				bool CrouchButtonPressed { get; }

				/// <summary>
				/// Special settings to input controller for run
				/// 
				/// </summary>
				bool RunButtonPressedOrHold { get; }

				bool PlaceEquipmentButtonPressing { get; }

				void Disable();

				void Enable();
		}
}