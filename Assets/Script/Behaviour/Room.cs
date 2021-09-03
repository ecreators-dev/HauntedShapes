using Assets.Script.Behaviour.Thermal;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(Collider))]
		[DisallowMultipleComponent]
		public class Room : MonoBehaviour
		{
				[SerializeField] private float temperatureDegreesCelsius = 16;
				[SerializeField] private string roomName;

				private readonly Dictionary<int, ThermalBody> triggeredThermalBodys = new Dictionary<int, ThermalBody>();
				private float ghostThermal;
				private float otherThermal;

				private GhostRoom SelfGhostRoom { get; set; }

				public bool IsGhostRoom { get; private set; }

				public string RoomName => roomName;

				/// <summary>
				/// Temperature this room heated or cooled by inner objects
				/// </summary>
				public virtual float Temperature => temperatureDegreesCelsius + ghostThermal + otherThermal;

				private Transform Transform { get; set; }

				private void Awake()
				{
						Transform = transform;
				}

				private void Start()
				{
						SelfGhostRoom = this as GhostRoom;
						IsGhostRoom = SelfGhostRoom is { };
				}

				private void Update()
				{
						if (IsGhostRoom)
						{
								int count = SelfGhostRoom.GetGhosts().Count;
								ghostThermal = Mathf.Max(-36, count * -21);
						}

						otherThermal = 0;
						foreach (ThermalBody other in triggeredThermalBodys.Values)
						{
								// temperature by distance
								otherThermal += other.GetTemperature(Transform);
						}
						Debug.Log($"Room Temperature {Temperature} °C, Ghosts: {ghostThermal} °C, Other: {otherThermal} °C: '{gameObject.name}'");
				}

				private void OnTriggerEnter(Collider other)
				{
						if (other.gameObject is { } &&
								other.TryGetComponent(out ThermalBody body))
						{
								triggeredThermalBodys[other.GetInstanceID()] = body;
						}
				}

				private void OnTriggerExit(Collider other)
				{
						if (other.gameObject is { } &&
								other.TryGetComponent(out ThermalBody body))
						{
								triggeredThermalBodys.Remove(other.GetInstanceID());
						}
				}
		}
}