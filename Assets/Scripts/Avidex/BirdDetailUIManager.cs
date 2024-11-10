using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BirdDetailUIManager : MonoBehaviour
{
    public static BirdDetailUIManager Instance;
    private void Awake() => Instance = this;

    public GameObject detailPanel; // Assign the detail panel GameObject
    public Image birdImage;
    public TMP_Text birdNameText;
    public TMP_Text birdDescriptionText;
    public UserAvidexBird currentBird;
    public Transform galleryContentParent;
    public GameObject galleryItemPrefab;
    public GameObject maximizedViewPanel;

    public void ShowDetails(UserAvidexBird bird)
    {
        currentBird = bird;
        var userBird = bird.birdData;
        birdImage.sprite = userBird.birdImage;
        Debug.Log($"Avendano Bird name is: {userBird.birdName}");
        birdNameText.text = userBird.birdName;
        birdDescriptionText.text = userBird.birdDescription;
        LoadUserBirdImageGallery();
    }

    public void HideDetails()
    {
        AvidexPanelManager.Instance.ShowAvidexPanel();
    }

    void LoadUserBirdImageGallery()
    {
        // Clear any existing items
        foreach (Transform child in galleryContentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var capture in currentBird.captureData)
        {
            GameObject newItem = Instantiate(galleryItemPrefab, galleryContentParent);
            GalleryItemController controller = newItem.GetComponent<GalleryItemController>();
            if (controller != null)
            {
                controller.Initialize(capture);
            }
            else
            {
                Debug.LogWarning("GalleryItemController not found on the instantiated item.");
            }

        }
    }

    public void ShowMaximizedImage(BirdCaptureData data)
    {
        maximizedViewPanel.SetActive(true);
        MaximizedViewController controller = maximizedViewPanel.GetComponent<MaximizedViewController>();
        controller.Display(data);
    }
}