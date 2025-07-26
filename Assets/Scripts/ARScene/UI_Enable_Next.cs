using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Enable_Next : MonoBehaviour
{
    private bool Next = false;
    [SerializeField] private TMP_Text Enabled_Next_Text;
    private void OnEnable()
    {
        StartCoroutine(Delayed_Enabled(1f));
    }

    private IEnumerator Delayed_Enabled(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Enabled_Next_Text.enabled = true;
        Next = true;
    }

    public void Reset_Object()
    {
        Enabled_Next_Text.enabled = false;
        Next = false;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if(Input.GetMouseButtonUp(0) && Next)
        {
            this.gameObject.transform.parent.GetComponent<AREnd_Sequence>().Next_Sequence();
        }
#else
        if (Input.touchCount > 0 && Next)
        {
            Touch touch = Input.GetTouch(0);
            if (Next && touch.phase == TouchPhase.Began)
            {
                this.gameObject.transform.parent.GetComponent<AREnd_Sequence>().Next_Sequence();
            }
        }
#endif
    }
}
