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
    public TMP_Text birdHawaiianBirdName;
    public TMP_Text birdConservationStatus;
    public TMP_Text birdNativeHawaiianSpecies;
    public TMP_Text birdPoints;
    public TMP_Text birdEBirdURL;
    public UserAvidexBird currentBird;
    public Transform galleryContentParent;
    public GameObject galleryItemPrefab;
    public GameObject maximizedViewPanel;

    public void ShowDetails(UserAvidexBird bird)
    {
        currentBird = bird;
        Debug.Log($"[DEBUG]: Showing capture details of current bird: {currentBird.captureData}");
        var userBird = bird.birdData;
        birdImage.sprite = userBird.birdImage;
        birdNameText.text = userBird.birdName;
        birdDescriptionText.text = userBird.birdDescription;
        birdHawaiianBirdName.text = userBird.hawaiianBirdName;
        birdConservationStatus.text = userBird.conservationStatus;
        birdNativeHawaiianSpecies.text = userBird.nativeHawaiianSpecies == true ? "yes" : "no";
        birdPoints.text = userBird.points.ToString();
        birdEBirdURL.text = userBird.ebirdURL;
        Debug.Log($"[DEBUG]: Loading user's bird image gallery");
        LoadUserBirdImageGallery();
    }

    public void HideDetails()
    {
        AvidexPanelManager.Instance.ShowAvidexPanel();
    }

    void LoadUserBirdImageGallery()
    {
        // Clear any existing items
        Debug.Log("[DEBUG] Removing any existing item from bird image gallery");
        foreach (Transform child in galleryContentParent)
        {
            Destroy(child.gameObject);
        }

        if (currentBird == null || currentBird.captureData == null)
        {
            Debug.Log("[DEBUG]: Current bird should not be null in bird image gallery");
            return;
        }
        
        if (currentBird.captureData.Count == 0)
        {
            Debug.Log("[DEBUG]: No capture data to add");
            return;
        }

        Debug.Log($"[DEBUG] Instantiating LoaduserBirdImage per capture. Count: {currentBird.captureData.Count}");
        foreach (var capture in currentBird.captureData)
            {
                Debug.Log($"[DEBUG]: Adding bird with Capture time: {capture.captureTime}");
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