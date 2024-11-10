using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ARBirdCaptureManager : MonoBehaviour
{
    public Transform galleryContentParent;
    public GameObject imagePrefab;
    public GameObject popupPrefab;
    public Canvas mainCanvas;
    private bool isBirdCaptured = false;
    private bool isScreenTransitioning = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !isBirdCaptured && !isScreenTransitioning)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("Bird"))
                    {
                        CaptureBird(hit.collider.gameObject);
                        isBirdCaptured = true;
                        return;
                    }
                }
            }
        }
        else if(Input.touchCount == 0)
        {
            isBirdCaptured = false;
        }
    }

    void CaptureBird(GameObject birdObject)
    {

        var controller = birdObject.GetComponent<ARBirdController>();
        var birdSpawnData = controller.birdData;
        var captureData = new BirdCaptureData()
        {
            captureTime = DateTime.Now,
            // TODO: Verify the bird spawn location is the actual location not scene location
            // May need to convert scene to lat long
            location = new GeoLocation(birdSpawnData.location.x, birdSpawnData.location.y)
        };

        var existingUserBird = PersistentDataManager.Instance.GetExisitingUserBird(birdSpawnData);
        Debug.Log($"Avendano is existing bird null? {existingUserBird == null}");
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
            Debug.Log($"Avendano existing bird name? {existingUserBird.birdData.birdName}");

            PersistentDataManager.Instance.UpdateUserAvidexBird(existingUserBird.birdData.birdName, captureData);
        }
        PersistentDataManager.Instance.UpdateUserCaptures();
        ShowPopup($"You captured a {birdSpawnData.birdName}");
        isScreenTransitioning = true;
        StartCoroutine(ReturnToMap(6f));
        
        // TODO: Uncomment to save screenshot 
        StartCoroutine(SaveScreenshotToGallery(birdSpawnData.birdName));
        // Provide feedback to the user
    }

    public void onClickCapture()
    {
        // find bird gameobject 
        var birdObject = GameObject.FindGameObjectWithTag("Bird");
        CaptureBird(birdObject);
    }

    private void ShowPopup(string message)
    {
        if (popupPrefab != null)
        {
            GameObject popup = Instantiate(popupPrefab, mainCanvas.transform);
            popup.GetComponentInChildren<TMP_Text>().text = message;
            // Optionally, add animations or auto-destroy after some time
            Destroy(popup, 5f); // Destroys the popup after 2 seconds
        }
        else
        {
            Debug.LogWarning("Popup Prefab is not assigned.");
        }
    }

    private IEnumerator ReturnToMap(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        NavigationManager.Instance.ReturnToPrevScene();
        isScreenTransitioning = false;
    }

    IEnumerator SaveScreenshotToGallery(string birdName)
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("Saving screenshot");
        Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        // Save to device gallery
        // NativeGallery.SaveImageToGallery(screenImage, "MyGameGallery", "CapturedBird_{0}.png");

        var existingBird = PersistentDataManager.Instance.GetExisitingUserBirdByName(birdName);
        if (existingBird != null)
        {
            // This should be the one added above
            var lastCapture = existingBird.captureData.Last();
            lastCapture.screenCaptureShot = screenImage;
        }
        else
        {
            Debug.LogError("Bird should exist before adding image to gallery");
        }

        // Clean up
        // Don't destroy screenImage if you're using it in the gallery
    }
}
