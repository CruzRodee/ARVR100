using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARSubsystems;

public class AnchorPlacer : MonoBehaviour
{
    //Anchor Manager
    ARAnchorManager anchorManager;
    ARRaycastManager raycastManager;

    // Furniture Items
    [SerializeField] private GameObject item1Prefab;
    [SerializeField] private GameObject item2Prefab;
    [SerializeField] private GameObject item3Prefab;
    [SerializeField] private Button button1; // item 1 button
    [SerializeField] private Button button2; // item 2 button
    [SerializeField] private Button button3; // item 3 button

    [SerializeField] private GameObject prefabToAnchor; // active prefab
    [SerializeField] private float forwardOffset = 2f;

    // List to store raycast hits
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // List to track placed furniture objects
    private List<GameObject> placedFurniture = new List<GameObject>();

    // Track the selected object for repositioning
    private GameObject selectedObject;
    private bool isDragging = false;


    // Start is called before the first frame update
    void Start()
    {
        anchorManager = GetComponent<ARAnchorManager>();
        raycastManager = GetComponent<ARRaycastManager>();

        // set item 1 as initial prefab
        SelectPrefab(item1Prefab, button1);

        // Subscribe to rotation events from GestureManager
        if (GestureManager.Instance != null)
        {
            GestureManager.Instance.OnRotate += HandleRotateGesture;
            Debug.Log("[AnchorPlacer] Subscribed to rotation events.");
        }
        else
        {
            Debug.LogWarning("[AnchorPlacer] GestureManager instance not found.");
        }


        // assign button click events
        button1.onClick.AddListener(() => SelectPrefab(item1Prefab, button1));
        button2.onClick.AddListener(() => SelectPrefab(item2Prefab, button2));
        button3.onClick.AddListener(() => SelectPrefab(item3Prefab, button3));

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Check if the touch is over a UI element (button)
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                Debug.Log("[AnchorPlacer] Touch is over a UI element, not interacting with objects.");
                return; // Skip the rest of the code if the touch is over a button
            }

            // Handling touch began phase
            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log("[AnchorPlacer] Screen touched, performing raycast.");
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                // Perform raycast to detect if a furniture object is hit
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("[AnchorPlacer] Raycast hit something: " + hit.collider.gameObject.name);

                    // Check if the object hit is part of the placed furniture
                    if (placedFurniture.Contains(hit.collider.gameObject))
                    {
                        selectedObject = hit.collider.gameObject;
                        isDragging = true; // Begin dragging the object
                        Debug.Log("[AnchorPlacer] Selected furniture for repositioning: " + selectedObject.name);
                        return;
                    }
                }

                // If no furniture object was hit, check for AR plane to place new furniture
                if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinBounds))
                {
                    Pose hitPose = hits[0].pose;
                    // Check if the plane is horizontal by examining the normal vector of the plane
                    if (Mathf.Abs(hitPose.up.y) > 0.9f)  // Adjust the threshold as needed (0.9 means it is almost horizontal)
                    {
                        AnchorObject(hitPose.position);
                    }
                    else
                    {
                        Debug.Log("[AnchorPlacer] The detected plane is not horizontal. Ignoring this plane.");
                    }
                }
                else
                {
                    Debug.Log("[AnchorPlacer] No AR plane detected. Not placing object.");
                }

            }
            else if (touch.phase == TouchPhase.Moved && isDragging && selectedObject != null)
            {
                // Continue to move the object if dragging
                MoveObjectAlongPlane(selectedObject);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                // Stop dragging when the touch ends
                if (isDragging)
                {
                    Debug.Log("[AnchorPlacer] Stopped dragging object.");
                    isDragging = false;
                    selectedObject = null;
                }
            }
        }
    }





    public void AnchorObject(Vector3 worldPos)
    {
        Debug.Log("[AnchorPlacer] Placing a new anchor object at position: " + worldPos);

        // Ensure that a prefab is selected
        if (prefabToAnchor == null)
        {
            Debug.LogError("[AnchorPlacer] No prefab selected for anchoring!");
            return;
        }

        GameObject newAnchor = new GameObject("NewAnchor");
        newAnchor.transform.parent = null;
        newAnchor.transform.position = worldPos;
        newAnchor.AddComponent<ARAnchor>();

        GameObject obj = Instantiate(prefabToAnchor, newAnchor.transform);

        // Ensure the object has a collider
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            // If the object does not have a collider, add one
            if (obj.GetComponent<Collider>() == null)
            {
                BoxCollider collider = obj.AddComponent<BoxCollider>();
                collider.size = renderer.bounds.size; // Adjust collider size to match the object's size
                Debug.Log("[AnchorPlacer] Added BoxCollider to object: " + obj.name);
            }
        }
        else
        {
            Debug.LogWarning("[AnchorPlacer] Prefab does not have a Renderer component. Cannot adjust collider size.");
        }

        // Add the new furniture object to the list of placed furniture
        placedFurniture.Add(obj);
        Debug.Log("[AnchorPlacer] Added furniture to list: " + obj.name);
    }

    public void SelectPrefab(GameObject selectedPrefab, Button selectedButton)
    {
        prefabToAnchor = selectedPrefab;

        // Disable the selected button
        button1.interactable = (selectedButton != button1);
        button2.interactable = (selectedButton != button2);
        button3.interactable = (selectedButton != button3);

        Debug.Log("[AnchorPlacer] Selected prefab: " + selectedPrefab.name);
    }

    private void MoveObjectAlongPlane(GameObject obj)
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinBounds))
        {
            Pose hitPose = hits[0].pose;
            obj.transform.position = hitPose.position;
            Debug.Log($"[AnchorPlacer] Moved {obj.name} to new position on plane at {hitPose.position}.");
        }
        else
        {
            Debug.Log("[AnchorPlacer] No AR plane detected for dragging operation.");
        }
    }



    private void HandleRotateGesture(object sender, RotateEventArgs e)
    {
        if (selectedObject != null && isDragging) // Check if any object is selected and is being interacted with
        {
            float rotationAmount = e.Angle; // Assuming e.Angle provides the rotation in degrees
            selectedObject.transform.Rotate(Vector3.up, rotationAmount, Space.World);
            Debug.Log($"[AnchorPlacer] Rotated {selectedObject.name} by {rotationAmount} degrees.");
        }
        else
        {
            Debug.Log("[AnchorPlacer] No object selected or not dragging, rotation ignored.");
        }
    }
}
