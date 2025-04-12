using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewBirdData", menuName = "Avidex/Bird Data")]
public class BirdData : ScriptableObject
{
    public Guid id;
    public string birdName;
    public string hawaiianBirdName;
    public Sprite birdImage;
    public string birdDescription;
    public string conservationStatus;
    public bool nativeHawaiianSpecies;
    public int points;
    public string ebirdURL;
    // Add more fields as needed (e.g., habitat, sound clip)
}
