using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 acceleration;
    public Vector3 velocity;
    public float maxSpeed;
    // Start is called before the first frame update
    void Start()
    {
        acceleration = Vector3.forward;
        velocity = Vector3.zero;
        maxSpeed = .1f;
        
    }

    // Update is called once per frame
    void Update()
    {
        velocity += acceleration * Time.deltaTime;
        Vector3.ClampMagnitude(velocity, maxSpeed);
        transform.position = transform.position + velocity;
        
    }
}
