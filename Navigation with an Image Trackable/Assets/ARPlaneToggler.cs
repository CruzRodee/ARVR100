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

    void Start()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();

        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(TogglePlanesVisibility);
        }
    }

    public void TogglePlanesVisibility()
    {
        isVisible = !isVisible;

        if (arPlaneManager != null)
        {
            foreach (var plane in arPlaneManager.trackables)
            {
                plane.gameObject.SetActive(isVisible);
            }
        }
    }
}
