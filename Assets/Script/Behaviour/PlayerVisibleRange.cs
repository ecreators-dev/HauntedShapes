
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(SphereCollider))]
		public class PlayerVisibleRange : MonoBehaviour
		{
				[SerializeField] private MeshRenderer[] makeVisibleOnEnter;
				[SerializeField] private MeshRenderer[] makeInvisibleOnEnter;

				private readonly ISet<PlayerBehaviour> enteredPlayers = new HashSet<PlayerBehaviour>();

				private bool IsVisible { get; set; }
				private Collider Collider { get; set; }

				private void Awake()
				{
						Collider = GetComponent<Collider>();
				}

				private void Start()
				{
						var renderer = GetComponent<Renderer>();
						if (renderer != null)
						{
								renderer.enabled = false;
						}
						Collider.isTrigger = true;
				}

				private void OnTriggerEnter(Collider other)
				{
						if (other.TryGetComponent(out PlayerBehaviour player) && enteredPlayers.Add(player) && IsVisible is false)
						{
								IsVisible = true;
								ChangeVisibilityOnRenderers();
						}
				}

				private void OnTriggerExit(Collider other)
				{
						if (other.TryGetComponent(out PlayerBehaviour player) && enteredPlayers.Remove(player) && enteredPlayers.Count == 0 && IsVisible is true)
						{
								IsVisible = false;
								ChangeVisibilityOnRenderers();
						}
				}

				private void ChangeVisibilityOnRenderers()
				{
						foreach (var item in makeVisibleOnEnter)
						{
								item.enabled = IsVisible;
						}
						foreach (var item in makeInvisibleOnEnter)
						{
								item.enabled = !IsVisible;
						}
				}
		}
}