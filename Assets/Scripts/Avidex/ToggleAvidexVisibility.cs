using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleAvidexVisibility : MonoBehaviour
{
    public GameObject avidexPanel; // Assign the panel containing the user's list

    private bool isVisible = true;

    public void ToggleVisibility()
    {
        isVisible = !isVisible;
        avidexPanel.SetActive(isVisible);
    }
}
