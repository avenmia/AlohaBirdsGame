using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserProfileData
{
    public string username;
    public int birdsCaptured;
    public int totalCaptures;
    public int points;
    public List<string> uniqueBirds;

    // Constructor
    public UserProfileData(string username, int birdsCaptured, int points, List<string> uniqueBirds)
    {
        this.username = username;
        this.birdsCaptured = birdsCaptured;
        this.points = points;
        this.uniqueBirds = uniqueBirds;
    }
}
