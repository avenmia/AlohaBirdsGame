using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BirdListItemController : MonoBehaviour
{
    public Image birdThumbnail;
    public TMP_Text birdNameText;
    private BirdData birdData;
    private System.Action<BirdData> onClickCallback;

    public void Setup(BirdData data, System.Action<BirdData> onClick)
    {
        birdData = data;
        birdThumbnail.sprite = data.birdImage;
        birdNameText.text = data.birdName;
        onClickCallback = onClick;

        // Add click listener
        GetComponent<Button>().onClick.AddListener(OnItemClick);


    }

    private void OnItemClick()
    {
        onClickCallback?.Invoke(birdData);
    }
}
