using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    // private void Start()
    // {
    //     GetComponent<Button>().onClick.AddListener(OnExitButtonClicked);
    // }

    public void OnExitButtonClicked()
    {
        NavigationManager.Instance.ReturnToPrevScene();
    }
}
