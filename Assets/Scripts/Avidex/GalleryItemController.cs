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

        // Create a Sprite from the Texture2D
        Sprite newSprite = Sprite.Create(
            data.screenCaptureShot,
            new Rect(0, 0, data.screenCaptureShot.width, data.screenCaptureShot.height),
            new Vector2(0.5f, 0.5f)
        );
        screenshotImage.sprite = newSprite;
        birdNameText.text = $"Capture Time: {data.captureTime} \n Capture Location: {data.location.latitude} {data.location.longitude}";
        

        // Add click listener
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        BirdDetailUIManager.Instance.ShowMaximizedImage(captureData);
    }
}