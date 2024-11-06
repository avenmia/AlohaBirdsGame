using UnityEngine;
using System.Collections;

public class PigeonAIController : MonoBehaviour {
	public PigeonCharacterScript3 pigeonCharacter;

	void Start () {
		pigeonCharacter = GetComponent<PigeonCharacterScript3> ();	
	}

	void SeekFood(){
		Vector3 targetRelPos = pigeonCharacter.food.transform.position - transform.position;
		if (targetRelPos.sqrMagnitude < 5f && pigeonCharacter.isFlying && !pigeonCharacter.soaring) {
			pigeonCharacter.tryingToLand = true;
			pigeonCharacter.SetForwardAcceleration (-1f);
			pigeonCharacter.SetUpSpeed (-1f);
		} else if (targetRelPos.sqrMagnitude > 30f && !pigeonCharacter.isFlying && !pigeonCharacter.tryingToLand) {
			pigeonCharacter.Soar ();
		} else if (targetRelPos.sqrMagnitude < .083f) {
			pigeonCharacter.SetForwardAcceleration(0f);
			pigeonCharacter.Eat();
		}else{
			pigeonCharacter.SetForwardAcceleration(.5f);
		}
		targetRelPos.Normalize();
		pigeonCharacter.turnSpeed = -Vector3.Dot (targetRelPos,transform.forward);
	}

	void OnCollisionEnter(Collision other){
		if(other.gameObject.GetComponent<PigeonCharacterScript3>()){
			pigeonCharacter.Soar();
		}
	}

	void FixedUpdate(){
		SeekFood ();
	}
}
