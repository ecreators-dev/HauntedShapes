using System.Collections.Generic;

using UnityEngine;

namespace Assets
{
		[ExecuteAlways]
		[RequireComponent(typeof(LineRenderer))]
		public class LineRendererWithTransforms : MonoBehaviour
		{
				[SerializeField] private List<Transform> elements;
				private readonly List<float> elementFixtures = new List<float>();

				private LineRenderer LineRenderer { get; set; }

				private void Awake()
				{
						LineRenderer = GetComponent<LineRenderer>();
						LineRenderer.useWorldSpace = true;
				}

				private void Start()
				{
						LineRenderer.positionCount = elements.Count;

						Transform before = elements[0];
						for (int i = 1; i < elements.Count; i++)
						{
								Transform item = elements[i];
								elementFixtures.Add(Vector3.Distance(item.position, before.position));
								before = item;
						}
				}

				private void Update()
				{
						Transform before = elements[0];
						LineRenderer.SetPosition(0, before.position);
						for (int i = 1; i < elements.Count; i++)
						{
								Transform item = elements[i];

								Vector3 position = item.position;
								LineRenderer.SetPosition(i, position);
								before = item;
						}
				}
		}
}