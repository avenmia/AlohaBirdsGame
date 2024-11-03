using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ARBirdCaptureManager : MonoBehaviour
{
    public Transform galleryContentParent;
    public GameObject imagePrefab;
    public GameObject popupPrefab;
    public Canvas mainCanvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("Bird"))
                    {
                        CaptureBird(hit.collider.gameObject);

                    }
                }
            }
        }
    }

    void CaptureBird(GameObject birdObject)
    {
        var controller = birdObject.GetComponent<ARBirdController>();
        var birdSpawnData = controller.birdData;
        var captureData = new BirdCaptureData()
        {
            captureTime = DateTime.Now,
            location = new GeoLocation(birdSpawnData.location.x, birdSpawnData.location.y)
        };

        var existingUserBird = PersistentDataManager.Instance.GetExisitingUserBird(birdSpawnData);
        if (existingUserBird == null)
        {
            var bird = PersistentDataManager.Instance.GetBirdData(birdSpawnData.birdName);
            var newUserAvidexBird = new UserAvidexBird(bird)
            {
                captureData = new List<BirdCaptureData>() { captureData }
            };
            PersistentDataManager.Instance.AddUserAvidexBird(newUserAvidexBird);
        }
        else
        {
            PersistentDataManager.Instance.UpdateUserAvidexBird(existingUserBird.birdData.name, captureData);
        }

        ShowPopup($"You captured a {birdSpawnData.birdName}");
        
        // TODO: Uncomment to save screenshot 
        // StartCoroutine(SaveScreenshotToGallery());
        // Provide feedback to the user
    }

    private void ShowPopup(string message)
    {
        if (popupPrefab != null)
        {
            GameObject popup = Instantiate(popupPrefab, mainCanvas.transform);
            popup.GetComponentInChildren<TMP_Text>().text = message;
            // Optionally, add animations or auto-destroy after some time
            Destroy(popup, 10f); // Destroys the popup after 2 seconds

        }
        else
        {
            Debug.LogWarning("Popup Prefab is not assigned.");
        }
    }

    IEnumerator SaveScreenshotToGallery()
    {
        yield return new WaitForEndOfFrame();
        Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        // Save to device gallery
        // NativeGallery.SaveImageToGallery(screenImage, "MyGameGallery", "CapturedBird_{0}.png");

        // Update in-game gallery
        // AddImageToGallery(screenImage);

        // Clean up
        // Don't destroy screenImage if you're using it in the gallery
    }

    void AddImageToGallery(Texture2D imageTexture)
    {
        GameObject newImage = Instantiate(imagePrefab, galleryContentParent);
        newImage.GetComponent<Image>().sprite = Sprite.Create(imageTexture, new Rect(0, 0, imageTexture.width, imageTexture.height), Vector2.zero);
    }
}
