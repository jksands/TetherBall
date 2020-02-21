using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 acceleration;
    public Vector3 velocity;
    public float maxSpeed;
    private Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        acceleration = Vector3.forward;
        velocity = Vector3.zero;
        maxSpeed = .5f;
        startPos = transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        transform.position = transform.position + velocity;
        transform.RotateAroundLocal(Vector3.right, 10 * Time.deltaTime);
        if (transform.position.z > 120f)
        {
            startPos.y = transform.position.y;
            transform.position = startPos;
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            Debug.Log("HIT OBSTACLE");
            transform.position = startPos;
        }
    }
}
