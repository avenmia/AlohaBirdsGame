using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager Instance;

    // save the data in a list
    public List<string> sceneStack = new List<string>();
    public int score = 0;
    public Image image;
    public GameObject AR_Scene;
    public GameObject Map_Scene;

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
        sceneStack.Add(currScene);
        score += 1;
        // goes to next scene

        SceneManager.LoadScene(sceneName);
    }

    public void LoadAR_Scene()
    {
        StartCoroutine(FadeInScene());
    }

    public void ReturnToPrevScene() 
    {
        if (sceneStack.Count > 0)
        {
            // retrieves and pops the last scene from the list
            string prevScene = sceneStack.Last();
            sceneStack.RemoveAt(sceneStack.Count - 1);

            // loads the previous Scene
            Debug.LogWarning($"Previous scene removed from stack {prevScene}");
            score += 1; 
            SceneManager.LoadScene(prevScene);
        } 
        else 
        {
            Debug.LogWarning("Scene stack is empty.");
        }
    }

    public void ReturnToMapScene(GameObject toInactivate = null)
    {
        if(toInactivate != null) { toInactivate.SetActive(false); }
        AR_Scene.gameObject.SetActive(false);
        Map_Scene.gameObject.SetActive(true);
    }

    IEnumerator FadeInScene()
    {
        Color startColor = image.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 255f); // Fully opaque
        image.CrossFadeColor(endColor, 3, true, true);

        //yield return new WaitForSeconds(3.0f);

        AR_Scene.gameObject.SetActive(true);
        Map_Scene.gameObject.SetActive(false);

        image.color = endColor; // Ensure it's fully opaque at the end
        yield return new WaitForEndOfFrame();

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        Color startColor = image.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // Fully transparent
        image.CrossFadeColor(endColor, 3.0f, true, true);
        //yield return new WaitForSeconds(3.0f);

        yield return new WaitForEndOfFrame();
        image.color = endColor; // Ensure it's fully transparent at the end
        yield return new WaitForEndOfFrame();
    }

}
