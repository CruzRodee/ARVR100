using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class AnchorPlacer : MonoBehaviour 
{
    ARAnchorManager anchorManager;
    ARPointCloudManager pointCloudManager; // Reference to ARPointCloudManager

    [SerializeField] private GameObject grayCubePrefab; 
    [SerializeField] private GameObject redCubePrefab;
    [SerializeField] private GameObject blueCubePrefab;
    [SerializeField] private Button button1; // gray cube button
    [SerializeField] private Button button2; // red cube button
    [SerializeField] private Button button3; // blue cube button

    [SerializeField] private Slider distanceSlider; // slider for spawn distance
    [SerializeField] private TextMeshProUGUI sliderValueText;

    [SerializeField] private GameObject settingsPanel; // The panel for settings
    [SerializeField] private Button settingsButton; // The button that toggles the settings panel
    [SerializeField] private Toggle pointCloudToggle; // Toggle for point cloud
    [SerializeField] private Button deleteAllButton; // Button to delete all anchored objects
    
    // List to store all spawned objects
    private List<GameObject> anchoredObjects = new List<GameObject>();

    [SerializeField] private GameObject prefabToAnchor; // active prefab
    [SerializeField] private float forwardOffset = 2f;

    // Start is called before the first frame update
    void Start()
    {
        anchorManager = GetComponent<ARAnchorManager>();
        pointCloudManager = GetComponent<ARPointCloudManager>();

        // set gray cube as initial prefab
        SelectPrefab(grayCubePrefab, button1);

        // assign button click events
        button1.onClick.AddListener(() => SelectPrefab(grayCubePrefab, button1));
        button2.onClick.AddListener(() => SelectPrefab(redCubePrefab, button2));
        button3.onClick.AddListener(() => SelectPrefab(blueCubePrefab, button3));

        // set the initial forwardOffset based on the default slider value and add a listener
        forwardOffset = distanceSlider.value;
        UpdateForwardOffset(forwardOffset);
        distanceSlider.onValueChanged.AddListener(UpdateForwardOffset);

        // Hide settings panel by default
        settingsPanel.SetActive(false);

        // Assign the toggle functionality to the settings button
        settingsButton.onClick.AddListener(ToggleSettingsPanel);

        // Point Cloud Toggle event listener
        pointCloudToggle.onValueChanged.AddListener(TogglePointCloud);

        // Assign the delete all functionality to the delete button
        deleteAllButton.onClick.AddListener(DeleteAllAnchoredObjects);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began){

            // Check if the touch is over a UI element (button)
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                Debug.Log("[AnchorPlacer] Touch is over a UI element, not spawning a cube.");
                return; // Skip the rest of the code if the touch is over a button
            }

            Debug.Log("[AnchorPlacer] Screen touched, starting raycast.");
            Touch touch = Input.GetTouch(0); // get first touch
            Ray ray = Camera.main.ScreenPointToRay(touch.position); // create ray from touch position
            RaycastHit hit;  // store result of raycast

            // check if ray hit something
            if (Physics.Raycast(ray, out hit)) 
            {
                Debug.Log("[AnchorPlacer] Raycast hit something: " + hit.collider.gameObject.name);

                // Get the ARAnchor component from the hit object's parent (which is the anchor)
                ARAnchor anchor = hit.collider.GetComponentInParent<ARAnchor>();
                if (anchor != null)
                {
                    Debug.Log("[AnchorPlacer] Hit object is an anchored object. Destroying it.");
                    Destroy(anchor.gameObject);  // Destroy the entire anchor (parent) object
                }
                else
                {
                    Debug.Log("[AnchorPlacer] Hit object is not an anchored object.");
                }
            }
            else
            {
                Debug.Log("[AnchorPlacer] Raycast did not hit anything. Placing new anchored object.");
                //If there was no object hit, place new anchored object
                Vector3 spawnPos = ray.GetPoint(forwardOffset);
                AnchorObject(spawnPos); // place new anchored object
            }

            
        }
    }

    public void AnchorObject(Vector3 worldPos)
    {
        Debug.Log("[AnchorPlacer] Placing a new anchor object at position: " + worldPos);
        GameObject newAnchor = new GameObject("NewAnchor");
        newAnchor.transform.parent = null;
        newAnchor.transform.position = worldPos;
        newAnchor.AddComponent<ARAnchor>();

        GameObject obj = Instantiate(prefabToAnchor, newAnchor.transform);

        // adds a collider if the prefab doesn't already have one.
        if (obj.GetComponent<Collider>() == null)
        {
            BoxCollider collider = obj.AddComponent<BoxCollider>();
            collider.size = obj.GetComponent<Renderer>().bounds.size; // Adjust collider size to match the object's size
        }

        obj.transform.localPosition = Vector3.zero;

        // Add the newly created anchor object to the list
        anchoredObjects.Add(newAnchor);
        Debug.Log("[AnchorPlacer] Added new anchor object to list: " + newAnchor.name);
    }

    //method for selecting prefab and handling button states
    public void SelectPrefab(GameObject selectedPrefab, Button selectedButton)
    {
        prefabToAnchor = selectedPrefab;

        // Disable the selected button
        button1.interactable = (selectedButton != button1);
        button2.interactable = (selectedButton != button2);
        button3.interactable = (selectedButton != button3);

        Debug.Log("[AnchorPlacer] Selected prefab: " + selectedPrefab.name);
    }

    public void UpdateForwardOffset(float value)
    {
        forwardOffset = value;
        sliderValueText.text = $"Distance: {value:F2}";
        Debug.Log("[AnchorPlacer] Updated forwardOffset to: " + forwardOffset);
    }

    public void ToggleSettingsPanel()
    {
        Debug.Log("[AnchorPlacer] Toggling Settings Panel");
        bool isActive = settingsPanel.activeSelf;
        settingsPanel.SetActive(!isActive);
        Debug.Log("[AnchorPlacer] Active Status: " + !isActive);
    }

    // Method for toggling the point cloud visibility
    public void TogglePointCloud(bool isEnabled)
    {
        Debug.Log("[AnchorPlacer] Point cloud visualization toggled: " + isEnabled);
        if (pointCloudManager != null)
        {
            pointCloudManager.SetTrackablesActive(isEnabled);
        }
    }
    public void DeleteAllAnchoredObjects()
    {
        Debug.Log("[AnchorPlacer] Deleting all anchored objects.");

        // Loop through and destroy each anchored object
        foreach (GameObject obj in anchoredObjects)
        {
            Destroy(obj);
        }

        // Clear the list after deleting
        anchoredObjects.Clear();
    }
}
