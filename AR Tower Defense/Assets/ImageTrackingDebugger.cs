using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageTrackingDebugger : MonoBehaviour
{
    private ARTrackedImageManager trackedImageManager;

    void OnEnable()
    {
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            Debug.Log($"[ImageTrackingDebugger] Added: {trackedImage.referenceImage.name} - Tracking State: {trackedImage.trackingState}");
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            Debug.Log($"[ImageTrackingDebugger] Updated: {trackedImage.referenceImage.name} - Tracking State: {trackedImage.trackingState}");
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            Debug.Log($"[ImageTrackingDebugger] Removed: {trackedImage.referenceImage.name}");
        }
    }
}
