using UnityEngine;

public class AvidexPanelManager : MonoBehaviour
{
    public GameObject avidexPanel; // Assign in Inspector
    public GameObject detailPanel; // Assign in Inspector

    public static AvidexPanelManager Instance;
    private void Awake() => Instance = this;

    private void Start()
    {
        ShowAvidexPanel();
    }

    public void ShowAvidexPanel()
    {
        avidexPanel.SetActive(true);
        detailPanel.SetActive(false);
    }

    public void ShowDetailPanel(UserAvidexBird birdData)
    {
        // Update detail panel content
        BirdDetailUIManager.Instance.ShowDetails(birdData);
        avidexPanel.SetActive(false);
        detailPanel.SetActive(true);
    }
}
