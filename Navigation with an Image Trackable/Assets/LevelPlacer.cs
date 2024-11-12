using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class LevelPlacer : MonoBehaviour
{
    public GameObject levelPrefab; // Drag your level prefab here in the Inspector
    private GameObject placedLevel;
    private ARRaycastManager raycastManager;

    void Start()
    {
        // Get the ARRaycastManager component
        raycastManager = GetComponent<ARRaycastManager>();
        if (raycastManager == null)
        {
            Debug.LogError("ARRaycastManager not found on the GameObject.");
        }
        else
        {
            Debug.Log("ARRaycastManager successfully found.");
        }
    }

    void Update()
    {
        // Use mouse click for desktop testing
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse button clicked.");

            if (placedLevel != null)
            {
                Debug.Log("Level has already been placed. Ignoring further clicks.");
                return;
            }

            Vector2 mousePosition = Input.mousePosition;
            Debug.Log("Mouse position: " + mousePosition);

            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            // Perform the raycast
            if (raycastManager.Raycast(mousePosition, hits, TrackableType.PlaneWithinPolygon))
            {
                Debug.Log("Raycast hit detected. Number of hits: " + hits.Count);

                // Get the first hit point
                Pose hitPose = hits[0].pose;
                Debug.Log("Hit pose position: " + hitPose.position);

                // Instantiate the level prefab at the hit position
                placedLevel = Instantiate(levelPrefab, hitPose.position, hitPose.rotation);
                Debug.Log("Level prefab instantiated at position: " + hitPose.position);
            }
            else
            {
                Debug.Log("No raycast hit detected. Clicked position did not intersect with any detected planes.");
            }
        }
    }
}
