using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FloatingText : MonoBehaviour
{
    public float floatingSpeed = 2f;
    public float floatHeight = 2f;
    private Vector3 startPosition;
    private bool isFloating = false;
    private static FloatingText instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            startPosition = transform.position;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void StartFloatingText(float speed, float height)
    {
        if(instance!= null)
        {
            instance.floatingSpeed = speed;
            instance.floatHeight = height;
            instance.isFloating = true;
            Debug.Log($"Floating text start with speed: {speed}, height: {height}.");
        }
        else
        {
            Debug.LogError("No instance of FloatingText found.");
        }
    }

    void Update()
    {
        if (isFloating)
        {
            float newY = Mathf.Sin(Time.time * floatingSpeed) * floatHeight;
            transform.position = startPosition + new Vector3(0, newY, 0);
        }
    }
}
