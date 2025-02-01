using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserAvidexBird
{
    public BirdData birdData;
    public List<BirdCaptureData> captureData;

    public UserAvidexBird(GameBird bird)
    {
        birdData = bird.birdData;
    }
}
