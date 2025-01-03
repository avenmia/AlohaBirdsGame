using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnOwl : MonoBehaviour
{
    private Animator barnowl;
    public GameObject MainCamera;

	// Use this for initialization
	void Start ()
    {
        barnowl = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (barnowl.GetCurrentAnimatorStateInfo(0).IsName("flying"))
        {
            barnowl.SetBool("landing", false);
            barnowl.SetBool("takeoff", false);
        }
        if (barnowl.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            barnowl.SetBool("eating", false);
            barnowl.SetBool("catch", false);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            barnowl.SetBool("idle", true);
            barnowl.SetBool("walking", false);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            barnowl.SetBool("walking", true);
            barnowl.SetBool("idle", false);
            barnowl.SetBool("turnleft", false);
            barnowl.SetBool("turnright", false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            barnowl.SetBool("flyright", true);
            barnowl.SetBool("turnright", true);
            barnowl.SetBool("flyleft", false);
            barnowl.SetBool("flying", false);
            barnowl.SetBool("idle", false);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            barnowl.SetBool("flyleft", true);
            barnowl.SetBool("turnleft", true);
            barnowl.SetBool("flyright", false);
            barnowl.SetBool("flying", false);
            barnowl.SetBool("idle", false);
        }
        if ((Input.GetKeyUp(KeyCode.A))|| (Input.GetKeyUp(KeyCode.D)))
        {
            barnowl.SetBool("flyleft", false);
            barnowl.SetBool("flyright", false);
            barnowl.SetBool("flying", true);
            barnowl.SetBool("turnleft", false);
            barnowl.SetBool("turnright",false);
            barnowl.SetBool("idle", true);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            barnowl.SetBool("idle", false);
            barnowl.SetBool("takeoff", true);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            barnowl.SetBool("landing", true);
            barnowl.SetBool("flying", false);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            barnowl.SetBool("glide", true);
            barnowl.SetBool("flying", false);
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            barnowl.SetBool("flying", true);
            barnowl.SetBool("glide", false);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            barnowl.SetBool("catch", true);
            barnowl.SetBool("flying", false);
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            barnowl.SetBool("hited", true);
            barnowl.SetBool("flying", false);
        }
        if (Input.GetKeyUp(KeyCode.Keypad1))
        {
            barnowl.SetBool("hited", false);
            barnowl.SetBool("flying", true);
        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            barnowl.SetBool("death", true);
            barnowl.SetBool("idle", false);
        }
        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            MainCamera.GetComponent<CameraFollow>().enabled = false;
        }
        if (Input.GetKeyUp(KeyCode.RightControl))
        {
            MainCamera.GetComponent<CameraFollow>().enabled = true;
        }

    }
}
