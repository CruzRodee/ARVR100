using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AgentManager : MonoBehaviour
{
    List<ARAgent> agents;
    private ARTrackedImageManager trackedImageManager; // Make this private to find it in Start()
    private Vector3? beaconPosition = null;
    public float updateInterval = 0.1f;
    private float timer = 0f;

    // Reference to the level collider to check bounds
    public Collider levelCollider;

    // Flag to indicate if the beacon is visible
    private bool beaconVisible = false;

    // Start is called before the first frame update
    void Start()
    {
        // Find ARTrackedImageManager in the scene
        trackedImageManager = Object.FindFirstObjectByType<ARTrackedImageManager>();

        if (trackedImageManager != null)
        {
            trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
            Debug.Log("[Platform Level] ARTrackedImageManager found and subscribed.");
        }
        else
        {
            Debug.LogError("[Platform Level] ARTrackedImageManager not found in the scene!");
        }

        agents = new List<ARAgent>(GetComponentsInChildren<ARAgent>());
        Debug.Log("[Platform Level] Number of ARAgents found: " + agents.Count);

        // Check if the levelCollider is assigned
        if (levelCollider == null)
        {
            Debug.LogError("[Platform Level] Level collider not assigned!");
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
        Debug.Log("[Platform Level] OnTrackedImagesChanged triggered");

        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            Debug.Log("[Platform Level] Tracked image added: " + trackedImage.referenceImage.name);

            if (trackedImage.referenceImage.name == "cobblestone")
            {
                beaconPosition = trackedImage.transform.position;
                beaconVisible = true; // Set the flag to true when the beacon is first detected
                Debug.Log("[Platform Level] Beacon image detected at position: " + beaconPosition);
            }
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            Debug.Log("[Platform Level] Tracked image updated: " + trackedImage.referenceImage.name);

            if (trackedImage.referenceImage.name == "cobblestone" && trackedImage.trackingState == TrackingState.Tracking)
            {
                beaconPosition = trackedImage.transform.position;
                beaconVisible = true; // Set the flag to true when the beacon is updated and tracked
                Debug.Log("[Platform Level] Beacon image updated position: " + beaconPosition);
            }
            else if (trackedImage.referenceImage.name == "cobblestone" && trackedImage.trackingState != TrackingState.Tracking)
            {
                beaconPosition = null; // The beacon is not currently tracked
                beaconVisible = false; // Set the flag to false when the beacon is lost
                Debug.Log("[Platform Level] Beacon image lost tracking");
                StopAllAgents();
            }
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            Debug.Log("[Platform Level] Tracked image removed: " + trackedImage.referenceImage.name);

            if (trackedImage.referenceImage.name == "cobblestone")
            {
                beaconPosition = null;
                beaconVisible = false; // Set the flag to false when the beacon is removed
                Debug.Log("[Platform Level] Beacon image removed");
                StopAllAgents();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (beaconVisible && beaconPosition.HasValue && levelCollider != null && levelCollider.bounds.Contains(beaconPosition.Value))
        {
            Debug.Log("[Platform Level] Beacon position within level bounds: " + beaconPosition.Value);
            timer += Time.deltaTime;
            if (timer >= updateInterval)
            {
                Debug.Log("[Platform Level] Moving agents to position: " + beaconPosition.Value);
                MoveAllAgents(beaconPosition.Value);
                timer = 0f;
            }
        }
        else
        {
            if (timer >= updateInterval) // Only log stopping agents at the update interval
            {
                Debug.Log("[Platform Level] Beacon position outside level bounds or not detected. Stopping agents.");
                StopAllAgents();
                timer = 0f;
            }
        }
    }

    public void MoveAllAgents(Vector3 position)
    {
        if (agents.Count == 0)
        {
            Debug.LogWarning("[Platform Level] No agents found to move.");
            return;
        }

        foreach (ARAgent agent in agents)
        {
            Debug.Log("[Platform Level] Moving agent: " + agent.name + " to position: " + position);
            agent.MoveAgent(position);
        }
    }

    public void StopAllAgents()
    {
        if (agents.Count == 0)
        {
            Debug.LogWarning("[Platform Level] No agents found to stop.");
            return;
        }

        foreach (ARAgent agent in agents)
        {
            Debug.Log("[Platform Level] Stopping agent: " + agent.name);
            agent.StopAgent();
        }
    }
}
