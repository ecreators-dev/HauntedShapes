using UnityEngine;

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

		bool PlayerToggleEquipmentOnOff { get; }

		bool PlayerDropEquipment { get; }

		bool DebugHuntToggleOnOff { get; }

		bool IsEnabled { get; }

		void Disable();
		void Enable();
}