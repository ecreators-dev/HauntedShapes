using Assets.Script.Model;

using UnityEngine;

namespace Assets.Script.Behaviour.FirstPerson
{
		[RequireComponent(typeof(CameraFirstPerson))]
		[RequireComponent(typeof(MovementForPlayer))]
		[DisallowMultipleComponent]
		public class FirstPersonView : MonoBehaviour
		{
				[SerializeField] private CameraMoveType moveType = new CameraMoveType(CameraMoveType.TypeEnum.NOT_BUMPING);

				private CameraFirstPerson View { get; set; }
				private MovementForPlayer Move { get; set; }

				private void Awake()
				{
						View = GetComponent<CameraFirstPerson>();
						Move = GetComponent<MovementForPlayer>();
						moveType.FixCameraMissing();
				}

				private void Update()
				{
						UpdateCanRotate();
				}

				private void UpdateCanRotate()
				{
						// GameController toggles rotation on/off (only pre-release)
						// This component handles how to enable view on this setting

						IGameController gameController = this.GetGameController();
						if (gameController == null) return;
						View.enabled = !gameController.IsCameraRotateStop;
				}
		}
}
