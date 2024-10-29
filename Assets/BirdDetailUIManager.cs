using UnityEngine;
using UnityEngine.UI;

public class BirdDetailUIManager : MonoBehaviour
{
    public static BirdDetailUIManager Instance;
    private void Awake() => Instance = this;

    public GameObject detailPanel; // Assign the detail panel GameObject
    public Image birdImage;
    public Text birdNameText;
    public Text birdDescriptionText;

    public void ShowDetails(BirdData bird)
    {
        birdImage.sprite = bird.birdImage;
        birdNameText.text = bird.birdName;
        birdDescriptionText.text = bird.birdDescription;
        detailPanel.SetActive(true);
    }

    public void HideDetails()
    {
        detailPanel.SetActive(false);
    }
}