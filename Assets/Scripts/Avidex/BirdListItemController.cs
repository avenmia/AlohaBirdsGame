using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BirdListItemController : MonoBehaviour
{
    public Image birdThumbnail;
    public TMP_Text birdNameText;
    public UserAvidexBird birdData;
    private System.Action<UserAvidexBird> onClickCallback;

    public void Setup(UserAvidexBird data, System.Action<UserAvidexBird> onClick)
    {
        var userBirdData = data.birdData;
        birdData = data;
        birdThumbnail.sprite = userBirdData.birdImage;
        birdNameText.text = userBirdData.birdName;
        onClickCallback = onClick;

        // Add click listener
        GetComponent<Button>().onClick.AddListener(OnItemClick);


    }

    private void OnItemClick()
    {
        onClickCallback?.Invoke(birdData);
    }
}
