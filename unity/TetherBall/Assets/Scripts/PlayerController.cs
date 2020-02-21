using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 direction; //Direction in which to apply the movement force
    public float tetherDistance = 5f;
    public float tetherForceLateral = 5f;
    public float tetherForceVertical = 5f;
    
    public float speed = 5f;
    public float maxSpeed = 50f;

    private Vector3 startPos;
    private bool mouseDown = false;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        direction = Vector3.forward * speed;
        //velocity = Vector3.zero;
        //maxSpeed = .5f;
        startPos = transform.position;
        rb = gameObject.GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDown = true;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
        }

    }

    void FixedUpdate()
    {
        direction = Vector3.forward;

        if (mouseDown)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Get the 8th mask (which is the tetherable layer)
            int layerMask = 1 << 8;
            if (Physics.Raycast(ray, out hit, tetherDistance, layerMask))
            {

                Vector3 newDirVector = (hit.point - transform.position).normalized;
                newDirVector.x *= tetherForceLateral;
                newDirVector.y *= tetherForceVertical;
                newDirVector.z *= tetherForceVertical;

                direction += newDirVector;

                Debug.DrawLine(ray.origin, hit.point);
                Debug.Log(hit.point);
            }
        }

        rb.AddForce(direction * speed);
        if (rb.velocity.z > maxSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, maxSpeed);
        }

        //Old Code
        //Debug.Log(acceleration);
        //velocity += acceleration * Time.deltaTime * speed;
        //velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        //transform.position = transform.position + velocity;
        //transform.RotateAroundLocal(Vector3.right, 10 * Time.deltaTime);

        if (transform.position.z > 120f || transform.position.y < -20)
        {
            transform.position = startPos;
            direction = Vector3.forward;
            rb.velocity = Vector3.forward;
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
