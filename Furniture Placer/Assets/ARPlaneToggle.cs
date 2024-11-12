using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ARPlaneToggle : MonoBehaviour
{
    private ARPlaneManager arPlaneManager;
    private bool isVisible = true;
    public Button toggleButton;

    // Start is called before the first frame update
    void Start()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();

        if (arPlaneManager == null)
        {
            Debug.LogError("[Furniture Placer] ARPlaneManager component not found on this GameObject!");
        }
        else
        {
            Debug.Log("[Furniture Placer] ARPlaneManager found and assigned successfully.");
        }

        // Ensure the button is hooked up to the TogglePlanesVisibility method
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(TogglePlanesVisibility);
            Debug.Log("[Furniture Placer] Toggle button linked to visibility toggle.");
        }
        else
        {
            Debug.LogError("[Furniture Placer] No toggle button assigned!");
        }
    }

    // Update is called once per frame
    void Update()
    {
/*        // Detect touch input and log success
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Debug.Log("[Furniture Placer (Plane Toggle)] Touch input detected successfully.");
        }*/
    }

    public void TogglePlanesVisibility()
    {
        // Toggle the visibility state
        isVisible = !isVisible;

        // Debug: Log current visibility state
        Debug.Log("[Furniture Placer] Toggling AR planes visibility. New state: " + (isVisible ? "Visible" : "Invisible"));

        // Toggle all AR planes
        if (arPlaneManager != null)
        {
            foreach (var plane in arPlaneManager.trackables)
            {
                plane.gameObject.SetActive(isVisible);
                // Debug: Log each plane’s new state
                Debug.Log("[Furniture Placer] Plane " + plane.trackableId + " visibility set to " + isVisible);
            }
        }
    }
}
