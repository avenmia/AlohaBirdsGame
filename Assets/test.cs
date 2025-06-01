using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private PersistentDataManager persistentDataManagerObj;

    private void Start()
    {
        persistentDataManagerObj = GameObject.FindObjectOfType<PersistentDataManager>().GetComponent<PersistentDataManager>();
    }
    void Update()
    {
        text.text = "Persistent Data Manager Username: " + PersistentDataManager.Instance.userProfileData.username
                    + "\n Persistent Data Manager Object: " + persistentDataManagerObj.userProfileData.username;
    }
}
