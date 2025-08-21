using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserAvidexBird
{
    public BirdData birdData;
    public List<Guid> caughtBirds = new List<Guid>();
    public List<BirdCaptureData> captureData = new List<BirdCaptureData>();

    public UserAvidexBird(GameBird bird)
    {
        caughtBirds.Add(bird.birdData.id);
        birdData = bird.birdData;
    }

    public UserAvidexBird(GameBird bird, List<Guid> caughtBirdIds)
    {
        caughtBirds = caughtBirdIds;
        birdData = bird.birdData;
    }
}
