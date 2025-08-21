using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;

public class ARBirdCaptureManager : MonoBehaviour
{
    public Transform galleryContentParent;
    public Canvas mainCanvas;
    private bool isBirdCaptured = false;
    private bool isScreenTransitioning = false;
    [SerializeField] private List<RawImage> Polaroid;
    [SerializeField] private List<GameObject> Visual_Attempts;
    [SerializeField] private int Capture_Attempts;
    [SerializeField] private float Timer = 30f;
    [SerializeField] private TMP_Text Time_Text;
    [SerializeField] private ARCameraManager AR_Camera_Manager;
    [SerializeField] private Button Capture_Button;

    [SerializeField] private GameObject End_Capture_Sequence; //AREnd_Sequence

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !isBirdCaptured && !isScreenTransitioning)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("Bird"))
                    {
                        //CaptureBird(hit.collider.gameObject);
                        isBirdCaptured = true;
                        //return;
                    }
                }
            }
        }
        else if(Input.touchCount == 0)
        {
            isBirdCaptured = false;
        }

        if(ARSession.state == ARSessionState.SessionTracking && !End_Capture_Sequence.activeInHierarchy)
        {
            Timer -= Time.deltaTime;
            Time_Text.text = Timer.ToString("0.00");
        }
        
        if(Timer <= 0 && !End_Capture_Sequence.activeInHierarchy)
        {
            NavigationManager.Instance.ReturnToMapScene();
        }
    }

    public void onClickCapture()
    {
        Capture_Button.enabled = false;
        Capture_Attempts--;
        var birdObject = GameObject.FindGameObjectWithTag("Bird");
        float distance = Vector3.Distance(birdObject.transform.position, Camera.main.transform.position);
        var polData = Polaroid[Capture_Attempts].GetComponent<Polaroid_Data>();
        Debug.Log($"[DEBUG]: Found polaroid data {Polaroid[Capture_Attempts].GetComponent<Polaroid_Data>()}");
        polData.Score = distance;
        Debug.Log($"[DEBUG]: Setting distance {polData.Score}");
        polData.Name = birdObject.gameObject.name;
        Debug.Log($"[DEBUG]: Setting bird name {polData.Name}");

        StartCoroutine(SaveScreenshotToDecision(Capture_Attempts));
        Debug.Log($"[DEBUG]: Successfully set screenshot");
        Visual_Attempts[Capture_Attempts].SetActive(false);

        if(Capture_Attempts <= 0)
        {
            StartCoroutine(Wait_For_Last_Screenshot());
        }
    }

    IEnumerator Wait_For_Last_Screenshot()
    {
        yield return new WaitForEndOfFrame();
        //mainCanvas.enabled = false;
        End_Capture_Sequence.SetActive(true);

        yield return new WaitForEndOfFrame();
        AR_Camera_Manager.enabled = false;
        Capture_Attempts = 3;
        Timer = 30.0f;
        Time_Text.text = Timer.ToString("0.00");
        foreach (var vis in Visual_Attempts)
        {
            vis.SetActive(true);
        }
    }

    IEnumerator SaveScreenshotToDecision(int polaroidNum)
    {
        mainCanvas.enabled = false;

        yield return new WaitForEndOfFrame();
        Debug.Log($"Saving screenshot for position {polaroidNum}");
        Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        Polaroid[polaroidNum].texture = screenImage;

        mainCanvas.enabled = true;
        yield return new WaitForEndOfFrame();
        Capture_Button.enabled = true;
        yield return new WaitForEndOfFrame();

        // Save to device gallery
        // NativeGallery.SaveImageToGallery(screenImage, "MyGameGallery", "CapturedBird_{0}.png");
    }
}
