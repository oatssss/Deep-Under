using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	//private float lsXDeadValue = 0.058f; //fix dead values in both LS and RS
	//private float lsYDeadValue = 1f;	//fix dead value

	private float speed = 40f;
	Rigidbody rigidbody;
	public Camera camera; 

	public Light spotlight; 
	private bool lightOn = false;

	public Transform lightOrb;
	public Transform lightOrbPosition;
	 

	public float h; 
	public float v; 
	public float u;
	
	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody>();
		spotlight.gameObject.SetActive(lightOn); 
	}

	void FixedUpdate () {
		
		h = Input.GetAxis("Horizontal");
		v = Input.GetAxis("Vertical");	
		if (Input.GetKey (KeyCode.Joystick1Button4)) u = 1;
		else if (Input.GetKey (KeyCode.Joystick1Button5)) u = -1;
		else u = 0;
		if (Input.GetKeyUp(KeyCode.Joystick1Button11)) lightToggle();
		if (Input.GetKeyUp(KeyCode.Joystick1Button0)) createLightOrb();
		Move(h,v,u);
		autoTurn();
		//bob();
		controllerButtonTest(); 
	}

	private void Move (float h, float v, float u) { 
		Vector3 movementHorizontal = (camera.transform.right * h) * speed/2f * Time.deltaTime;
		Vector3 movementForward = (camera.transform.forward * v) * speed * Time.deltaTime; 
		Vector3 movementVertical = (transform.up * u) * speed/2f * Time.deltaTime;
		rigidbody.MovePosition(transform.position + movementHorizontal + movementForward +  movementVertical );
	}


	private void autoTurn () { 
		transform.rotation = Quaternion.RotateTowards (transform.rotation, camera.transform.rotation, 2f);	
	}

	private void bob () { 
		if (v < 1f && h < 1f) { 
			rigidbody.MovePosition(transform.position + (Vector3.up * (Mathf.Sin(Time.time * 2f)) * 0.005f));
		}
	}

	private void lightToggle () { 
		if (lightOn) lightOn = false; 
		else lightOn = true; 
		spotlight.gameObject.SetActive(lightOn);
	}

	private void createLightOrb () { 
		lightOrb.position = lightOrbPosition.transform.position;
		GameObject.Instantiate(lightOrb);
	}

	private void controllerButtonTest() { 
		if (Input.GetKeyDown (KeyCode.Joystick1Button0)) Debug.Log("Square pressed");
		if (Input.GetKeyDown (KeyCode.Joystick1Button1)) Debug.Log("X pressed");
		if (Input.GetKeyDown (KeyCode.Joystick1Button2)) Debug.Log("O pressed");
		if (Input.GetKeyDown (KeyCode.Joystick1Button3)) Debug.Log("Triangle pressed");
		if (Input.GetKeyDown (KeyCode.Joystick1Button4)) Debug.Log("L1");
		if (Input.GetKeyDown (KeyCode.Joystick1Button5)) Debug.Log("R1");
		if (Input.GetKeyDown (KeyCode.Joystick1Button10)) Debug.Log("L3");
		if (Input.GetKeyDown (KeyCode.Joystick1Button11)) Debug.Log("R3");
	}
}
