using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private float speed = 20f;
	private float turnSpeed = 60f;
	Rigidbody rigidbody;
	public Camera camera; 

	float h; 
	float v; 
	
	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody>(); 

	}

	void FixedUpdate () {
		h = Input.GetAxisRaw("Horizontal");
		v = Input.GetAxisRaw("Vertical");	
		Move(h,v);
		Turn();
	}

	//TODO: movement is jittery when button first pressed; fix. 
	private void Move (float h, float v) { 
		Vector3 movementHorizontal = (transform.right * h) * speed * Time.deltaTime;
		Vector3 movementVertical = (transform.forward * v) * speed * Time.deltaTime; 
		rigidbody.MovePosition(transform.position + movementHorizontal + movementVertical);
	}

	private void Turn () { 
		if (v!=0) { 
			transform.rotation = Quaternion.RotateTowards(transform.rotation, camera.transform.rotation, turnSpeed * Time.deltaTime );
		}
	}
}
