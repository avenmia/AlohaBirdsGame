using System;
using UnityEngine;
using UnityEngine.UI;

public class PicGalleryItemController : MonoBehaviour
{
    [Header("UI Components")]
    public Image screenshotImage; // Assign in the Inspector

    private UserBirdUploadData birdData;
    private Action<UserBirdUploadData> onClickCallback;

    /// <summary>
    /// Initializes the gallery item with bird data and assigns the click callback.
    /// </summary>
    /// <param name="data">Data of the bird to display.</param>
    /// <param name="onClick">Callback to invoke on click.</param>
    public void Initialize(UserBirdUploadData data, Action<UserBirdUploadData> onClick)
    {
        // Assign bird data
        birdData = data;

        // Assign the bird image
        if (screenshotImage != null)
        {
            screenshotImage.sprite = data.userImage;
            Debug.Log("PicGalleryItemController: Bird image assigned.");
        }
        else
        {
            Debug.LogWarning("PicGalleryItemController: screenshotImage is not assigned.");
        }

        // Assign the click callback
        onClickCallback = onClick;
        Debug.Log("PicGalleryItemController: Click callback assigned.");

        // Assign the button click listener
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners(); // Prevent duplicate listeners
            btn.onClick.AddListener(OnItemClick);
            Debug.Log("PicGalleryItemController: Button click listener assigned.");
        }
        else
        {
            Debug.LogWarning("PicGalleryItemController: No Button component found on this GameObject.");
        }
    }

    /// <summary>
    /// Called when the button is clicked.
    /// </summary>
    private void OnItemClick()
    {
        Debug.Log("PicGalleryItemController: Image button clicked.");
        onClickCallback?.Invoke(birdData);
    }
}
