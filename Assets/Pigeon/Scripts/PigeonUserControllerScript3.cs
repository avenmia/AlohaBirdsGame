using UnityEngine;
using System.Collections;

public class PigeonUserControllerScript3 : MonoBehaviour {
	public PigeonCharacterScript3 pigeonCharacter;
	void Start () {
		pigeonCharacter = GetComponent<PigeonCharacterScript3> ();	
	}
	
	void Update(){
		if (Input.GetKeyDown(KeyCode.L)) {
			pigeonCharacter.Landing();
		}
		
		if (Input.GetButtonDown ("Jump")) {
			pigeonCharacter.Soar ();
		}
		
		if (Input.GetButtonDown ("Fire1")) {
			pigeonCharacter.Attack ();
		}
		
		if (Input.GetKeyDown(KeyCode.E)) {
			pigeonCharacter.Eat();
		}

		if (Input.GetKeyDown(KeyCode.H)) {
			pigeonCharacter.Hop();
		}		
	}
	
	void FixedUpdate(){
		pigeonCharacter.SetForwardAcceleration(Input.GetAxis ("Vertical"));
		pigeonCharacter.turnSpeed=Input.GetAxis ("Horizontal");
	}
	
}
