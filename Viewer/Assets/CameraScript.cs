using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]

public class CameraScript : MonoBehaviour
{

   // private float camera_dist = 10;
    //private Vector3 camera_offset;

    private float camera_angle = (Mathf.PI / 180) * 70;

    // Start is called before the first frame update
    void Start()
    {
        //camera_offset = transform.position;
        ChangeDistance(10f);
        transform.LookAt(Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        //transform.LookAt(Vector3.zero);
        //if (Input.GetMouseButtonDown(0))
        //{
        //    transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * camera_rotspeed_x, 0), Space.World);
        //    //Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * camera_rotspeed_x, Vector3.up);
        //    //Vector3 newpos = camTurnAngle * camera_offset;
        //    ////transform.position = Vector3.Slerp(transform.position, newpos, smooth_factor);
        //    //transform.position = newpos;
        //    //transform.LookAt(Vector3.zero);
        //}
    }

    public void ChangeDistance(float distance)
    {
        transform.position = new Vector3(0, distance * Mathf.Sin(camera_angle), distance * -1 * Mathf.Cos(camera_angle));
        transform.LookAt(Vector3.zero);
    }
}
