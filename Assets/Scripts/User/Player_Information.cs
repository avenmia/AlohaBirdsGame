using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.CloudSave.Models.Data.Player;
using SaveOptions = Unity.Services.CloudSave.Models.Data.Player.SaveOptions;
using UnityEngine;
using UnityEngine.SceneManagement;

//Primary function is to load user data from cloud or playerpref, PDM updates the loaded files
public class Player_Information : MonoBehaviour
{
    [SerializeField] public string Player_Name;
    //[SerializeField] public int Level = 1;
    //[SerializeField] public float Current_EXP = 0;
    //[SerializeField] public float Max_EXP = 5;
    [SerializeField] public List<string> Unique_Birds_Caught;
    //[SerializeField] public List<string> Achievements;
    public int Points = 0;
    public int BirdsCaptured = 0;

    [SerializeField] private GameObject No_Token_Path;
    [SerializeField] private GameObject Attempting_Log_In;
    [SerializeField] private bool Clear_Token;

    [SerializeField] private PersistentDataManager PersistentDataManager;

    async private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    async private void Start()
    {
        if (Clear_Token) { AuthenticationService.Instance.ClearSessionToken(); }
        if (AuthenticationService.Instance.SessionTokenExists)
        {
            Sign_In_Cached_Player();
            //Load GPS scene
        }
        else
        {
            No_Token_Path.SetActive(true);
            Attempting_Log_In.SetActive(false);
            //Load SignIn/LogIn
        }
    }

