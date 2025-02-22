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

public class PersistentDataManager : MonoBehaviour
{
    public static PersistentDataManager Instance;

    public UserProfileData userProfileData;

    public TMP_InputField usernameInputField;

    public BirdDataObject selectedBirdData; // The bird data to pass to the next scene

    public Dictionary<string, GameBird> gameBirds = new Dictionary<string, GameBird>();

    public List<UserAvidexBird> userCapturedBirds = new List<UserAvidexBird>();

    public List<UserBirdUploadData> userGalleryPics = new List<UserBirdUploadData>();

    // public event Action<BirdData> OnBirdDataSet;

    private void Awake()
    {
        // Implement Singleton pattern
        if (Instance == null)
        {
            //PlayerPrefs.DeleteAll();
            //PlayerPrefs.Save();
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object alive across scenes
            LoadGameBirds();
            // userProfileData = new UserProfileData("Guest", 0, 0);
            //LoadPlayerData();
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

    //public void SavePlayerData()
    //{
    //    BinaryFormatter formatter = new BinaryFormatter();
    //    Debug.Log($"Application path{Application.persistentDataPath}");
    //    string path = Application.persistentDataPath + "/playerdata.dat";

    //    FileStream stream = new FileStream(path, FileMode.Create);

    //    formatter.Serialize(stream, userProfileData);
    //    stream.Close();
    //}

    public async void Save_Data()
    {
        string uniqueBirds = string.Join(",", userProfileData.uniqueBirds);
        //string totalAchievements = string.Join(",", Achievements);

        var timeNow = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss,fff");

        var playerData = new Dictionary<string, object>()
        {
            {"time", timeNow },
            {"playerName", AuthenticationService.Instance.PlayerName},
            //{"level",  Level},
            //{"currentEXP", Current_EXP},
            //{"maxEXP", Max_EXP},
            {"uniqueBirds", uniqueBirds},
            {"birdsCaptured", userProfileData.birdsCaptured},
            {"points", userProfileData.points }
            //{"totalAchievements", totalAchievements},
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
        //PlayerPrefs.SetInt("level", Level);
        //PlayerPrefs.SetFloat("currentEXP", Current_EXP);
        //PlayerPrefs.SetFloat("maxEXP", Max_EXP);
        PlayerPrefs.SetString("uniqueBirds", uniqueBirds);
        PlayerPrefs.SetInt("birdsCaptured", userProfileData.birdsCaptured);
        PlayerPrefs.SetInt("points", userProfileData.points);
        //PlayerPrefs.SetString("totalAchievements", totalAchievements);
    }

    //private void OnApplicationPause(bool pause)
    //{
    //    Save_Data();
    //}

    private void OnApplicationQuit()
    {
        Save_Data();
    }

    //public void LoadPlayerData()
    //{
    //    string path = Application.persistentDataPath + "/playerdata.dat";

    //    if (File.Exists(path))
    //    {
    //        BinaryFormatter formatter = new BinaryFormatter();
    //        FileStream stream = new FileStream(path, FileMode.Open);

    //        userProfileData = formatter.Deserialize(stream) as UserProfileData;
    //        stream.Close();
    //    }
    //    else
    //    {
    //        // If no data exists, initialize with default values
    //        var username = PlayerPrefs.GetString("Username");
    //        if (username == null)
    //        {
    //            username = "Guest";
    //        }
    //        userProfileData = new UserProfileData(username, 0, 0);
    //    }
    //}

    //public void SplashButtonPressed()
    //{
    //    AddUsername();
    //    SceneManager.LoadScene("MapScene");
    //}

    //public void AddUsername()
    //{
    //    string username = usernameInputField.text.Trim();
    //    userProfileData.username = username;
    //    Debug.Log($"Username added: {username}");
    //}

    public void AddUserGalleryBird(UserBirdUploadData birdData)
    {
        userGalleryPics.Add(birdData);
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
        userCapturedBirds.Add(bird);
        if(bird.birdData == null || bird.birdData.birdName == null || gameBirds[bird.birdData.birdName] == null)
        {
            Debug.LogWarning("Bird data or bird name should not be null here");
        }

        userProfileData.points += bird.birdData.points;
        gameBirds[bird.birdData.birdName].userDiscovered = true;
    }

    // TODO: Confirm this is right
    public void UpdateUserAvidexBird(string name, BirdCaptureData captureData)
    {
        var existingBird = userCapturedBirds.Find(b => b.birdData.birdName == name);
        existingBird.captureData.Add(captureData);
        userProfileData.points += existingBird.birdData.points;
    }

    public void LoadGameBirds()
    {
        BirdData[] allBirdData = Resources.LoadAll<BirdData>("GameBirds");
        var geoJsonLoader = new GeoJsonLoader();
        geoJsonLoader.LoadGeoJson();

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
