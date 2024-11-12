using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI; // For handling UI elements

public class BeaconVisibilityUIManager : MonoBehaviour
{
    private ARTrackedImageManager trackedImageManager;
    public GameObject beaconWarningUI; // Assign your UI message GameObject in the Inspector

    // Flag to check if the beacon has been detected at least once
    private bool beaconDetectedOnce = false;

    void Start()
    {
        // Find ARTrackedImageManager in the scene
        trackedImageManager = GetComponent<ARTrackedImageManager>();

        if (trackedImageManager != null)
        {
            trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
            Debug.Log("[BeaconVisibility] ARTrackedImageManager found and subscribed.");
        }
        else
        {
            Debug.LogError("[BeaconVisibility] ARTrackedImageManager not found on this GameObject!");
        }

        // Ensure the UI message is hidden at the start
        if (beaconWarningUI != null)
        {
            beaconWarningUI.SetActive(false);
        }
        else
        {
            Debug.LogError("[BeaconVisibility] BeaconWarningUI not assigned!");
        }
    }

    void OnDestroy()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            Debug.Log("[BeaconVisibility] Tracked image added: " + trackedImage.referenceImage.name);

            if (trackedImage.referenceImage.name == "cobblestone")
            {
                beaconDetectedOnce = true; // Set the flag to true after the first detection
                if (beaconWarningUI != null) beaconWarningUI.SetActive(false);
            }
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            Debug.Log("[BeaconVisibility] Tracked image updated: " + trackedImage.referenceImage.name);

            if (trackedImage.referenceImage.name == "cobblestone" && trackedImage.trackingState == TrackingState.Tracking)
            {
                if (beaconDetectedOnce && beaconWarningUI != null) beaconWarningUI.SetActive(false);
            }
            else if (trackedImage.referenceImage.name == "cobblestone" && trackedImage.trackingState != TrackingState.Tracking)
            {
                Debug.Log("[BeaconVisibility] Beacon image lost tracking");
                if (beaconDetectedOnce && beaconWarningUI != null) beaconWarningUI.SetActive(true);
            }
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            Debug.Log("[BeaconVisibility] Tracked image removed: " + trackedImage.referenceImage.name);

            if (trackedImage.referenceImage.name == "cobblestone")
            {
                Debug.Log("[BeaconVisibility] Beacon image removed");
                if (beaconDetectedOnce && beaconWarningUI != null) beaconWarningUI.SetActive(true);
            }
        }
    }
}
