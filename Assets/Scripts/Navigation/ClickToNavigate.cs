using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ClickToNavigate : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private BirdDataObject birdSpawnData;
    public string prefabName;
    public Guid birdId;

    public GameObject NavPrefab;
    private bool hasBeenClicked = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (hasBeenClicked) return;
        hasBeenClicked = true;
        if (PersistentDataManager.Instance != null)
        {
            birdSpawnData = MapGameState.Instance.spawnedBirds.Find(b => b.id == birdId);
            if (birdSpawnData == null)
            {
                Debug.LogError("This prefab does not exist");
                return;
            }

            // Store the bird data in the PersistentDataManager
            PersistentDataManager.Instance.SetBirdData(birdSpawnData);

            // find navManager if exists from other interactions
            Debug.Log($"[DEBUG]: Loading AR_Scene for {prefabName}");
            NavigationManager.Instance.LoadAR_Scene();
        }
        else
        {
            Debug.LogError("PersistentDataManager instance is not available.");
        }

        Destroy(this.gameObject);
    }
}
