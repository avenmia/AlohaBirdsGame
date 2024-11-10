using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public TMP_Text userNameText;
    public TMP_Text birdCapturedCountText;
    public TMP_Text totalCapturesText;
    public TMP_Text pointsText;
    // Start is called before the first frame update
    void Start()
    {
        var player = PersistentDataManager.Instance.userProfileData;
        userNameText.text = player.username;
        birdCapturedCountText.text = $"Birds Captured Count: {player.birdsCaptured}";
        pointsText.text = $"Total Points: {player.points}";
        totalCapturesText.text = $"Total Captures: {player.totalCaptures}";

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
