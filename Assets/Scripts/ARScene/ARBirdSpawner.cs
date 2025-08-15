using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections;

public class ARBirdSpawner : MonoBehaviour
{

    [SerializeField]
    private BirdLayerGameObjectPlacement _birdSpawner;
    public GameObject Spawn_Bird;

    [SerializeField] private BirdPrefabRegistry registry;

    private void OnEnable()
    {
        StartCoroutine(Await_Camera_Load());
    }

    IEnumerator Await_Camera_Load()
    {
        while (ARSession.state != ARSessionState.SessionTracking)
        {
            Debug.Log("Waiting for AR Session to start tracking. Current state: " + ARSession.state);
            yield return null; // Wait for the next frame
        }

        Debug.Log("AR Session is tracking! Spawning content.");
        SpawnBird();   
    }

    private void SpawnBird()
    {

        if (PersistentDataManager.Instance.selectedBirdData != null)
        {
            BirdDataObject birdData = PersistentDataManager.Instance.selectedBirdData;


            // Instantiate the bird prefab at the specified position

            var spawnPosition = new Vector3(0, 0, 70);
            Quaternion spawnRotation = Quaternion.Euler(0, 180, 0);

            // var birdPrefab = _birdSpawner.GetBirdPrefab(birdData.birdType);
            var birdPrefab = registry.Get(birdData.birdType);
            var spawnedBird = Instantiate(birdPrefab, spawnPosition, spawnRotation);
            spawnedBird.name = PersistentDataManager.Instance.selectedBirdData.birdName;
            var birdMovement = spawnedBird.GetComponent<AR_Bird_Movement>();
            birdMovement.enabled = true;
            birdMovement.Init_Fly_To_Points();

            // Assuming the birdPrefab has a script to handle its data
            ARBirdController birdController = Spawn_Bird.GetComponent<ARBirdController>();
            if (birdController != null)
            {
                birdController.Initialize(birdData);
            }
            else
            {
                Debug.LogWarning("ARBirdController script not found on birdPrefab.");
            }
        }
        else
        {
            Debug.LogError("No BirdData found in PersistentDataManager.");
        }
    }
}
