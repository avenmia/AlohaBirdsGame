using UnityEngine;

public class AvidexPanelManager : MonoBehaviour
{
    public GameObject avidexPanel; // Assign in Inspector
    public GameObject detailPanel; // Assign in Inspector

    private void Start()
    {
        ShowAvidexPanel();
    }

    public void ShowAvidexPanel()
    {
        avidexPanel.SetActive(true);
        detailPanel.SetActive(false);
    }

    public void ShowDetailPanel(BirdData birdData)
    {
        // Update detail panel content
        detailPanel.GetComponent<BirdDetailUIManager>().ShowDetails(birdData);

        avidexPanel.SetActive(false);
        detailPanel.SetActive(true);
    }
}
