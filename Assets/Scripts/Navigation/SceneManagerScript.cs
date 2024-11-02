using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        if (NavigationManager.Instance != null)
        {
            NavigationManager.Instance.LoadNewScene(sceneName);
        }
        else
        {
            Debug.LogWarning("NavigationManager instance is not available.");
        }
    }

    public void returnScene()
    {
        if (NavigationManager.Instance != null)
        {
            NavigationManager.Instance.returnToPrevScene();
        }
        else
        {
            Debug.LogWarning("NavigationManager instance is not available.");
        }
    }
}
