using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TouchInputManager : MonoBehaviour
{
    public UserListManager userListManager; // Reference to the user's list manager

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // We only care about touch began for a tap
            if (touch.phase == TouchPhase.Began)
            {
                // Create a ray from the touch position
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                // Perform the raycast
                if (Physics.Raycast(ray, out hit))
                {
                    // Check if the hit object has the ObjectProperties component
                    BirdProperties birdProps = hit.collider.gameObject.GetComponent<BirdProperties>();

                    if (birdProps != null)
                    {
                        // Add the object's properties to the user's list
                        userListManager.AddObjectToList(birdProps);
                    }
                }
            }
        }
    }
}
