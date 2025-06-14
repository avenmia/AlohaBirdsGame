using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AvidexUIManager : MonoBehaviour
{
    public Transform contentPanel; // Assign the Content object of the ScrollView
    public GameObject birdListItemPrefab; // Assign the prefab you created
    public TMP_InputField searchField;

    readonly List<GameObject> _rows = new();

    private void OnEnable()
    {
        PopulateAvidex();
        searchField.onValueChanged.AddListener(FilterRows);
        FilterRows(string.Empty);  
    }


    void OnDisable() {
        searchField.onValueChanged.RemoveListener(FilterRows);
    }                              

    private void PopulateAvidex()
    {
        // Clear existing items
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
        _rows.Clear();

        if (PersistentDataManager.Instance == null || PersistentDataManager.Instance.userCapturedBirds == null)
        {
            return;
        }
        //List<UserAvidexBird> capturedBirds = UserBirdManager.Instance.capturedBirds;

        List<UserAvidexBird> capturedBirds = PersistentDataManager.Instance.userCapturedBirds;

        Debug.Log($"[DEBUG]: Adding {capturedBirds.Count} to Avidex");
        foreach (UserAvidexBird bird in capturedBirds)
        {
            GameObject newItem = Instantiate(birdListItemPrefab, contentPanel);
            newItem.transform.SetParent(contentPanel, false);
            BirdListItemController controller = newItem.GetComponent<BirdListItemController>();
            controller.Setup(bird, OnBirdSelected);
            _rows.Add(newItem);
        }
    }

    void FilterRows(string term)
    {
        term = term.Trim().ToLowerInvariant();

        foreach (GameObject row in _rows)
        {
            var data = row.GetComponent<BirdListItemController>().birdData;
            bool match = string.IsNullOrEmpty(term) ||
                         data.birdData.birdName.ToLowerInvariant().Contains(term) ||
                         data.birdData.hawaiianBirdName.ToLowerInvariant().Contains(term);

            row.SetActive(match);                 // hide rows that don’t match
        }
    }

    private void OnBirdSelected(UserAvidexBird bird)
    {
        if (BirdDetailUIManager.Instance == null)
        {
            return;
        }
        AvidexPanelManager.Instance.ShowDetailPanel(bird);
    }
}
