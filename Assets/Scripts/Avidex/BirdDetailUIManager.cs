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

    public void ShowDetails(UserAvidexBird bird)
    {
        var userBird = bird.birdData;
        birdImage.sprite = userBird.birdImage;
        birdNameText.text = userBird.birdName;
        birdDescriptionText.text = userBird.birdDescription;
    }

    public void HideDetails()
    {
        AvidexPanelManager.Instance.ShowAvidexPanel();
    }
}