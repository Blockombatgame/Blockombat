using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles camera auto movements
public class CameraController : MonoBehaviour
{
    public List<Transform> targets;
    public Vector3 offset;
    Vector3 velocity;
    public float smoothTime = 0.5f, slerpTime, smoothMoveTime;
    public float minZoom, maxZoom, zoomLimiter, upOffset;
    public Camera cam;
    public Transform midPoint, camRig;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (targets.Count == 0)
            return;

        if (targets[0] != null)
        {
            Move();
            Zoom();
        }
    }

    //Adjusts the camera focal lens to allow for more view of both players
    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }

    //Gets the distance btw the 2 players
    float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);

        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.size.x;
    }

    //Rotates the camera rig to the perfect view spot for both players
    private void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;

        midPoint.GetChild(0).localPosition = offset;
        transform.localPosition = midPoint.GetChild(0).localPosition;

        midPoint.LookAt(targets[0].position);
        midPoint.position = centerPoint;

        camRig.position = Vector3.MoveTowards(camRig.position, midPoint.position, smoothMoveTime * Time.deltaTime);
        camRig.rotation = Quaternion.Slerp(camRig.rotation, midPoint.rotation, slerpTime * Time.deltaTime);
    }

    //Gets the center point btw 2 targets
    Vector3 GetCenterPoint()
    {
        if (targets.Count == 1)
        {
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);

        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        transform.LookAt(bounds.center + new Vector3(0, upOffset, 0));

        return bounds.center;
    }
}
