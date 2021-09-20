using System;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(Animator))]
		public class StepAnimationEventReference : MonoBehaviour
		{
				public event Action StepAnimationEvent;

				private Animator animator;

				private void Awake()
				{
						animator = GetComponent<Animator>();
				}

				public void OnStepEvent(string name)
				{
						StepAnimationEvent?.Invoke();
				}
		}
}