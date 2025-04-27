using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class GalleryItemController : MonoBehaviour
{
    public Image screenshotImage;
    public TMP_Text birdNameText;

    private BirdCaptureData captureData;

    public void Initialize(BirdCaptureData data)
    {
        captureData = data;
        Debug.Log($"[DEBUG]: Capture Data: {captureData}");
        // Create a Sprite from the Texture2D
        Sprite newSprite = Sprite.Create(
            data.screenCaptureShot,
            new Rect(0, 0, data.screenCaptureShot.width, data.screenCaptureShot.height),
            new Vector2(0.5f, 0.5f)
        );
        screenshotImage.sprite = newSprite;
        birdNameText.text = $"Capture Time: {data.captureTime} \n Capture Location: {data.location.latitude} {data.location.longitude}";
        Debug.Log("[DEBUG]: " + birdNameText.text);


        // Add click listener
        Debug.Log("[DEBUG] Adding on click listener to the click button");
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Debug.Log($"[DEBUG] Click activated for Show maximized image of {captureData}");
        BirdDetailUIManager.Instance.ShowMaximizedImage(captureData);
    }
}