using UnityEngine;

namespace Assets.Script.Controller
{
		public sealed class MouseCursorController : MonoBehaviour, IMouseCursor
		{
				[SerializeField] private bool showCursor = false;
				[SerializeField] private bool lockedCursor = true;

				public static IMouseCursor Instance { get; private set; }

				public void HideCursorLocked()
				{
						showCursor = false;
						lockedCursor = true;
				}

				public void ShowCursorUnlocked()
				{
						showCursor = true;
						lockedCursor = false;
				}

				private void Awake()
				{
						if (Instance is null)
						{
								Instance = this;
						}
						else
						{
								Destroy(this);
						}
				}

				private void LateUpdate()
				{
						UpdateMouseCursor();
				}

				private void UpdateMouseCursor()
				{
						// none = outof game window
						// confined = within game window only
						// locked = center in game window : no move
						Cursor.lockState = lockedCursor ? CursorLockMode.Locked : CursorLockMode.None;
						Cursor.visible = showCursor;
				}
		}
}
