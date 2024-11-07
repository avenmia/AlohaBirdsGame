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

    public void ShowDetails(UserAvidexBird bird)
    {
        currentBird = bird;
        var userBird = bird.birdData;
        birdImage.sprite = userBird.birdImage;
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
        foreach(var capture in currentBird.captureData)
        {
            Debug.Log($"Avendano capture time {capture.captureTime}");
            Debug.Log($"Avendano capture location {capture.location.latitude} {capture.location.longitude}");
        }
        //GameObject newImage = Instantiate(imagePrefab, galleryContentParent);
        //newImage.GetComponent<Image>().sprite = Sprite.Create(imageTexture, new Rect(0, 0, imageTexture.width, imageTexture.height), Vector2.zero);

        //TMP_Text label = newImage.GetComponentInChildren<TMP_Text>();
        //if (label != null)
        //{
        //    label.text = birdName;
        //}

        // TODO: Add image to data structure
    }
}