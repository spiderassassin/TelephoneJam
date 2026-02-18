using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserFreakingVision : MonoBehaviour
{
    private LineRenderer _line1;
    private LineRenderer _line2;

    public Transform leftEyeOrigin;
    public Transform rightEyeOrigin;

    // Assign whichever camera is rendering your scene (usually Main Camera)
    public Camera playerCamera;

    void Start()
    {
        _line1 = transform.Find("Line1").GetComponent<LineRenderer>();
        _line2 = transform.Find("Line2").GetComponent<LineRenderer>();

        _line1.positionCount = 2;
        _line2.positionCount = 2;

        _line1.enabled = false;
        _line2.enabled = false;
    }

    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse down, enabling lasers");
            _line1.enabled = true;
            _line2.enabled = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Mouse up, disabling lasers");
            _line1.enabled = false;
            _line2.enabled = false;
        }
        

        if (_line1.enabled && _line2.enabled)
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();

            _line1.SetPosition(0, _line1.transform.InverseTransformPoint(transform.position));
            _line1.SetPosition(1, _line1.transform.InverseTransformPoint(mouseWorldPosition));
            
            _line2.SetPosition(0, _line2.transform.InverseTransformPoint(transform.position));
            _line2.SetPosition(1, _line2.transform.InverseTransformPoint(mouseWorldPosition));
            
            //TODO: ADD SFX HERE
            
            //TODO: ADD VFX at the hit position
            
            
        }
        
       
        

    }

    private Vector3 GetMouseWorldPosition()
    {
        // Cast a ray from the camera through the mouse position into the scene
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        // If the ray hits something, use that hit point
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
            return hit.point;
        }
        // draw a debug ray for visualization
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        // If nothing is hit, project the laser out a long distance
        return ray.GetPoint(100f);
    }
}