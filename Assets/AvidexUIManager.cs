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

        List<BirdData> capturedBirds = UserBirdManager.Instance.capturedBirds;

        Debug.Log($"Avendano captured bird count {capturedBirds.Count}");
        Debug.Log("Avendano captured birds:");

        foreach (BirdData bird in capturedBirds)
        {
            Debug.Log($"Avendano bird {bird.birdName}");
            GameObject newItem = Instantiate(birdListItemPrefab, contentPanel);
            // TODO: Testing this
            newItem.transform.SetParent(contentPanel, false);
            BirdListItemController controller = newItem.GetComponent<BirdListItemController>();
            controller.Setup(bird, OnBirdSelected);
        }
    }

    private void OnBirdSelected(BirdData bird)
    {
        Debug.Log("Avendano, Bird selected!");
        // Show detailed info
        // BirdDetailUIManager.Instance.ShowDetails(bird);
    }
}
