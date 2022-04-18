using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraControls : MonoBehaviour
{
    public List<Transform> targets = new List<Transform>();
    public Transform followRig, camRig;

    public float moveSpeed, camDistSpeed, rotationSpeed, minimumDist, maximumDist, maxCameraDist;

    public void LoadCamera()
    {
        followRig.LookAt(targets[0].position, Vector3.up);
        followRig.position = GetCenterPoint();
        followRig.GetChild(0).localPosition = new Vector3(-maxCameraDist * (GetNormalizedPlayerDistance() + 1), followRig.GetChild(0).localPosition.y, followRig.GetChild(0).localPosition.z);

        camRig.position = followRig.position;
        camRig.rotation = followRig.rotation;
        camRig.GetChild(0).localPosition = followRig.GetChild(0).localPosition;
    }

    private void LateUpdate()
    {
        if (targets.Count < 2)//less than 2
            return;

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] == null)
                targets.RemoveAt(i);
        }

        if (targets.Count == 1)//less than 2
            return;


        followRig.LookAt(new Vector3(targets[0].position.x, 9, targets[0].position.z), Vector3.up);
        followRig.position = GetCenterPoint();
        followRig.GetChild(0).localPosition = new Vector3(-maxCameraDist * (GetNormalizedPlayerDistance() + 1), followRig.GetChild(0).localPosition.y, followRig.GetChild(0).localPosition.z);

        camRig.position = Vector3.MoveTowards(camRig.position, followRig.position, moveSpeed * Time.deltaTime);
        camRig.rotation = Quaternion.Slerp(camRig.rotation, followRig.rotation, rotationSpeed * Time.deltaTime);
        camRig.GetChild(0).localPosition = Vector3.MoveTowards(camRig.GetChild(0).localPosition, followRig.GetChild(0).localPosition, camDistSpeed * Time.deltaTime);
    }

    //Gets the center point btw 2 targets
    Vector3 GetCenterPoint()
    {
        if (targets.Count == 1)
        {
            return new Vector3(targets[0].position.x, 9, targets[0].position.z);
        }

        var bounds = new Bounds(new Vector3(targets[0].position.x, 9, targets[0].position.z), Vector3.zero);

        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(new Vector3(targets[i].position.x, 9, targets[i].position.z));
        }

        return bounds.center;
    }

    float GetNormalizedPlayerDistance()
    {
        float diff = Vector3.Distance(targets[0].position, targets[1].position) - minimumDist;
        if (diff < 0)
            diff = 0;
        else if (diff > maximumDist)
            diff = maximumDist;
        
        return (diff/maximumDist);
    }
}
