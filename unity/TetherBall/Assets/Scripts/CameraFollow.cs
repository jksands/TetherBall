using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Camera mc;
    public Transform playerTransform;
    private Vector3 prevPosition;
    public float zOffset;
    public float yOffset;
    // Start is called before the first frame update
    void Start()
    {
        // prevPosition = playerTransform.position;
        // prevPosition.z -= 6;
        // prevPosition.y += 5;
        // mc.transform.position = prevPosition;
        // Debug.Log(mc.transform.position.z);
        zOffset = 6;
        yOffset = 3;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 temp = playerTransform.position;
        temp.z -= zOffset;
        temp.y += yOffset;
        mc.transform.position = temp;
        // mc.transform.position = mc.transform.position + (playerTransform.position - prevPosition);
        // prevPosition = mc.transform.position;
    }
}
