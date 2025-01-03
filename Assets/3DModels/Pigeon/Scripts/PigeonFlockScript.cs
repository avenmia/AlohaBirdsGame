using UnityEngine;
using System.Collections;

public class PigeonFlockScript : MonoBehaviour {
	public GameObject pigeonPrefab;
	GameObject[] pigeons;
	public int maxXNum=2;
	public int maxYNum=3;
	public int maxZNum=4;
	int pigeonCount;
	public GameObject pigeonFood;

	void Start () {
		pigeonCount = maxZNum *maxYNum* maxXNum;
		pigeons = new GameObject[pigeonCount];
		for (int k=0; k<maxZNum; k++) {
			for (int j=0; j<maxYNum; j++) {
				for (int i=0; i<maxXNum; i++) {
					int sNum=k*maxXNum*maxYNum+j*maxXNum+i;
					pigeons[sNum]=(GameObject)GameObject.Instantiate (pigeonPrefab, transform.position+Vector3.right*i+Vector3.up*j+Vector3.forward*k, transform.rotation);
					pigeons[sNum].GetComponent<PigeonCharacterScript3>().food=pigeonFood;

				}
			}
		}
	}
}
