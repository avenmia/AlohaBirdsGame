using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AvidexUIManager : MonoBehaviour
{
    public Transform contentPanel; // Assign the Content object of the ScrollView
    public GameObject birdListItemPrefab; // Assign the prefab you created

    private void OnEnable()
    {
        PopulateAvidex();
    }

    private void PopulateAvidex()
    {
        // Clear existing items
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        if (UserBirdManager.Instance == null || UserBirdManager.Instance.capturedBirds == null)
        {
            return;
        }
        List<BirdData> capturedBirds = UserBirdManager.Instance.capturedBirds;

        foreach (BirdData bird in capturedBirds)
        {
            GameObject newItem = Instantiate(birdListItemPrefab, contentPanel);
            newItem.transform.SetParent(contentPanel, false);
            BirdListItemController controller = newItem.GetComponent<BirdListItemController>();
            controller.Setup(bird, OnBirdSelected);
        }
    }

    private void OnBirdSelected(BirdData bird)
    {
        if (BirdDetailUIManager.Instance == null)
        {
            return;
        }
        AvidexPanelManager.Instance.ShowDetailPanel(bird);
    }
}
