using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class BirdDataObject
{
    public string birdName;
    public BirdType birdType;
    public float spawnProbability;
    public float spawnRadius;
    public Sprite birdImage;
    public Vector3 location;

}
