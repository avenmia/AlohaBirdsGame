using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AR_Bird_Movement : MonoBehaviour
{
    //Create generic flight patterns
    [SerializeField] private List<Vector3> Bird_Path;
    [SerializeField] private Camera Camera;
    [SerializeField] private GameObject Hide_Aura;
    private void Start()
    {
        if(Hide_Aura != null)
        {
            Hide_Aura.SetActive(false);
        }
        DOTween.Init();
        Camera = Camera.main;
        Generate_Flight_Paths();
    }
    private void Generate_Flight_Paths()
    {
        //Can not be too close to player
        //Must be a nice distance away ~ -75-75 is a nice distance (x, z)
        //can't be too high ~ -5 - 25 testing (y)
        //can't go through player
        Bird_Path.Clear();
        Bird_Path.Add(new Vector3(Random.Range(25, 75), Random.Range(-5, 25), Random.Range(-50, 50))); //(x,-y/y)
        Bird_Path.Add(new Vector3(Random.Range(-50, 75), Random.Range(-5, 25), Random.Range(25, 50))); //(-x/x, y)
        Bird_Path.Add(new Vector3(Random.Range(-50, -25), Random.Range(-5, 25), Random.Range(-50, 50))); //(-x, -y/y)
        Bird_Path.Add(new Vector3(Random.Range(-50, 50), Random.Range(-5, 25), Random.Range(-50, -25))); //(-x/x, -y)
        Init_Fly_To_Points();
    }

    public void Init_Fly_To_Points()
    {
        StartCoroutine(Fly_To_Points());
    }

    IEnumerator Fly_To_Points()
    {
        for(int i = 0; i<Bird_Path.Count; i++)
        {
            Debug.Log("Flying to point:" + Bird_Path[i]);
            transform.DOLocalMove(Bird_Path[i], 3, false);
            transform.DODynamicLookAt(Bird_Path[i], 1);
            yield return new WaitForSeconds(3);
            transform.DODynamicLookAt(Camera.transform.position, 1);
            yield return new WaitForSeconds(Random.Range(0, 3));
        }

        Generate_Flight_Paths();
    }

    public void Bird_To_Camera()
    {
        var dist = Mathf.Abs(Vector3.Distance(this.gameObject.transform.position, Camera.transform.position));
        Debug.Log("Distance from bird to camera: " + dist);
    }
}
