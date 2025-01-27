using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sign_In_Toggle : MonoBehaviour
{
    [SerializeField] private GameObject Sign_In_Path;
    [SerializeField] private GameObject Register_Path;
    [SerializeField] private Button R_Button;
    [SerializeField] private Button SI_Button;
    //[SerializeField] private Color Faded_Color;
    [SerializeField] private Color Full_Color;

    public void Register_Button()
    {
        Register_Path.gameObject.SetActive(true);
        Sign_In_Path.gameObject.SetActive(false);
        R_Button.interactable = false;
        SI_Button.gameObject.GetComponent<Image>().color = Full_Color;
        //R_Button.gameObject.GetComponent<Image>().color = Faded_Color;
        SI_Button.interactable = true;
    }

    public void Sign_In_Button()
    {
        Register_Path.gameObject.SetActive(false);
        Sign_In_Path.gameObject.SetActive(true);
        SI_Button.interactable = false;
        R_Button.gameObject.GetComponent<Image>().color = Full_Color;
        //SI_Button.gameObject.GetComponent<Image>().color = Faded_Color;
        R_Button.interactable = true;
    }
}
