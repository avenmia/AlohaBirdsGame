using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AR_Bird_Movement : MonoBehaviour
{
    //Create generic flight patterns
    [SerializeField] private List<Vector3> Bird_Path;
    [SerializeField] private Camera Camera;
    [SerializeField] private bool Loop = true;
    private void Start()
    {
        DOTween.Init();
        StartCoroutine(Fly_To_Points());
    }
    IEnumerator Fly_To_Points()
    {
        foreach(var path in Bird_Path)
        {
            Debug.Log("Flying to point:" + path);
            transform.DOMove(path, 3, false);
            transform.DODynamicLookAt(path, 1);
            yield return new WaitForSeconds(2.5f);
        }

        if(Loop) { StartCoroutine(Fly_To_Points()); }
    }

    public void Bird_To_Camera()
    {
        var dist = Mathf.Abs(Vector3.Distance(this.gameObject.transform.position, Camera.transform.position));
        Debug.Log("Distance from bird to camera: " + dist);
    }
}
