using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ArrowTracker : MonoBehaviour
{
    // Start is called before the first frame update
    public ARTrackedImage trackedImage;
    public GameObject arCamera;
    Vector3 offset = new Vector3(0, 0.15f, 0);
    void Awake()
    {
        arCamera = GameObject.Find("AR Camera");
    }
    void Update()
    {
        if(trackedImage!= null)
        {
            // float movement = math.cos(Time.frameCount*Time.deltaTime);
            if(trackedImage.trackingState == TrackingState.Tracking)
            {
                // var deltaPos = trackedImage.gameObject.transform.position - arOrigion.transform.position;
                // transform.LookAt(trackedImage.gameObject.transform);
                // transform.position = arOrigion.transform.position + deltaPos.normalized * 0.08f;
            }else if(trackedImage.trackingState == TrackingState.Limited)
            {
                // Destroy(gameObject);
            }
            var deltaPos = trackedImage.gameObject.transform.position - arCamera.transform.position - offset;
            transform.LookAt(trackedImage.gameObject.transform);
            transform.position = arCamera.transform.position + deltaPos.normalized * 0.12f;
        }
    }
    public void SetFromToRotation(Vector3 fromLocation, Vector3 toLocation)
    {
        
    }
}
