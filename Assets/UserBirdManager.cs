using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CapturedBirdsData
{
    public List<string> capturedBirdNames = new List<string>();
}

public class UserBirdManager : MonoBehaviour
{
    public static UserBirdManager Instance;
    private void Awake() => Instance = this;

    public List<BirdData> capturedBirds = new List<BirdData>();

    public void CaptureBird(BirdData bird)
    {
        if (!capturedBirds.Contains(bird))
        {
            capturedBirds.Add(bird);
            SaveProgress();
        }
    }

    public void SaveProgress()
    {
        CapturedBirdsData data = new CapturedBirdsData();
        foreach (BirdData bird in capturedBirds)
        {
            data.capturedBirdNames.Add(bird.birdName);
        }
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/capturedBirds.json", json);
    }

    public void LoadProgress()
    {
        string path = Application.persistentDataPath + "/capturedBirds.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            CapturedBirdsData data = JsonUtility.FromJson<CapturedBirdsData>(json);
            foreach (string birdName in data.capturedBirdNames)
            {
                BirdData bird = Resources.Load<BirdData>($"BirdData/{birdName}");
                if (bird != null)
                {
                    capturedBirds.Add(bird);
                }
            }
        }
    }
}
