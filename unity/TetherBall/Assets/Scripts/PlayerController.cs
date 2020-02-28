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

    public float maxGyroOffset = 3f;
    private Vector3 gyroOffset;

    private Vector3 startPos;
    private bool mouseDown = false;
    private Rigidbody rb;
    public float currentOffset;
    public float maxUnityOffset;
    public float prevOffset;
    public float moveBy;

    private RaycastHit hit;

    public Material debugBlack;
    public Material debugYellow;

    public float originX;
    private float originOff;
    private bool centralizing;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(hit);
        direction = Vector3.forward * speed;
        //velocity = Vector3.zero;
        //maxSpeed = .5f;
        startPos = transform.position;

        // Set the initial gyro offset
        gyroOffset = new Vector3(0, maxGyroOffset, 0);

        // Enable the gyroscope
        Input.gyro.enabled = true;

        currentOffset = 0;
        rb = gameObject.GetComponent<Rigidbody>();
        originX = transform.position.x;
        centralizing = false;
        maxUnityOffset = 3;
        
    }

    // Update is called once per frame
    void Update()
    {
       
        Vector3 temp = transform.position;
        moveBy = 0;
        //if (Mathf.Abs(rb.velocity.y) < .1f)
        //{
        //    originOff = originX - transform.position.x;
        //    centralizing = true;
        //}
        //else
        //    centralizing = false;
        if (centralizing)
        {
            // ReturnToOrigin();
        }
        else
        {
            if (Application.isEditor)
            {
                // Moved to FixedUpdate

                // Set to previous position
                if (Input.GetMouseButtonDown(0))
                {
                    mouseDown = true;
                    // centralizing = false;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    mouseDown = false;
                    hit = new RaycastHit();
                    rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
                    // centralizing = true;
                    // originOff = originX - transform.position.x;
                }
                //// Calculate offset
                if (Input.GetKey(KeyCode.A))
                {
                    currentOffset -= 10 * Time.deltaTime;
                    if (currentOffset < -maxUnityOffset)
                        currentOffset = -maxUnityOffset;


                    moveBy = currentOffset - prevOffset;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    currentOffset += 10 * Time.deltaTime;
                    if (currentOffset > maxUnityOffset)
                        currentOffset = maxUnityOffset;


                    moveBy = currentOffset - prevOffset;
                }
                else
                {
                    if (Mathf.Abs(rb.velocity.y) < .1f)
                    {
                        currentOffset = 0;
                        originOff = 10 * (originX - transform.position.x);
                        temp = ReturnToOrigin(temp);
                    }
                    //if (currentOffset != 0)
                    //{
                    //    currentOffset += ((0 - currentOffset) / Mathf.Abs(currentOffset)) * 10 * Time.deltaTime;
                    //    if (currentOffset > -.1f && currentOffset < .1f)
                    //        currentOffset = 0;
                    //}

                }
            }
            else
            {
                // We're in mobile so use dis shit
                // Handling Gyroscope code
                gyroOffset = GyroToUnity(Input.gyro.attitude) * new Vector3(0, maxGyroOffset, 0);
                if (Mathf.Abs(gyroOffset.x) < .1f && Mathf.Abs(rb.velocity.y) < .1f)
                {
                    currentOffset = 0;
                    originOff = 10 * (originX - transform.position.x);
                    temp = ReturnToOrigin(temp);
                }
                else
                {
                    currentOffset = gyroOffset.x;
                    Mathf.Clamp(currentOffset, -maxGyroOffset, maxGyroOffset);
                    moveBy = currentOffset - prevOffset;
                }
            }

            
            prevOffset = currentOffset;
            // Debug.Log(moveBy);
            // Apply it to the position
            temp.x += moveBy;
            rb.MovePosition(temp);
        }

    }

    Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    Vector3 ReturnToOrigin(Vector3 temp)
    {
        // temp = transform.position;
        float range = Mathf.Abs(temp.x - originX);
        if (range > .5f)
        {
            // Debug.Log("offset: " + originOff);
            temp.x += originOff * Time.deltaTime;
        }
        return temp;
        // rb.MovePosition(temp);
    }
    void FixedUpdate()
    {
        // rb.velocity = new Vector3(0, 0, rb.velocity.z);
        direction = Vector3.forward;

        if (Application.isEditor)
        {
            HandleUnity();
        }
        else
        {
            HandleMobile();
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

        if (transform.position.z > 1600f || 
            transform.position.y < -20)
        {
            transform.position = startPos;
            currentOffset = 0;
            direction = Vector3.forward;
            rb.velocity = Vector3.forward;
            transform.rotation = Quaternion.identity;
        }

    }

    void HandleUnity()
    {
        if (mouseDown)
        {
            HandleRaycast(Camera.main.ScreenPointToRay(Input.mousePosition));
        }
        else
        {
            mouseDown = false;
            hit = new RaycastHit();
            rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
            // centralizing = true;
            // originOff = originX - transform.position.x;
        }
    }
    void HandleMobile()
    {
        if (Input.touchCount > 0)
        {
            HandleRaycast(Camera.main.ScreenPointToRay(Input.touches[0].position));
        }
        else
        {
            mouseDown = false;
            hit = new RaycastHit();
            rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
            // centralizing = true;
            // originOff = originX - transform.position.x;
        }
    }
    void HandleRaycast(Ray ray)
    {
        // Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
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

           //  Debug.Log(hit.point);

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            // Debug.Log("HIT OBSTACLE");
            transform.position = startPos;
            direction = Vector3.forward;
            rb.velocity = Vector3.forward;
            currentOffset = 0;
            gyroOffset = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }
    }

    void OnRenderObject()
    {
        if (Input.touchCount>0 || mouseDown)
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

        //debugYellow.SetPass(0);
        //GL.Begin(GL.LINES);
        //GL.Vertex3(transform.position.x, transform.position.y, transform.position.z);
        //GL.Vertex3(transform.position.x + gyroOffset.x, transform.position.y + gyroOffset.y, transform.position.z + gyroOffset.z);
        //GL.End();
    }
}
