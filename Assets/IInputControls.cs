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
				bool StopCameraRotationButton { get; }

				bool InteractWithEquipmentButton { get; }

				bool DropEquipmentButton { get; }

				bool DebugHuntingButton { get; }

				bool IsEnabled { get; }

				bool InteractWithCrosshairTargetButton { get; }

				bool CrouchButton { get; }

				/// <summary>
				/// Special settings to input controller for run
				/// 
				/// </summary>
				bool RunButton { get; }

				bool PlacingButton { get; }

				void Disable();

				void Enable();
		}
}