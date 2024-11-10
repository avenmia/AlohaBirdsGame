using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserProfileData : MonoBehaviour
{
    public string username;
    public int birdsCaptured;
    public int totalCaptures;
    public int points;

    // Constructor
    public UserProfileData(string username, int birdsCaptured, int points)
    {
        this.username = username;
        this.birdsCaptured = birdsCaptured;
        this.points = points;
    }
}
