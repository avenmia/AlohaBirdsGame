using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserProfileData
{
    /// <summary>
    /// The user's name
    /// </summary>
    public string username;

    /// <summary>
    /// The unique birds captured
    /// </summary>
    public int birdsCaptured;

    /// <summary>
    /// The total number of birds the user captured 
    /// including birds of the same type
    /// </summary>
    public int totalCaptures;

    /// <summary>
    /// The user's points
    /// </summary>
    public int points;

    /// <summary>
    /// The list of unique bird names
    /// </summary>
    public List<string> uniqueBirds;

    // Constructor
    public UserProfileData(string username, int birdsCaptured, int points, List<string> uniqueBirds, int totalCaptures)
    {
        this.username = username;
        this.birdsCaptured = birdsCaptured;
        this.totalCaptures = totalCaptures;
        this.points = points;
        this.uniqueBirds = uniqueBirds;
    }
}
