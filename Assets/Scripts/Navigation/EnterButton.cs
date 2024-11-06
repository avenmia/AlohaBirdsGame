using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterButton : MonoBehaviour
{
    public void OnEnterButtonClicked(string sceneName)
    {
        NavigationManager.Instance.LoadNewScene(sceneName);
    }
}
