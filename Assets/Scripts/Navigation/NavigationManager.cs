using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using DG.Tweening;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager Instance;

    // save the data in a list
    public List<string> sceneStack = new List<string>();
    public int score = 0;
    public Image image;
    public GameObject AR_Scene;
    public GameObject Map_Scene;

    [SerializeField] private Camera AR_Camera;
    [SerializeField] private Camera Map_Camera;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DOTween.Init();
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
        StartCoroutine(Drop_Transistion());
        AR_Scene.gameObject.SetActive(true);
        Toggle_Camera(AR_Camera, Map_Camera);
        StartCoroutine(AwaitCamera());
        StartCoroutine(Lift_Transistion());
        Map_Scene.gameObject.SetActive(false);
    }

    private void Toggle_Camera(Camera activeCam, Camera inactiveCam)
    {
        activeCam.depth = 1;
        inactiveCam.depth = -1;
    }

    IEnumerator AwaitCamera()
    {
        while (ARSession.state != ARSessionState.SessionTracking)
        {
            Debug.Log("Waiting for AR Session to start tracking. Current state: " + ARSession.state);
            yield return null; // Wait for the next frame
        }
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
        Toggle_Camera(Map_Camera, AR_Camera);
    }

    IEnumerator Drop_Transistion()
    {
        image.rectTransform.DOMoveY(0, 0.25f);
        yield return new WaitForSeconds(0.25f);
        image.rectTransform.DOMoveY(-Screen.height * 2, 0.25f);
    }

    IEnumerator Lift_Transistion()
    {
        image.rectTransform.DOMoveY(Screen.height * 2, 0.25f);
        yield return new WaitForSeconds(0.25f);
    }

}
