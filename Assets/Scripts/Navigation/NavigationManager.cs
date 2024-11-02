using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager Instance;

    // save the data in a list
    public List<string> sceneQueue = new List<string>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadNewScene(string sceneName)
    {      
        // grabs the current scene and appends it to the list queue
        string currScene = SceneManager.GetActiveScene().name;
        sceneQueue.Add(currScene);
        
        // goes to next scene
        SceneManager.LoadScene(sceneName);
    }

    public void returnToPrevScene() 
    {
        // retrieves and pops the last scene from the list
        string prevScene = sceneQueue[sceneQueue.Count - 1];
        sceneQueue.RemoveAt(sceneQueue.Count - 1);

        // loads the previous Scene 
        SceneManager.LoadScene(prevScene);
    }

}
