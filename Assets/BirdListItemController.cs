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
        Debug.Log("Avendano 1");
        birdData = data;
        Debug.Log("Avendano 2");

        birdThumbnail.sprite = data.birdImage;
        Debug.Log("Avendano 3");

        birdNameText.text = data.birdName;
        Debug.Log("Avendano 4");

        onClickCallback = onClick;

        Debug.Log("Avendano 5");


        // Add click listener
        //GetComponent<Button>().onClick.AddListener(OnItemClick);

        Debug.Log("Avendano 6");

    }

    private void OnItemClick()
    {
        Debug.Log("Avendano On item click");
        onClickCallback?.Invoke(birdData);
    }
}
