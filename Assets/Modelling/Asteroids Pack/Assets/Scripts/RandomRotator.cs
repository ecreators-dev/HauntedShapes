using UnityEngine;
using System.Collections;

public class RandomRotator : MonoBehaviour
{
    [SerializeField]
    private float tumble;

    void Start()
    {
				Rigidbody body = GetComponent<Rigidbody>();
        if (body is null)
            return;
        
        body.angularVelocity = Random.insideUnitSphere * tumble;
    }
}