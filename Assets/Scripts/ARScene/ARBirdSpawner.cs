using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARBirdSpawner : MonoBehaviour
{

    [SerializeField]
    private BirdLayerGameObjectPlacement _birdSpawner;

    private void Start()
    {
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

            var birdPrefab = _birdSpawner.GetBirdPrefab(birdData.birdType);
            var spawnedBird = Instantiate(birdPrefab, spawnPosition, spawnRotation);
            spawnedBird.name = PersistentDataManager.Instance.selectedBirdData.birdName;
            var birdMovement = spawnedBird.GetComponent<AR_Bird_Movement>();
            birdMovement.enabled = true;
            birdMovement.Init_Fly_To_Points();

            // Assuming the birdPrefab has a script to handle its data
            ARBirdController birdController = spawnedBird.GetComponent<ARBirdController>();
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
