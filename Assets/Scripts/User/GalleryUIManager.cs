using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GalleryUIManager : MonoBehaviour
{
    public static GalleryUIManager Instance;

    [Header("UI Panels")]
    public GameObject picPanel;
    public GameObject profilePanel;

    [Header("Pic Panel Components")]
    public Image selectedBirdImage; // Assign in Inspector
    public TMP_Text selectedBirdDescriptionText; // Assign in Inspector

    private void Awake()
    {
        // Implement Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object alive across scenes
            Debug.Log("GalleryUIManager: Instance created.");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("GalleryUIManager: Duplicate instance destroyed.");
        }
    }

    /// <summary>
    /// Activates the picPanel, deactivates the profilePanel, and displays selected bird data.
    /// </summary>
    /// <param name="bird">The selected bird's data.</param>
    public void PicOnClick(UserBirdUploadData bird)
    {
        if (picPanel != null && profilePanel != null)
        {
            picPanel.SetActive(true);
            profilePanel.SetActive(false);
            Debug.Log("GalleryUIManager: picPanel activated, profilePanel deactivated.");

            // Display selected bird image
            if (selectedBirdImage != null)
            {
                selectedBirdImage.sprite = bird.userImage;
                Debug.Log("GalleryUIManager: Selected bird image set.");
            }
            else
            {
                Debug.LogError("GalleryUIManager: selectedBirdImage is not assigned in the Inspector.");
            }

            // Display AI description
            if (selectedBirdDescriptionText != null)
            {
                selectedBirdDescriptionText.text = bird.aiDescription;
                Debug.Log("GalleryUIManager: Selected bird description set.");
            }
            else
            {
                Debug.LogError("GalleryUIManager: selectedBirdDescriptionText is not assigned in the Inspector.");
            }
        }
        else
        {
            Debug.LogError("GalleryUIManager: picPanel or profilePanel is not assigned in the Inspector.");
        }
    }

    /// <summary>
    /// Deactivates the picPanel and activates the profilePanel.
    /// </summary>
    public void PicCloseClick()
    {
        if (picPanel != null && profilePanel != null)
        {
            picPanel.SetActive(false);
            profilePanel.SetActive(true);
            Debug.Log("GalleryUIManager: picPanel deactivated, profilePanel activated.");
        }
        else
        {
            Debug.LogError("GalleryUIManager: picPanel or profilePanel is not assigned in the Inspector.");
        }
    }
}
