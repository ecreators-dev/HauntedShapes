using UnityEngine;

namespace Assets.Script.UI
{
		/// <summary>
		/// Keeps the same size as from game start in update
		/// </summary>
		public class KeepSizeChild : MonoBehaviour
		{
				private Vector3 size;
				private Transform myParent;

				private Transform Transform { get; set; }

				private void Awake()
				{
						Transform = transform;
				}

				private void Start()
				{
						size = Transform.localScale;
						myParent = transform.parent;
				}

				private void Update()
				{
						Transform.localScale = new Vector3
						{
								x = size.x / myParent.localScale.x,
								y = size.y / myParent.localScale.y,
								z = 1f
						};
				}
		}
}