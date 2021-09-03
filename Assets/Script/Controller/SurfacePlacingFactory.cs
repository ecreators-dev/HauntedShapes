using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public sealed class SurfacePlacingFactory :
				SurfacePlacingFactory.IFactoryFindSurface,
				SurfacePlacingFactory.IFactoryShowObject,
				SurfacePlacingFactory.IFactoryUpdateVisual
		{
				private static readonly LayerMask LAYER_MASK_EVERYTHING = ~0;
				private readonly Transform movingObject;
				private readonly Vector3 movingObjectUp;
				private RaycastHit testHitInfo;
				private bool found;
				private GameObject visual;

				public bool IsPlaced { get; private set; }

				private SurfacePlacingFactory(Transform movingObject, Vector3 up)
				{
						this.movingObject = movingObject;
						this.movingObjectUp = up;
				}


				public static IFactoryFindSurface FindSurface(Transform target, Vector3 up) => new SurfacePlacingFactory(target, up);

				bool IFactoryFindSurface.CheckSurface(Camera cameraSource)
				{
						float w = cameraSource.scaledPixelWidth;
						float h = cameraSource.scaledPixelHeight;
						Ray ray = cameraSource.ScreenPointToRay(new Vector2(w, h) * 0.5f);
						return found = Physics.Raycast(ray, out testHitInfo, float.MaxValue, LAYER_MASK_EVERYTHING, QueryTriggerInteraction.Ignore);
				}

				IFactoryUpdateVisual IFactoryShowObject.CheckPlacingExecute(Predicate<RaycastHit> tester)
				{
						if (found && tester.Invoke(testHitInfo))
						{
								IsPlaced = true;
								movingObject.transform.position = testHitInfo.point;
								movingObject.transform.rotation = Quaternion.FromToRotation(movingObjectUp, testHitInfo.normal);
						}
						return this;
				}

				IFactoryShowObject IFactoryFindSurface.ShowObject(GameObject visual, Vector3 up)
				{
						this.visual = visual;
						visual.SetActive(found);
						if (found)
						{
								visual.transform.position = testHitInfo.point;
								visual.transform.rotation = Quaternion.FromToRotation(up, testHitInfo.normal);
						}
						return this;
				}

				void IFactoryUpdateVisual.UpdateVisual()
				{
						if (!found || found && IsPlaced)
						{
								found = false;
						}
							
						visual?.SetActive(found);
				}

				public interface IFactoryFindSurface : IFactoryUpdateVisual
				{
						bool IsPlaced { get; }

						bool CheckSurface(Camera cameraSource);

						IFactoryShowObject ShowObject(GameObject visual, Vector3 up);
				}

				public interface IFactoryShowObject
				{
						IFactoryUpdateVisual CheckPlacingExecute(Predicate<RaycastHit> tester);
				}

				public interface IFactoryUpdateVisual
				{
						void UpdateVisual();
				}
		}
}