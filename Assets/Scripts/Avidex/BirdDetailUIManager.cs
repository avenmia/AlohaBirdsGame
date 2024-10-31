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

    public void ShowDetails(BirdData bird)
    {
        birdImage.sprite = bird.birdImage;
        birdNameText.text = bird.birdName;
        birdDescriptionText.text = bird.birdDescription;
    }

    public void HideDetails()
    {
        AvidexPanelManager.Instance.ShowAvidexPanel();
    }
}