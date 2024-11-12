using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TrackedImageHandler : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public GameObject beaconPrefab; // Assign your virtual beacon prefab in the Inspector
    public float hoverHeight = 0.5f; // Height above the image
    public float rotationSpeed = 5f; // Rotation speed in degrees per second

    private Dictionary<string, GameObject> spawnedBeacons = new Dictionary<string, GameObject>();

    void OnEnable()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
            Debug.Log("[TrackedImageHandler] ARTrackedImageManager event subscribed.");
        }
        else
        {
            Debug.LogError("[TrackedImageHandler] ARTrackedImageManager is not assigned!");
        }
    }

    void OnDisable()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
            Debug.Log("[TrackedImageHandler] ARTrackedImageManager event unsubscribed.");
        }
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            Debug.Log("[TrackedImageHandler] Image added: " + trackedImage.referenceImage.name);
            SpawnBeacon(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            Debug.Log("[TrackedImageHandler] Image updated: " + trackedImage.referenceImage.name + " | Tracking State: " + trackedImage.trackingState);
            UpdateBeacon(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            Debug.Log("[TrackedImageHandler] Image removed: " + trackedImage.referenceImage.name);
            RemoveBeacon(trackedImage);
        }
    }

    private void SpawnBeacon(ARTrackedImage trackedImage)
    {
        if (trackedImage.referenceImage == null)
        {
            Debug.LogWarning("[TrackedImageHandler] Reference image is null during spawn.");
            return;
        }

        string imageName = trackedImage.referenceImage.name;
        Debug.Log("[TrackedImageHandler] Attempting to spawn beacon for image: " + imageName);

        if (!spawnedBeacons.ContainsKey(imageName))
        {
            Vector3 hoverPosition = trackedImage.transform.position + new Vector3(0, hoverHeight, 0);
            GameObject beacon = Instantiate(beaconPrefab, hoverPosition, trackedImage.transform.rotation);
            beacon.transform.parent = trackedImage.transform; // Make the beacon follow the tracked image
            spawnedBeacons[imageName] = beacon;
            Debug.Log("[TrackedImageHandler] Beacon spawned for image: " + imageName);
        }
        else
        {
            Debug.Log("[TrackedImageHandler] Beacon already exists for image: " + imageName);
        }
    }

    private void UpdateBeacon(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        Debug.Log("[TrackedImageHandler] Updating beacon for image: " + imageName);

        if (spawnedBeacons.ContainsKey(imageName))
        {
            GameObject beacon = spawnedBeacons[imageName];
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                beacon.SetActive(true);
                Vector3 hoverPosition = trackedImage.transform.position + new Vector3(0, hoverHeight, 0);
                beacon.transform.position = hoverPosition;
                beacon.transform.Rotate(Vector3.up, rotationSpeed);
                Debug.Log("[TrackedImageHandler] Beacon active and updated for image: " + imageName);
            }
            else
            {
                beacon.SetActive(false);
                Debug.Log("[TrackedImageHandler] Beacon deactivated for image: " + imageName);
            }
        }
        else
        {
            Debug.LogWarning("[TrackedImageHandler] No beacon found to update for image: " + imageName);
        }
    }

    private void RemoveBeacon(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        Debug.Log("[TrackedImageHandler] Removing beacon for image: " + imageName);

        if (spawnedBeacons.ContainsKey(imageName))
        {
            Destroy(spawnedBeacons[imageName]);
            spawnedBeacons.Remove(imageName);
            Debug.Log("[TrackedImageHandler] Beacon removed for image: " + imageName);
        }
        else
        {
            Debug.LogWarning("[TrackedImageHandler] No beacon to remove for image: " + imageName);
        }
    }
}
