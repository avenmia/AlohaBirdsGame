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
            //birdSpawnData = MapGameState.Instance.spawnedBirds.Find(b => BirdTypeUtil.GetBirdPinName(b.birdName) == prefabName);
            //if (birdSpawnData == null)
            //{
            //    Debug.LogError("This prefab does not exist");
            //    return;
            //}

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
