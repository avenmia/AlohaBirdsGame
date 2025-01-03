using UnityEngine;
using System.Collections;

public class PigeonCharacterScript3 : MonoBehaviour {
	public Animator pigeonAnimator;
	public float pigeonSpeed=1f;
	Rigidbody pigeonRigid;
	public bool isFlying=false;
	public float rotateSpeed=.2f;
	public float forwardSpeed=0f;
	public float turnSpeed=0f;
	public float upSpeed=0f;
	public float groundCheckDistance=5f;
	public float forwardCheckDistance=100f;
	public bool isGrounded=false;
	public bool soaring=false;
	public float forwardAcceleration=0f;
	public float maxForwardSpeed=3f;
	public bool tryingToLand=false;
	public GameObject food;

	void Start () {
		pigeonAnimator = GetComponent<Animator> ();
		pigeonAnimator.speed = pigeonSpeed;
		pigeonRigid = GetComponent<Rigidbody> ();
	}	

	void FixedUpdate(){
		Move ();

		ForwardCheck ();
       // Debug.Log(pigeonAnimator.GetCurrentAnimatorClipInfo(0).Length);
		if (pigeonAnimator.GetCurrentAnimatorClipInfo (0) [0].clip.name == "Flap1" || pigeonAnimator.GetCurrentAnimatorClipInfo (0) [0].clip.name == "Glide"  ) {
			if(soaring){
				soaring=false;
				pigeonAnimator.SetBool ("IsSoaring",false);
				tryingToLand=true;
				forwardAcceleration=1f;
				upSpeed=1f;
			}
		}
		if(!soaring){
			GroundedCheck();
		}
	}

	void ForwardCheck(){
		RaycastHit hit;
		if (Physics.Raycast (transform.position, transform.right, out hit, forwardCheckDistance)) {
			if(hit.distance<3f){
				forwardSpeed=0f;
			}
		}
	}

	void GroundedCheck(){
		RaycastHit hit;
		if (Physics.Raycast (transform.position, Vector3.down, out hit, groundCheckDistance)) {
			isGrounded = true;
			if(tryingToLand){
				Landing();
				tryingToLand=false;
			}

		} else {
			isGrounded=false;
		}

	}

	public void SetUpSpeed(float sp){
		if (!soaring) {

			upSpeed=Mathf.Lerp(upSpeed,sp,Time.deltaTime);
		}
	}

	public void SetForwardAcceleration(float ac){
		if (!soaring) {
			forwardAcceleration=ac;
		}
	}



	public void Landing(){
		if (isFlying) {
			pigeonAnimator.SetTrigger ("Landing");
			pigeonAnimator.applyRootMotion = true;
			pigeonRigid.useGravity=true;
			isFlying = false;
			soaring=false;
			pigeonAnimator.SetBool ("IsSoaring",false);
		}
	}
	
	public void Soar(){
		if(!isFlying && !tryingToLand){
			pigeonAnimator.SetBool ("IsSoaring",true);
			forwardSpeed=.02f;
			pigeonAnimator.applyRootMotion = false;
			pigeonRigid.useGravity=false;
			upSpeed=0.5f;
			isFlying = true;
			isGrounded=false;
			soaring=true;
		}
	}
	
	public void Attack(){
		pigeonAnimator.SetTrigger ("Attack");
	}

	public void Eat(){
		pigeonAnimator.SetTrigger ("Eat");
	}

	public void Hop(){
		pigeonAnimator.SetTrigger ("Hop");
	}


	public void Move(){
		pigeonAnimator.SetFloat ("Forward",forwardAcceleration);
		pigeonAnimator.SetFloat ("Turn",turnSpeed);

		if(isFlying) {
			pigeonRigid.velocity=transform.up*upSpeed+transform.right*forwardSpeed*3f;

			forwardSpeed=Mathf.Clamp( forwardSpeed+forwardAcceleration*Time.deltaTime,0f,maxForwardSpeed);

			if(!tryingToLand){
				upSpeed=Mathf.Lerp(upSpeed,forwardAcceleration,Time.deltaTime);
			}else{
				upSpeed=Mathf.Lerp(upSpeed,forwardAcceleration-.8f,Time.deltaTime);
			}

			transform.RotateAround(transform.position,Vector3.up,Time.deltaTime*turnSpeed*100f);
		}
	}
}
