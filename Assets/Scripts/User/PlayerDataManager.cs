using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text userNameText;
    public TMP_Text birdCapturedCountText;
    public TMP_Text totalCapturesText;
    public TMP_Text pointsText;

    [Header("Gallery Configuration")]
    public GameObject birdPicButtonPrefab; // Assign the birdPicButtonPrefab here
    public Transform galleryContentParent; // Assign the Content GameObject of the ScrollView


    // Start is called before the first frame update
    void Start()
    {
        var player = PersistentDataManager.Instance.userProfileData;
        userNameText.text = player.username.ToString();
        birdCapturedCountText.text = player.birdsCaptured.ToString();
        pointsText.text = player.points.ToString();
        totalCapturesText.text = player.totalCaptures.ToString();
        Debug.Log("PlayerDataManager: Player data initialized.");
    }

}