    async private void Sign_In_Cached_Player()
    {
        // Check if a cached player already exists by checking if the session token exists
        if (!AuthenticationService.Instance.SessionTokenExists)
        {
            // if not, then do nothing
            return;
        }

        // Sign in Anonymously
        // This call will sign in the cached player.
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Cached user sign in succeeded!");

            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            Online_Initalize();
            SceneManager.LoadScene("MapScene", LoadSceneMode.Single);
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    public void Online_Initalize()
    {
        Player_Name = AuthenticationService.Instance.PlayerName;
        Load_Data();
    }

    private void Offline_Initalize()
    {
        Offline_Load();
    }

    //public void Add_EXP(float score)
    //{
    //    var tmp = score / 1000;
    //    Current_EXP += tmp;
    //    if(Current_EXP > Max_EXP)
    //    {
    //        Current_EXP -= Max_EXP;
    //        Level += 1;
    //        Max_EXP = Level * 1000;
    //    }
    //}
    //public void Add_Bird(string bird)
    //{
    //    if(!Unique_Birds_Caught.Contains(bird)) { Unique_Birds_Caught.Add(bird); }
    //}

    //public void Add_Achievement(string achievementID)
    //{
    //    if(!Achievements.Contains(achievementID))
    //    {
    //        Achievements.Add(achievementID);
    //    }
    //}

    //public async void Save_Data()
    //{
    //    string uniqueBirds = string.Join(",", Unique_Birds_Caught);
    //    //string totalAchievements = string.Join(",", Achievements);

    //    var timeNow = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss,fff");

    //    var playerData = new Dictionary<string, object>()
    //    {
    //        {"time", timeNow },
    //        {"playerName", AuthenticationService.Instance.PlayerName},
    //        //{"level",  Level},
    //        //{"currentEXP", Current_EXP},
    //        //{"maxEXP", Max_EXP},
    //        {"uniqueBirds", uniqueBirds},
    //        //{"totalAchievements", totalAchievements},
    //    };

    //    //await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);

    //    try
    //    {
    //        await CloudSaveService.Instance.Data.Player.SaveAsync(playerData, new SaveOptions(new PublicWriteAccessClassOptions()));
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.Log(ex);
    //        Debug.Log("Couldn't connect to cloud");
    //        //doesn't work but do offline save anyway
    //    }

    //    //Offline save
    //    PlayerPrefs.SetString("time", timeNow);
    //    PlayerPrefs.SetString("playerName", Player_Name);
    //    //PlayerPrefs.SetInt("level", Level);
    //    //PlayerPrefs.SetFloat("currentEXP", Current_EXP);
    //    //PlayerPrefs.SetFloat("maxEXP", Max_EXP);
    //    PlayerPrefs.SetString("uniqueBirds", uniqueBirds);
    //    //PlayerPrefs.SetString("totalAchievements", totalAchievements);
    //}

    //public void Offline_Save()
    //{
    //    string uniqueBirds = string.Join(",", Unique_Birds_Caught);
    //    //string totalAchievements = string.Join(",", Achievements);

    //    PlayerPrefs.SetString("time", System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss,fff"));
    //    PlayerPrefs.SetString("playerName", Player_Name);
    //    //PlayerPrefs.SetInt("level", Level);
    //    //PlayerPrefs.SetFloat("currentEXP", Current_EXP);
    //    //PlayerPrefs.SetFloat("maxEXP", Max_EXP);
    //    PlayerPrefs.SetString("uniqueBirds", uniqueBirds);
    //    //PlayerPrefs.SetString("totalAchievements", totalAchievements);
    //}

    public async void Load_Data()
    {
        var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> {
            "time", 
            "playerName", 
            //"level", 
            //"currentEXP", 
            //"maxEXP", 
            "uniqueBirds", 
            "birdsCaptured",
            "points",
            //"totalAchievements"
        }, new LoadOptions(new PublicReadAccessClassOptions()));

        if(playerData.TryGetValue("time", out var tim) && PlayerPrefs.HasKey("time"))
        {
            DateTime prefTime = DateTime.ParseExact(PlayerPrefs.GetString("time"), "yyyy-MM-dd HH:mm:ss,fff",
                System.Globalization.CultureInfo.InvariantCulture);
            DateTime cloudTime = DateTime.ParseExact(tim.Value.GetAs<string>(), "yyyy-MM-dd HH:mm:ss,fff",
                System.Globalization.CultureInfo.InvariantCulture);
            int result = DateTime.Compare(cloudTime, prefTime);
            if(result < 0) // cloudtime is earlier than preftime
            {
                Offline_Load();
                return;
            }
        }

        if (playerData.TryGetValue("birdsCaptured", out var bC))
        {
            BirdsCaptured = bC.Value.GetAs<int>();
        }

        if (playerData.TryGetValue("points", out var p))
        {
            Points = p.Value.GetAs<int>();
        }


        if (playerData.TryGetValue("uniqueBirds", out var uni))
        {
            var separate = uni.Value.GetAs<string>().Split(',');
            foreach(var bird in separate)
            {
                if(!string.IsNullOrEmpty(bird)) { Unique_Birds_Caught.Add(bird); }
            }
        }

        //if (playerData.TryGetValue("totalAchievements", out var ttl))
        //{
        //    HashSet<string> tempAch = new HashSet<string>(Achievements);
        //    var separate = ttl.Value.GetAs<string>().Split(',');
        //    foreach (var ach in separate)
        //    {
        //        if (!string.IsNullOrEmpty(ach)) { Achievements.Add(ach); }
        //    }
        //    Achievements = new List<string>(tempAch);
        //}

        PersistentDataManager.userProfileData = new UserProfileData(Player_Name, BirdsCaptured, Points, Unique_Birds_Caught);
    }

    public void Offline_Load()
    {
        Player_Name = PlayerPrefs.GetString("playerName");
        BirdsCaptured = PlayerPrefs.GetInt("birdsCaptured");
        Points = PlayerPrefs.GetInt("points");
        //Max_EXP = PlayerPrefs.GetFloat("maxEXP");

        HashSet<string> tempBirds = new HashSet<string>(Unique_Birds_Caught);
        var separate = PlayerPrefs.GetString("uniqueBirds").Split(',');
        foreach(var bird in separate)
        {
            tempBirds.Add(bird);
        }
        Unique_Birds_Caught = new List<string>(tempBirds);

        //HashSet<string> tempAch = new HashSet<string>(Achievements);
        //separate = PlayerPrefs.GetString("totalAchievements").Split(',');
        //foreach(var ach in separate)
        //{
        //    tempAch.Add(ach);
        //}
        //Achievements = new List<string>(tempAch);

        PersistentDataManager.userProfileData = new UserProfileData(Player_Name, BirdsCaptured, Points, Unique_Birds_Caught);
    }

    //private void OnApplicationPause(bool pause)
    //{
    //    Save_Data();
    //}

    //private void OnApplicationQuit()
    //{
    //    Save_Data();
    //}
}
