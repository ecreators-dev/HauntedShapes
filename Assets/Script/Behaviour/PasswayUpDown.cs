using Assets.Script.Behaviour;

using UnityEngine;
using UnityEngine.Events;

public class PasswayUpDown : MonoBehaviour, IGate
{
		public UnityEvent AfterOpened;
		public UnityEvent AfterClosed;

		[SerializeField] private Animator animator;
		[SerializeField] private AudioClip openSound;
		[SerializeField] private AudioClip closeSound;
		[Range(0, 1)]
		[SerializeField] private float volume = 1;

		[Header("Animation Parameter Names")]
		[SerializeField] private string openAnimationTrigger;
		[SerializeField] private string closeAnimationTrigger;

		public bool IsOpened { get; private set; }

		public void OnOpenPlaySound()
		{
				if (openSound is { })
				{
						AudioSource.PlayClipAtPoint(openSound, transform.position, volume);
				}

				AfterOpened?.Invoke();
		}

		public void OnClosePlaySound()
		{
				if (openSound is { })
				{
						AudioSource.PlayClipAtPoint(openSound, transform.position, volume);
				}

				AfterClosed?.Invoke();
		}

		public void Open()
		{
				if (string.IsNullOrWhiteSpace(openAnimationTrigger))
						return;

				if (IsOpened)
						return;

				animator.SetTrigger(openAnimationTrigger);
				this.IsOpened = true;
		}

		public void Close()
		{
				if (string.IsNullOrWhiteSpace(closeAnimationTrigger))
						return;

				if (IsOpened is false)
						return;

				animator.SetTrigger(closeAnimationTrigger);
				this.IsOpened = false;
		}
}
