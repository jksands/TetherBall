using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 direction; //Direction in which to apply the movement force
    public float tetherDistance = 5f;
    public float tetherForceLateral = 5f;
    public float tetherForceVertical = 5f;
    
    public float speed = 20f;
    public float maxSpeed = 10f;

    private Vector3 startPos;
    private bool mouseDown = false;
    private Rigidbody rb;
    private float maxLeft;
    private float maxRight;
    public float currentOffset;
    public float prevOffset;
    public float moveBy;

    private RaycastHit hit;

    public Material debugBlack;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(hit);
        direction = Vector3.forward * speed;
        //velocity = Vector3.zero;
        //maxSpeed = .5f;
        startPos = transform.position;
        maxLeft = -3;
        maxRight = 3;
        currentOffset = 0;
        rb = gameObject.GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        // Set to previous position
        if (Input.GetMouseButtonDown(0))
        {
            mouseDown = true;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
            hit = new RaycastHit();
            rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
        }
        // Calculate offset
        if (Input.GetKey(KeyCode.A))
        {
            currentOffset -= 10 * Time.deltaTime;
            if (currentOffset < maxLeft)
                currentOffset = maxLeft;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            currentOffset += 10 * Time.deltaTime;
            if (currentOffset > maxRight)
                currentOffset = maxRight;
        }
        else
        {
            if (currentOffset != 0)
            {
                currentOffset += ((0 - currentOffset) / Mathf.Abs(currentOffset)) * 10 * Time.deltaTime;
                if (currentOffset > -.1f && currentOffset < .1f)
                    currentOffset = 0;
            }

        }
        moveBy = currentOffset - prevOffset;
        prevOffset = currentOffset;
        Debug.Log(moveBy);
        // Apply it to the position
        Vector3 temp = transform.position;
        temp.x += moveBy;
        rb.MovePosition(temp);

    }

    void FixedUpdate()
    {
        // rb.velocity = new Vector3(0, 0, rb.velocity.z);
        direction = Vector3.forward;

        if (mouseDown)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // RaycastHit hit;
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

        if (transform.position.z > 500f || transform.position.y < -20)
        {
            transform.position = startPos;
            currentOffset = 0;
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
            direction = Vector3.forward;
            rb.velocity = Vector3.forward;
            currentOffset = 0;
        }
    }

    void OnRenderObject()
    {
        if (mouseDown)
        {
            if (hit.distance > 0)
            {

                debugBlack.SetPass(0);
                GL.Begin(GL.LINES); // LINE
                GL.Vertex3(transform.position.x, transform.position.y, transform.position.z);
                GL.Vertex3(hit.point.x, hit.point.y, hit.point.z);
                GL.End();
            }
        }
    }
}
