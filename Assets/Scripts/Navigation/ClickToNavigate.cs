using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ClickToNavigate : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private BirdDataObject birdSpawnData;
    public string prefabName;

    public GameObject NavPrefab;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (PersistentDataManager.Instance != null)
        {
            
            switch (prefabName)
            {
                case "PigeonPin":
                    {
                        // TODO: This won't work once we incorporate location
                        birdSpawnData = MapGameState.Instance.spawnedBirds.Find(b => b.birdName == "Pigeon");
                        break;
                    }
                case "BarnOwlPin":
                    {
                        birdSpawnData = MapGameState.Instance.spawnedBirds.Find(b => b.birdName == "Barn Owl");
                        break;
                    }
                default: Debug.LogWarning("This prefab does not exist"); break;
            }
            if (birdSpawnData == null)
            {
                Debug.LogWarning("Bird spawn data should not be null");
            }

            // Store the bird data in the PersistentDataManager
            PersistentDataManager.Instance.SetBirdData(birdSpawnData);
            
            // find navManager if exists from other interactions
            NavigationManager.Instance.LoadNewScene("ARScene");

        }
        else
        {
            Debug.LogError("PersistentDataManager instance is not available.");
        }
    }
}
