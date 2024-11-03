using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameBird
{
    public BirdData birdData;
    public bool userDiscovered;

    public GameBird(BirdData data, bool discovered = false)
    {
        birdData = data;
        userDiscovered = discovered;
    }
}
