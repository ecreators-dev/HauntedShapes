using UnityEngine;

namespace Assets.Script.Behaviour.Thermal
{
		[DisallowMultipleComponent]
		public class ThermalBody : MonoBehaviour
		{
				[SerializeField] private float temperatureDegreesCelsius;
				[SerializeField] private Transform heatMaximumSpot;
				[Min(0)]
				[SerializeField] private float heatingSeconds;
				[Min(0)]
				[SerializeField] private float coolingSeconds;
				[Min(0)]
				[SerializeField] private float maxDistance = 5;

				private Room roomWithin;

				private Transform Transform { get; set; }

				private float currentTemperature;
				private float targetTemperature;
				private float temperatureChangeTimeout;
				private float temperatureChangeTimeoutMax;
				private float oldTemperature;

				public Room Room => roomWithin;

				/// <summary>
				/// Temperature only self
				/// </summary>
				public float Temperature => currentTemperature;

				/// <summary>
				/// Temperature affected by distance
				/// </summary>
				public float GetTemperature(Transform caller)
				{
						return currentTemperature * GetDistance(caller);
				}

				private float GetDistance(Transform caller)
				{
						Transform point = heatMaximumSpot ?? Transform;
						float distance = Vector3.Distance(caller.position, point.position);
						return Mathf.Max(0, 1 - distance / maxDistance);
				}

				private void Awake()
				{
						Transform = transform;
				}

				private void UpdateRoomWithin()
				{
						roomWithin = null;
						var colliders = Physics.OverlapSphere(Transform.position, 15);
						foreach (Collider collider in colliders)
						{
								if (collider.TryGetComponent(out Room room))
								{
										roomWithin = room;
										break;
								}
						}
				}

				private void Update()
				{
						var roomOld = roomWithin;
						UpdateRoomWithin();

						// thermals heatup with distance to other thermals!

						// temperature by room & ghosts
						// change °C Timer: once:
						if (roomOld != roomWithin)
						{
								// enter room
								if (roomWithin is { })
								{
										OnEnterRoom();
								}
								// leave room
								else
								{
										OnLeaveRoom();
								}
								oldTemperature = currentTemperature;
						}

						// during change time: update temperature
						if (temperatureChangeTimeout > 0)
						{
								temperatureChangeTimeout -= Time.deltaTime;
								float progress = 1f - Mathf.Min(1f, temperatureChangeTimeout / temperatureChangeTimeoutMax);
								float deltaTemperature = targetTemperature - oldTemperature;
								float temperatureChangeRelative = deltaTemperature * progress;
								currentTemperature = oldTemperature + temperatureChangeRelative;
								Debug.Log($"Thermal Body Temperature: {currentTemperature} °C of max. {temperatureDegreesCelsius} °C: '{gameObject.name}'");
						}
				}

				private void OnLeaveRoom()
				{
						targetTemperature = temperatureDegreesCelsius;
						temperatureChangeTimeout = coolingSeconds;
						temperatureChangeTimeoutMax = temperatureChangeTimeout;
						Debug.Log("Thermal Body leaves room");
				}

				private void OnEnterRoom()
				{
						targetTemperature = roomWithin.Temperature + temperatureDegreesCelsius;
						temperatureChangeTimeout = heatingSeconds;
						temperatureChangeTimeoutMax = temperatureChangeTimeout;
						Debug.Log("Thermal Body enter room");
				}
		}
}