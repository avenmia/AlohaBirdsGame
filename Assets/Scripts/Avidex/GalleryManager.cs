using UnityEngine;
using System.Collections.Generic;

public class GalleryManager : MonoBehaviour
{
    public static GalleryManager Instance;

    public Transform galleryContentParent;
    public GameObject galleryItemPrefab;
    public GameObject maximizedViewPanel;

    private void Awake()
    {
        Instance = this;
    }

    //public void PopulateGallery(List<BirdCaptureData> captureDataList)
    //{
    //    // Clear existing items
    //    foreach (Transform child in galleryContentParent)
    //    {
    //        Destroy(child.gameObject);
    //    }

    //    // Instantiate gallery items
    //    foreach (var data in captureDataList)
    //    {
    //        GameObject newItem = Instantiate(galleryItemPrefab, galleryContentParent);
    //        GalleryItemController controller = newItem.GetComponent<GalleryItemController>();
    //        controller.Initialize(data);
    //    }
    //}

    public void ShowMaximizedImage(BirdCaptureData data)
    {
        maximizedViewPanel.SetActive(true);
        MaximizedViewController controller = maximizedViewPanel.GetComponent<MaximizedViewController>();
        controller.Display(data);
    }
}
