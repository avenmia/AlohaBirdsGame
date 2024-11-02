using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    // Reference to the camera; if not set, it will default to the main camera
    [SerializeField]
    private Camera targetCamera;

    void Start()
    {
        // If no camera is assigned, use the main camera
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera == null)
        {
            Debug.LogError("Billboard: No camera assigned and no Main Camera found.");
        }
    }

    void Update()
    {
        if (targetCamera != null)
        {
            // Make the object face the camera
            transform.LookAt(transform.position + targetCamera.transform.rotation * Vector3.forward,
                             targetCamera.transform.rotation * Vector3.up);
            transform.Rotate(0, 180, 0);
        }
    }
}
