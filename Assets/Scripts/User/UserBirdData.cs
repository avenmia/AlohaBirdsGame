using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UserBirdData
{
 
    public Dictionary<string,string> CaughtBirds;

    public UserBirdData(Guid birdId, string birdType)
    {
        CaughtBirds.Add(birdId.ToString(), birdType);
    }
}
