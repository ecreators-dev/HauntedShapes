using System;

using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.Behaviour.Animals
{
		public class NavMeshAgentTurnInsantStrategy
		{
				private NavMeshAgent Agent { get; set; }

				public void Start(NavMeshAgent agent)
				{
						Agent = agent;
						Agent.updateRotation = false;
				}

				internal void LateUpdate()
				{
						if (Agent.velocity.sqrMagnitude > Mathf.Epsilon)
						{
								Agent.transform.rotation = Quaternion.LookRotation(Agent.velocity.normalized);
						}
				}
		}
}