using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ClickToNavigate : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private BirdData birdData;
    public void OnPointerClick(PointerEventData eventData)
    {

        if (PersistentDataManager.Instance != null)
        {

            // Store the bird data in the PersistentDataManager
            PersistentDataManager.Instance.selectedBirdData = birdData;

            // Load the new scene
            SceneManager.LoadScene("ARScene");
        }
        else
        {
            Debug.LogError("PersistentDataManager instance is not available.");
        }
    }
}
