using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UsernameHandler : MonoBehaviour
{
    public TMP_InputField usernameInputField;

    void Start()
    {

        // Check if username already exists
        if (PlayerPrefs.HasKey("Username"))
        {
            // Skip splash screen if username exists
            LoadMainScene();
        }
    }

    public void OnSubmitButtonClicked()
    {
        string username = usernameInputField.text.Trim();

        if (!string.IsNullOrEmpty(username))
        {

            // Save the username
            PlayerPrefs.SetString("Username", username);
            PlayerPrefs.Save();

            // Proceed to the main game scene
            LoadMainScene();
        }
        else
        {
            Debug.LogWarning("Username cannot be empty.");
            // Optionally, display a warning message to the user
        }
    }

    void LoadMainScene()
    {
        // Replace "MainGameScene" with the actual name of your main scene
        SceneManager.LoadScene("MapScene");
    }
}
