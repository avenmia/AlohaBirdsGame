using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class AREnd_Sequence : MonoBehaviour
{
    public List<GameObject> Sequence;
    private int Sequence_Index = 0;
    public List<RawImage> Polaroids;
    [SerializeField] private ARBirdSpawner AR_Bird_Spawner;
    [SerializeField] private int Polaroid_Index = 0;
    [SerializeField] private GameObject Left_Arrow;
    [SerializeField] private GameObject Right_Arrow;
    [SerializeField] private ARCameraManager AR_Camera_Manager;

    [SerializeField] private TMP_Text Score;
    [SerializeField] private TMP_Text Bird_Name;
    private void OnEnable()
    {
        Bird_Name.text = Polaroids[0].GetComponent<Polaroid_Data>().Name;
        Sequence[0].SetActive(true); // Capture Popup
    }

    public void Next_Sequence()
    {
        Sequence_Index++;
        Sequence[Sequence_Index].SetActive(true);
    }

    public void Left_Next()
    {
        Polaroids[Polaroid_Index].gameObject.transform.gameObject.SetActive(false);

        Polaroid_Index = (Polaroid_Index - 1 + Polaroids.Count) % Polaroids.Count;

        Polaroids[Polaroid_Index].gameObject.transform.gameObject.SetActive(true);

        Debug.Log($"Current Polaroid Index: {Polaroid_Index}");
    }

    public void Right_Next()
    {
        Polaroids[Polaroid_Index].gameObject.transform.gameObject.SetActive(false);

        Polaroid_Index = (Polaroid_Index + 1 + Polaroids.Count) % Polaroids.Count;

        Polaroids[Polaroid_Index].gameObject.transform.gameObject.SetActive(true);

        Debug.Log($"Current Polaroid Index: {Polaroid_Index}");
    }
    
    public void Choose_Polaroid()
    {
        //Score.text = Polaroids[Polaroid_Index].GetComponent<Polaroid_Data>().Score.ToString();]
        CaptureBird();

        Debug.Log($"Polaroid bird name: {Polaroids[Polaroid_Index].gameObject.GetComponent<Polaroid_Data>().Name}");
        Debug.Log($"PersistentDataManager Name: {PersistentDataManager.Instance.selectedBirdData.birdName}");
        StartCoroutine(SaveScreenshotToGallery(PersistentDataManager.Instance.selectedBirdData.birdName, Polaroid_Index));
    }

    IEnumerator SaveScreenshotToGallery(string birdName, int index)
    {
        yield return new WaitForEndOfFrame();
        var existingBird = PersistentDataManager.Instance.GetExisitingUserBirdByName(birdName);
        if (existingBird != null)
        {
            // This should be the one added above
            var lastCapture = existingBird.captureData.Last();
            lastCapture.screenCaptureShot = Polaroids[index].texture as Texture2D;
        }
        else
        {
            Debug.LogError("Bird should exist before adding image to gallery");
        }

        Debug.Log($"[DEBUG] Resetting AR_Scene");
        Reset_Object();
        Debug.Log($"[DEBUG] Successfully reset AR_Scene");
        NavigationManager.Instance.ReturnToMapScene(this.gameObject);
    }

    void Reset_Object()
    {
        Destroy(AR_Bird_Spawner.Spawn_Bird);
        Debug.Log($"[DEBUG] Destroyed Spawned Bird");
        foreach (var seq in Sequence)
        {
            seq.SetActive(false);
        }
        Sequence_Index = 0;
        Debug.Log($"[DEBUG] Set Sequence Objects inactive");
        AR_Camera_Manager.enabled = true;
        Debug.Log($"[DEBUG] Disabled AR_Camera_Manager");
    }

    void CaptureBird()
    {
        var birdSpawnData = PersistentDataManager.Instance.selectedBirdData;
        var captureData = new BirdCaptureData()
        {
            captureTime = DateTime.Now,
            // TODO: Verify the bird spawn location is the actual location not scene location
            // May need to convert scene to lat long
            location = new GeoLocation(birdSpawnData.location.x, birdSpawnData.location.y)
        };

        var existingUserBird = PersistentDataManager.Instance.GetExisitingUserBird(birdSpawnData);
        if (existingUserBird == null)
        {
            Debug.Log($"[DEBUG]: existing user bird is null for birdSpawnData: {birdSpawnData.id}");
            var bird = PersistentDataManager.Instance.GetBirdData(birdSpawnData.birdName);
            bird.birdData.id = birdSpawnData.id;
            var newUserAvidexBird = new UserAvidexBird(bird)
            {
                captureData = new List<BirdCaptureData>() { captureData }
            };
            PersistentDataManager.Instance.AddUserAvidexBird(newUserAvidexBird);
        }
        else
        {
            Debug.Log($"[DEBUG]: existing user bird is not null null for birdSpawnData: {birdSpawnData.id}");
            PersistentDataManager.Instance.UpdateUserAvidexBird(existingUserBird.birdData.birdName, birdSpawnData.id, captureData);
        }
        PersistentDataManager.Instance.UpdateUserCaptures();
    }
}
