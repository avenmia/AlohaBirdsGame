using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserListManager : MonoBehaviour
{
    [System.Serializable]
    public class UserObjectEntry
    {
        public string BirdName;
        public string BirdDescription;
        public string BirdColor;
    }

    public List<UserObjectEntry> userObjectEntries = new List<UserObjectEntry>();

    public GameObject listItemPrefab; // Reference to the list item prefab
    public Transform listContentParent; // Reference to the content parent of the Scroll View

    public void AddObjectToList(BirdProperties objProps)
    {

        UserObjectEntry entry = new UserObjectEntry
        {
            BirdName = objProps.Name,
            BirdDescription = objProps.Description,
            BirdColor = objProps.Color
        };

        userObjectEntries.Add(entry);

        // Update the UI to reflect the new entry
        UpdateUI(entry);
    }

    private void UpdateUI(UserObjectEntry newEntry)
    {
        // Instantiate a new list item
        GameObject newItem = Instantiate(listItemPrefab, listContentParent);

        // Set the text fields
        TMP_Text[] texts = newItem.GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text t in texts)
        {
            if (t.name == "BirdNameText")
            {
                t.text = newEntry.BirdName;
            }
            else if (t.name == "BirdDescriptionText")
            {
                t.text = newEntry.BirdDescription;
            }
            else if (t.name == "BirdColorText")
            {
                t.text = newEntry.BirdColor.ToString();
            }
        }
    }
}
