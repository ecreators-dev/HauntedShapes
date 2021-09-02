using Assets.Script.Controller;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[ExecuteAlways]
		public class PlacementCheck : MonoBehaviour
		{
				[SerializeField] private bool showMeshes = false;

				[SerializeField] private MeshRenderer up;
				[SerializeField] private MeshRenderer down;
				[SerializeField] private MeshRenderer right;
				[SerializeField] private MeshRenderer left;
				[SerializeField] private MeshRenderer front;
				[SerializeField] private MeshRenderer back;

				private Transform Transform { get; set; }

				[Header("Current Contact Type")]
				[ReadOnly]
				[SerializeField] private PlacementEnum placementType;
				private HitCheck[] sides;
				private HitCheck? placementResult;

				public PlacementEnum GetPlacementType(out HitCheck? check)
				{
						check = placementResult;
						return placementType;
				}

				private void Awake()
				{
						Transform = transform;
				}

				private void Start()
				{
						sides = new[] {
						new HitCheck(front.transform, PlacementEnum.WALL),
						new HitCheck(back.transform, PlacementEnum.WALL),
						new HitCheck(right.transform, PlacementEnum.WALL),
						new HitCheck(left.transform, PlacementEnum.WALL),
						new HitCheck(down.transform, PlacementEnum.FLOOR),
						new HitCheck(up.transform, PlacementEnum.CEILING)
						};
				}

				private void FixedUpdate()
				{
						placementType = GetPlacement();
				}

				private void Update()
				{
						KeepWorldRotation();

						SetEnableOnMeshRenderers();
				}

				private void KeepWorldRotation()
				{
						Transform.rotation = Quaternion.identity;
				}

				private void SetEnableOnMeshRenderers()
				{
						front.enabled = showMeshes;
						back.enabled = showMeshes;
						right.enabled = showMeshes;
						left.enabled = showMeshes;
						up.enabled = showMeshes;
						down.enabled = showMeshes;
				}

				private PlacementEnum GetPlacement()
				{
						HitCheck? result = null;
						foreach (var hit in sides)
						{
								hit.TryHit(Transform);
								if (hit.Hit)
								{
										if (result.HasValue is false || result.Value.HitDistance > hit.HitDistance)
										{
												result = hit;
										}
								}
						}
						placementResult = result;
						return result.HasValue ? result.Value.PlacementType : PlacementEnum.NONE;
				}

				public struct HitCheck
				{
						public HitCheck(Transform reference, PlacementEnum placementType)
						{
								Reference = reference;
								PlacementType = placementType;
								HitDistance = 0;
								Hit = false;
								Raycast = default;
						}
						private Transform Reference { get; set; }

						public float HitDistance { get; private set; }

						public bool Hit { get; private set; }

						public RaycastHit Raycast { get; private set; }

						public PlacementEnum PlacementType { get; }

						public void TryHit(Transform source)
						{
								Hit = Physics.Raycast(new Ray(source.position, source.position - Reference.position),
										out var raycast, 1f, 0, QueryTriggerInteraction.Ignore);
								Raycast = raycast;
								HitDistance = Hit ? Raycast.distance : float.MaxValue;
						}
				}
		}
}