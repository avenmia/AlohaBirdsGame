using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARBirdController : MonoBehaviour
{
    public BirdDataObject birdData;
    
    public void Initialize(BirdDataObject data)
    {
        birdData = data;
    }
}
