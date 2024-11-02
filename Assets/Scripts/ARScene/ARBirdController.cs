using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARBirdController : MonoBehaviour
{
    public BirdData birdData;
    
    public void Initialize(BirdData data)
    {
        birdData = data;
    }
}
