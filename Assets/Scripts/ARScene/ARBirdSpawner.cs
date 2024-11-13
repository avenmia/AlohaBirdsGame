using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARBirdSpawner : MonoBehaviour
{
    public GameObject barnOwlPrefab; // Prefab representing the bird in AR
    public GameObject pigeonPrefab; // Prefab representing the bird in AR

    private void Start()
    {
        SpawnBird();
    }

    private void SpawnBird()
    {

        // Uncomment for testing
        //if(birdPrefab != null)
        //{
        //    // BirdDataObject birdData = birdPrefab.GetComponent<BirdDataObject>();
        //    var spawnPosition = new Vector3(0, 0, 0);
        //    Quaternion spawnRotation = Quaternion.Euler(0, 180, 0);
        //    GameObject spawnedBird = Instantiate(birdPrefab, spawnPosition, spawnRotation);
        //    spawnedBird.tag = "Bird";

        //    //ARBirdController birdController = spawnedBird.GetComponent<ARBirdController>();
        //    //if (birdController != null)
        //    //{
        //    //    birdController.Initialize(birdData);
        //    //}
        //}
        if (PersistentDataManager.Instance.selectedBirdData != null)
        {
            BirdDataObject birdData = PersistentDataManager.Instance.selectedBirdData;


            // Instantiate the bird prefab at the specified position

            var spawnPosition = new Vector3(0, 0, 0);
            Quaternion spawnRotation = Quaternion.Euler(0, 180, 0);
            GameObject spawnedBird;
            if(birdData.birdName == "Barn Owl")
            {
                spawnedBird = Instantiate(barnOwlPrefab, spawnPosition, spawnRotation);

            }
            else
            {
                spawnedBird = Instantiate(pigeonPrefab, spawnPosition, spawnRotation);
            }
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
