using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARBirdSpawner : MonoBehaviour
{
    public GameObject birdPrefab; // Prefab representing the bird in AR

    private void Start()
    {
        SpawnBird();
    }

    private void SpawnBird()
    {
        if (PersistentDataManager.Instance.selectedBirdData != null)
        {
            BirdData birdData = PersistentDataManager.Instance.selectedBirdData;


            // Instantiate the bird prefab at the specified position
            var spawnPosition = new Vector3(0, 0, 0);
            GameObject spawnedBird = Instantiate(birdPrefab, spawnPosition, Quaternion.identity);
            spawnedBird.tag = "Bird";

            // Customize the spawned bird based on BirdData
            // For example, set the bird's name, sprite, etc.
            // spawnedBird.name = birdData.birdName;

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
