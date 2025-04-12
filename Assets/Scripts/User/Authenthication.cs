using System;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Authenthication : MonoBehaviour
{
    [SerializeField] private TMP_InputField Username;
    [SerializeField] private TMP_InputField Password;
    [SerializeField] private TMP_InputField ReType_Password;
    [SerializeField] private TMP_InputField Screen_Name;
    [SerializeField] private TMP_Text Error;

    [SerializeField] private GameObject Sign_Up;
    [SerializeField] private GameObject Sign_In;
    [SerializeField] private GameObject After_SignIn;
    [SerializeField] private GameObject After_SignUp;
    [SerializeField] private GameObject Delete_Toggler;
    [SerializeField] private TMP_Text Error_Message;

    [SerializeField] private Player_Information Player_Information;
    //async void Awake()
    //{
    //    try
    //    {
    //        await UnityServices.InitializeAsync();
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogException(e);
    //    }
    //    Error_Message = GameObject.Find("Error Message").GetComponent<TMP_Text>();
    //}

    // Setup authentication event handlers if desired
    void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () => {
            // Shows how to get a playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            // Shows how to get an access token
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");

        };

        AuthenticationService.Instance.SignInFailed += (err) => {
            Debug.LogError(err);
        };

        AuthenticationService.Instance.SignedOut += () => {
            Debug.Log("Player signed out.");
        };

        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player session could not be refreshed and expired.");
        };
    }

    async public void SignUp()
    {
        if(Password.text.Equals(ReType_Password.text))
        {
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(Username.text, Password.text);
                Sign_Up.SetActive(false);
                After_SignUp.SetActive(true);
                Delete_Toggler.SetActive(false);
                Debug.Log("SignUp is successful.");
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                
                Debug.LogException(ex);
                Error_Message.text = ex.Message;
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
                Error_Message.text = ex.Message;
            }
        }
        else
        {
            Error.text = "Passwords don't match";
            Debug.Log("Passwords don't match");
            Error_Message.text = "Passwords don't match";
        }
        
    }

    async public void Create_Screen_Name()
    {
        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(Screen_Name.text);
            Debug.Log("Name set");
            Player_Information.Load_Data();
            SceneManager.LoadScene("MapScene", LoadSceneMode.Single);
        }
        catch (AuthenticationException ex)
        {
            //Couldn't authenticate, relog
            Debug.LogException(ex);
            Error_Message.text = ex.Message;
            return;
        }
        catch (RequestFailedException ex)
        {
            //Not signed in/request failed
            Debug.LogException(ex);
            Error_Message.text = ex.Message;
            return;
        }
    }

    async  public void SignIn()
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(Username.text, Password.text);
            Debug.Log("SignIn is successful.");
            Debug.Log(AuthenticationService.Instance.PlayerName);
            Player_Information.Load_Data();
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            Error_Message.text = ex.Message;
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            Error_Message.text = ex.Message;
        }
    }
}
