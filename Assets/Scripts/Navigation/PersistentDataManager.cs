using System;
using System.Collections.Generic;
using UnityEngine;

public class PersistentDataManager : MonoBehaviour
{
    public static PersistentDataManager Instance;

    public BirdDataObject selectedBirdData; // The bird data to pass to the next scene

    public Dictionary<string, GameBird> gameBirds = new Dictionary<string, GameBird>();

    public List<UserAvidexBird> userCapturedBirds = new List<UserAvidexBird>();

    // public event Action<BirdData> OnBirdDataSet;

    private void Awake()
    {
        // Implement Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object alive across scenes
            LoadGameBirds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetBirdData(BirdDataObject newBirdData)
    {
        selectedBirdData = newBirdData;
    }

    public UserAvidexBird GetExisitingUserBird(BirdDataObject bird)
    {
        return userCapturedBirds.Find(b => b.birdData.birdName == bird.birdName);
    }
    public UserAvidexBird GetExisitingUserBirdByName(string name)
    {
        return userCapturedBirds.Find(b => b.birdData.birdName == name);
    }

    public GameBird GetBirdData(string name)
    {
        GameBird result;
        if(gameBirds.TryGetValue(name, out result))
        {
            return result;
        }

        Debug.LogWarning("No bird exists in the game");
        return null;
    }

    public void AddUserAvidexBird(UserAvidexBird bird)
    {
        userCapturedBirds.Add(bird);
        if(bird.birdData == null || bird.birdData.birdName == null || gameBirds[bird.birdData.birdName] == null)
        {
            Debug.LogWarning("Bird data or bird name should not be null here");
        }
        gameBirds[bird.birdData.birdName].userDiscovered = true;
    }

    // TODO: Confirm this is right
    public void UpdateUserAvidexBird(string name, BirdCaptureData captureData)
    {
        var existingBird = userCapturedBirds.Find(b => b.birdData.birdName == name);
        existingBird.captureData.Add(captureData);
    }

    public void LoadGameBirds()
    {
        BirdData[] allBirdData = Resources.LoadAll<BirdData>("GameBirds");

        foreach (BirdData birdData in allBirdData)
        {
            if (birdData != null && !string.IsNullOrEmpty(birdData.birdName))
            {
                // Create a new GameBird instance
                GameBird gameBird = new GameBird(birdData, discovered: false);

                // Add to the dictionary
                if (!gameBirds.ContainsKey(birdData.birdName))
                {
                    gameBirds.Add(birdData.birdName, gameBird);
                }
                else
                {
                    Debug.LogWarning($"BirdManager: Duplicate bird name detected: {birdData.birdName}. Skipping.");
                }
            }
            else
            {
                Debug.LogWarning("BirdManager: Encountered a BirdData asset with null or empty birdName.");
            }
        }

        Debug.Log($"BirdManager: Loaded {gameBirds.Count} birds into the gameBirds dictionary.");
    }
}
