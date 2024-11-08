using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaximizedViewController : MonoBehaviour
{
    public Image screenshotImage;
    public TMP_Text captureTimeText;

    public void Display(BirdCaptureData data)
    {
        Sprite newSprite = Sprite.Create(
            data.screenCaptureShot,
            new Rect(0, 0, data.screenCaptureShot.width, data.screenCaptureShot.height),
            new Vector2(0.5f, 0.5f)
        );

        screenshotImage.sprite = newSprite;
        captureTimeText.text = $"Captured on {data.captureTime.ToString("f")}";
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
