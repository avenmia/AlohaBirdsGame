using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudSave.Models.Data.Player;
using Unity.Services.CloudSave;
using SaveOptions = Unity.Services.CloudSave.Models.Data.Player.SaveOptions;
using System.Linq;

public class PersistentDataManager : MonoBehaviour
{
    public static PersistentDataManager Instance;
    public UserProfileData userProfileData;
    public TMP_InputField usernameInputField;
    public BirdDataObject selectedBirdData; // The bird data to pass to the next scene
    public Dictionary<string, GameBird> gameBirds = new Dictionary<string, GameBird>();
    public List<UserAvidexBird> userCapturedBirds = new List<UserAvidexBird>();
    public List<UserBirdUploadData> userGalleryPics = new List<UserBirdUploadData>();
    public static IList<string> GameBirdNames = new List<string>();

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

    private void OnDestroy()
    {
        Debug.Log($"PersistentDataManager {this.GetInstanceID()} is being destroyed.");
    }

    public async void Save_Data()
    {
        if (userProfileData == null) { return; }
        string uniqueBirds = string.Join(",", userProfileData.uniqueBirds);

        var timeNow = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss,fff");

        var playerData = new Dictionary<string, object>()
        {
            {"time", timeNow },
            {"playerName", AuthenticationService.Instance.PlayerName},
            {"uniqueBirds", uniqueBirds},
            {"birdsCaptured", userProfileData.birdsCaptured},
            {"points", userProfileData.points }
        };

        //await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);

        try
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(playerData, new SaveOptions(new PublicWriteAccessClassOptions()));
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            Debug.Log("Couldn't connect to cloud");
            //doesn't work but do offline save anyway
        }

        //Offline save
        PlayerPrefs.SetString("time", timeNow);
        PlayerPrefs.SetString("playerName", userProfileData.username);
        PlayerPrefs.SetString("uniqueBirds", uniqueBirds);
        PlayerPrefs.SetInt("birdsCaptured", userProfileData.birdsCaptured);
        PlayerPrefs.SetInt("points", userProfileData.points);
    }

    private void OnApplicationQuit()
    {
        if (userProfileData == null) { return; }
        Save_Data();
    }

    //private void OnApplicationPause()
    //{
    //    Save_Data();
    //}

    public void AddUserGalleryBird(UserBirdUploadData birdData)
    {
        userGalleryPics.Add(birdData);
    }

    public void SetBirdData(BirdDataObject newBirdData)
    {
        Debug.Log($"[DEBUG]: Setting PDM selected bird: {newBirdData.id}");
        selectedBirdData = newBirdData;
    }

    public void RemoveSelectedBird()
    {
        if (selectedBirdData != null)
        {
            Debug.Log($"[DEBUG]: removing bird from selected bird: {selectedBirdData.id}");
            selectedBirdData = null;
            return;
        }
        Debug.Log("[DEBUG]: Selected bird was already set to null");
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

    public void UpdateUserCaptures()
    {
        userProfileData.birdsCaptured = userCapturedBirds.Count;
        int totalCount = 0;
        foreach(var bird in userCapturedBirds)
        {
            foreach(var capture in bird.captureData)
            {
                totalCount++;
            }
        }
        userProfileData.totalCaptures = totalCount;
    }

    public void AddUserAvidexBird(UserAvidexBird bird)
    {
        Debug.Log($"[DEBUG] adding user avidex bird and bird userCapturedbirds: {bird.birdData.id}");
        userCapturedBirds.Add(bird);
        if(bird.birdData == null || bird.birdData.birdName == null || gameBirds[bird.birdData.birdName] == null)
        {
            Debug.LogWarning("Bird data or bird name should not be null here");
        }

        userProfileData.points += bird.birdData.points;
        gameBirds[bird.birdData.birdName].userDiscovered = true;
    }

    // TODO: Confirm this is right
    public void UpdateUserAvidexBird(string name, Guid id, BirdCaptureData captureData)
    {
        Debug.Log($"[DEBUG] Updating existing user avidex bird {id}");
        var existingBird = userCapturedBirds.Find(b => b.birdData.birdName == name);
        existingBird.captureData.Add(captureData);
        existingBird.caughtBirds.Add(id);
        userProfileData.points += existingBird.birdData.points;
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
        GameBirdNames = gameBirds.Select(b => b.Key).ToArray();
        Debug.Log($"BirdManager: Loaded {gameBirds.Count} birds into the gameBirds dictionary.");
    }
}
