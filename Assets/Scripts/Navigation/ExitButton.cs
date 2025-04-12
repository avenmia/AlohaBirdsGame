using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    //public void OnExitButtonClicked()
    //{
    //    NavigationManager.Instance.ReturnToPrevScene();
    //}

    public void Destroy_UI_Object(GameObject UIPrefab)
    {
        Destroy(UIPrefab);
    }
}
