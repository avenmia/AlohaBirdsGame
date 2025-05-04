using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ARBirdCaptureManager : MonoBehaviour
{
    public Transform galleryContentParent;
    public Canvas mainCanvas;
    private bool isBirdCaptured = false;
    private bool isScreenTransitioning = false;
    [SerializeField] private List<RawImage> Polaroid;
    [SerializeField] private List<GameObject> Visual_Attempts;
    [SerializeField] private int Capture_Attempts;
    [SerializeField] private List<GameObject> Hide_List;
    [SerializeField] private float Timer = 30f;
    [SerializeField] private TMP_Text Time_Text;

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

        Timer -= Time.deltaTime;
        Time_Text.text = Timer.ToString("0.00");
        if(Timer <= 0 && !End_Capture_Sequence.activeInHierarchy)
        {
            NavigationManager.Instance.ReturnToPrevScene();
        }
    }

    public void onClickCapture()
    {
        Capture_Attempts--;
        var birdObject = GameObject.FindGameObjectWithTag("Bird");
        float distance = Vector3.Distance(birdObject.transform.position, Camera.main.transform.position);
        var polData = Polaroid[Capture_Attempts].GetComponent<Polaroid_Data>();
        polData.Score = distance;
        polData.Name = birdObject.name;

        StartCoroutine(SaveScreenshotToDecision(Capture_Attempts));
        Destroy(Visual_Attempts[Capture_Attempts]);

        if(Capture_Attempts <= 0)
        {
            StartCoroutine(Wait_For_Last_Screenshot());
        }
    }

    IEnumerator Wait_For_Last_Screenshot()
    {
        yield return new WaitForEndOfFrame();
        foreach (var item in Hide_List)
        {
            item.gameObject.SetActive(false);
        }
        End_Capture_Sequence.SetActive(true);
        this.gameObject.SetActive(false);
    }

    IEnumerator SaveScreenshotToDecision(int polaroidNum)
    {
        mainCanvas.enabled = false;

        yield return new WaitForEndOfFrame();
        Debug.Log("Saving screenshot");
        Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        Polaroid[polaroidNum].texture = screenImage;

        mainCanvas.enabled = true;

        // Save to device gallery
        // NativeGallery.SaveImageToGallery(screenImage, "MyGameGallery", "CapturedBird_{0}.png");
    }
}
