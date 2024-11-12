using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class VirtualButton : MonoBehaviour
{
    private ARTrackedImage trackedImage;

    [SerializeField]
    private UnityEvent onTrackedImageLimited;

    // Start is called before the first frame update
    void Start()
    {
        // Attempt to get ARTrackedImage from the parent GameObject
        trackedImage = transform.parent.GetComponent<ARTrackedImage>();

        if (trackedImage == null)
        {
            Debug.LogError("[VirtualButton] ARTrackedImage component is missing on the parent GameObject. Please ensure the parent GameObject has ARTrackedImage attached.");
        }
        else
        {
            Debug.Log("[VirtualButton] Successfully found ARTrackedImage component on the parent GameObject.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (trackedImage != null)
        {
            Debug.Log("[VirtualButton] Current tracking state: " + trackedImage.trackingState);

            if (trackedImage.trackingState == TrackingState.Limited)
            {
                Debug.Log("[VirtualButton] Tracking state is Limited. Invoking onTrackedImageLimited event.");
                onTrackedImageLimited.Invoke();
            }
            else if (trackedImage.trackingState == TrackingState.Tracking)
            {
                Debug.Log("[VirtualButton] Tracking state is Tracking.");
            }
            else if (trackedImage.trackingState == TrackingState.None)
            {
                Debug.Log("[VirtualButton] No tracking information available.");
            }
        }
        else
        {
            Debug.LogError("[VirtualButton] trackedImage is null. Make sure ARTrackedImage is set up correctly on the parent.");
        }
    }
}
