using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private float speed = 20f;
	//private float turnSpeed = 60f;
	Rigidbody rigidbody;
	public Camera camera; 

	private float h; 
	private float v; 
	private float u;
	
	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody>(); 

	}

	void FixedUpdate () {
		h = Input.GetAxisRaw("Horizontal");
		v = Input.GetAxisRaw("Vertical");	
		if (Input.GetKey(KeyCode.Q)) u = 1;
		else if (Input.GetKey(KeyCode.E)) u = -1;
		else u = 0;
		Move(h,v,u);
		autoTurn();
		bob();
	}

	private void Move (float h, float v, float u) { 
		Vector3 movementHorizontal = (camera.transform.right * h) * speed/4f * Time.deltaTime;
		Vector3 movementForward = (camera.transform.forward * v) * speed * Time.deltaTime; 
		Vector3 movementVertical = (transform.up * u) * speed/4f * Time.deltaTime;
		rigidbody.MovePosition(transform.position + movementVertical + movementHorizontal + movementForward);
	}


	private void autoTurn () { 
		if (v!=0) transform.rotation = Quaternion.RotateTowards (transform.rotation, camera.transform.rotation, 2f);	
	}

	private void bob () { 
		if (v == 0 && h == 0) { 
			rigidbody.MovePosition(transform.position + (Vector3.up * (Mathf.Sin(Time.time * 2f)) * 0.005f));
		}
	}
}
