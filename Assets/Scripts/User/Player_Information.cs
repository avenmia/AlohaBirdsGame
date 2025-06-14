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

    public int TotalCaptures = 0;

    [SerializeField] private GameObject No_Token_Path;
    [SerializeField] private GameObject Attempting_Log_In;
    [SerializeField] private bool Clear_Token;

    [SerializeField] private PersistentDataManager PersistentDataManager;

    private async void Awake()
    {
        // DontDestroyOnLoad(this.gameObject);
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void Start()
    {
        if (Clear_Token)
        {
            Debug.Log("[DEBUG]: Signing user out and removing session token");
            AuthenticationService.Instance.SignOut();
            AuthenticationService.Instance.ClearSessionToken();
            
        }

        if (AuthenticationService.Instance.SessionTokenExists)
        {
	    Debug.Log("[DEBUG]: Signing in cached player from session token");
            Sign_In_Cached_Player();
            //Load GPS scene
        }
        else
        {
	        Debug.Log("[DEBUG]: Non-logged on route");
            No_Token_Path.SetActive(true);
            Attempting_Log_In.SetActive(false);
            // Offline_Load();
            //Load SignIn/LogIn
        }
    }

    async private void Sign_In_Cached_Player()
    {
        if (!AuthenticationService.Instance.SessionTokenExists) { return; }

        // Sign in Anonymously
        // This call will sign in the cached player.
        try
        {
	        Debug.Log("[DEBUG]: Signing in Cached user");
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log(AuthenticationService.Instance.PlayerName);
            Debug.Log("Cached user sign in succeeded!");

            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            Load_Data();
            // SceneManager.LoadScene("MapScene", LoadSceneMode.Single);
            return;
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

    public async void Load_Data()
    {
        Debug.Log("Load Player: " + Player_Name);
        var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> {
            "time", 
            "uniqueBirds",
            "totalCaptures", 
            "birdsCaptured",
            "points",
        }, new LoadOptions(new PublicReadAccessClassOptions()));
        Player_Name = await AuthenticationService.Instance.GetPlayerNameAsync();

        // TODO: Offline load
        if (playerData.TryGetValue("time", out var tim) && PlayerPrefs.HasKey("time"))
        {
            DateTime prefTime = DateTime.ParseExact(PlayerPrefs.GetString("time"), "yyyy-MM-dd HH:mm:ss,fff",
                System.Globalization.CultureInfo.InvariantCulture);
            DateTime cloudTime = DateTime.ParseExact(tim.Value.GetAs<string>(), "yyyy-MM-dd HH:mm:ss,fff",
                System.Globalization.CultureInfo.InvariantCulture);
            int result = DateTime.Compare(cloudTime, prefTime);
            // if(result < 0) // cloudtime is earlier than preftime
            // {
            //    Offline_Load();
            //    return;
            // }
        }

        if (playerData.TryGetValue("birdsCaptured", out var bC))
        {
            BirdsCaptured = bC.Value.GetAs<int>();
        }

        if (playerData.TryGetValue("totalCaptures", out var tC))
        {
            TotalCaptures = tC.Value.GetAs<int>();
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

        PersistentDataManager.userProfileData = new UserProfileData(Player_Name, BirdsCaptured, Points, Unique_Birds_Caught, TotalCaptures);
        SceneManager.LoadScene("MapScene", LoadSceneMode.Single);
    }

    // TODO: Add offline 
    public void Offline_Load()
    {
        Player_Name = PlayerPrefs.GetString("playerName");
        BirdsCaptured = PlayerPrefs.GetInt("birdsCaptured");
        TotalCaptures = PlayerPrefs.GetInt("totalCaptures");
        Points = PlayerPrefs.GetInt("points");

        HashSet<string> tempBirds = new HashSet<string>(Unique_Birds_Caught);
        var separate = PlayerPrefs.GetString("uniqueBirds").Split(',');
        foreach (var bird in separate)
        {
            tempBirds.Add(bird);
        }
        Unique_Birds_Caught = new List<string>(tempBirds);

        PersistentDataManager.Instance.userProfileData = new UserProfileData(Player_Name, BirdsCaptured, Points, Unique_Birds_Caught, 0);
        // SceneManager.LoadScene("MapScene", LoadSceneMode.Single);
    }
}
