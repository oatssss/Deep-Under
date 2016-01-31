using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private float speed = 20f;
	private float turnSpeed = 60f;
	Rigidbody rigidbody;
	public Camera camera; 

	public float h; 
	public float v; 
	
	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody>(); 

	}

	void FixedUpdate () {
		h = Input.GetAxisRaw("Horizontal");
		v = Input.GetAxisRaw("Vertical");	
		Move(h,v);
		autoTurn();
	}

	//TODO: movement is jittery when button first pressed; fix. 
	private void Move (float h, float v) { 
		Vector3 movementHorizontal = (transform.right * h) * speed * Time.deltaTime;
		Vector3 movementVertical = (camera.transform.forward * v) * speed * Time.deltaTime; 
		rigidbody.MovePosition(transform.position + movementVertical + movementHorizontal);
	}


	private void autoTurn () { 
		if (v!=0) transform.rotation = Quaternion.RotateTowards (transform.rotation, camera.transform.rotation, 2f);	
	}
}
