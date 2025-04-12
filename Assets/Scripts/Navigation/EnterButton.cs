using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterButton : MonoBehaviour
{
    public void Create_Prefab(GameObject UI_Prefab)
    {
        Instantiate(UI_Prefab);
    }
}
