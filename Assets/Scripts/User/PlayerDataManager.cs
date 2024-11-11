using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text userNameText;
    public TMP_Text birdCapturedCountText;
    public TMP_Text totalCapturesText;
    public TMP_Text pointsText;

    [Header("Gallery Configuration")]
    public GameObject birdPicButtonPrefab; // Assign the birdPicButtonPrefab here
    public Transform galleryContentParent; // Assign the Content GameObject of the ScrollView

    private int galleryCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        var player = PersistentDataManager.Instance.userProfileData;
        userNameText.text = player.username;
        birdCapturedCountText.text = $"Birds Captured Count: {player.birdsCaptured}";
        pointsText.text = $"Total Points: {player.points}";
        totalCapturesText.text = $"Total Captures: {player.totalCaptures}";
        Debug.Log("PlayerDataManager: Player data initialized.");
    }

    void Update()
    {
        if (PersistentDataManager.Instance.userGalleryPics.Count != galleryCount)
        {
            Debug.Log("PlayerDataManager: Gallery count changed. Reloading gallery pics.");
            LoadUserGalleryPics();
        }
    }

    /// <summary>
    /// Loads user gallery pictures into the ScrollView.
    /// </summary>
    public void LoadUserGalleryPics()
    {
        // Clear existing gallery items
        foreach (Transform child in galleryContentParent)
        {
            Destroy(child.gameObject);
        }

        var userGalleryPics = PersistentDataManager.Instance.userGalleryPics;
        galleryCount = userGalleryPics.Count; // Update galleryCount to prevent repeated calls

        foreach (var pic in userGalleryPics)
        {
            GameObject newItem = Instantiate(birdPicButtonPrefab, galleryContentParent);
            PicGalleryItemController controller = newItem.GetComponent<PicGalleryItemController>();
            if (controller != null)
            {
                controller.Initialize(pic, OnPicSelected);
                Debug.Log("PlayerDataManager: Gallery pic instantiated and initialized.");
            }
            else
            {
                Debug.LogWarning("PlayerDataManager: PicGalleryItemController not found on the instantiated item.");
            }
        }
    }

    /// <summary>
    /// Callback invoked when a bird picture is selected.
    /// </summary>
    /// <param name="bird">The selected bird's data.</param>
    private void OnPicSelected(UserBirdUploadData bird)
    {
        Debug.Log("PlayerDataManager: Pic selected.");
        // Ensure that GalleryUIManager.Instance is not null
        if (GalleryUIManager.Instance == null)
        {
            Debug.LogWarning("PlayerDataManager: Gallery UI Manager is null.");
            return;
        }
        Debug.Log("PlayerDataManager: Opening pic panel.");
        GalleryUIManager.Instance.PicOnClick(bird);

        // Optionally, you can pass bird data to the PicPanel for display
        // For example:
        // GalleryUIManager.Instance.DisplaySelectedBird(bird);
    }
}
