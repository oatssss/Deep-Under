using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private float speed = 80f;
	private float autoTurnSpeed = 10f; 
	Rigidbody rigidbody;

	public CameraFollow camera;
	public GameObject defaultCameraPosition; //is constantly updated by player, so is situated here
	private float aimZoomAmount = 2f; //scalar
	private float aimShiftAmount = 15f; //translational
	public bool isAiming = false; 

	public Light spotlight;
	private bool lightOn = false;

	public lightOrb lightOrb;
	public Transform lightOrbPosition;
	public bool shoot = false; 
	private float throwForce = 1600f;

	private float h;
	private float v;
	private float a;

	public float energy;
	private float maxEnergy = 100f;
	public float energyDrainRate;
	public bool charging = false;

	LineRenderer lineRenderer;
	private int lineSmoothness = 10; 

	// Use this for initialization
	void Start () {
		energy = 80f;
		rigidbody = GetComponent<Rigidbody>();
		spotlight.gameObject.SetActive(lightOn);
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.SetVertexCount(lineSmoothness); // set according to how smooth we want line to be
		lineRenderer.useWorldSpace = true;
		camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>();
		defaultCameraPosition.transform.position = camera.transform.position;
		defaultCameraPosition.transform.rotation = camera.transform.rotation;
		energyDrainRate = 3f;

	}

	void FixedUpdate () {
		//Debug.DrawLine(defaultCameraPosition.transform.position, transform.position);
		//Debug.DrawLine(camera.transform.position, transform.position);
		Debug.DrawRay(transform.position, transform.forward*10f);
		h = Input.GetAxis("Horizontal");
		v = Input.GetAxis("Vertical");
		if (Input.GetKey(KeyCode.Joystick1Button4)) a = 1; 
		else if (Input.GetKey(KeyCode.Joystick1Button5)) a = -1; 
        else {
       		a = Input.GetAxis("Altitude");
        }
		if (Input.GetKey(KeyCode.Joystick1Button6) || Input.GetKey(KeyCode.LeftShift)) { 
        	autoTurn();	
        }
        if (shoot) createLightOrb();
		Move(h,v,a);

		//autoTurn();
		//bob();

	}

	void Update() {
		if (Input.GetKeyUp(KeyCode.Joystick1Button11) || Input.GetKeyUp(KeyCode.F)) lightToggle();
		if (Input.GetKeyUp(KeyCode.Joystick1Button7) || Input.GetKeyUp(KeyCode.Mouse0)) callCreateLightOrb();
		//controllerButtonTest();
		removeEnergy(energyDrainRate);
		if (Input.GetKeyDown(KeyCode.Joystick1Button6) || Input.GetKeyDown(KeyCode.LeftShift)) { 
        	aim(aimZoomAmount, aimShiftAmount);	
        }
		if (Input.GetKeyUp(KeyCode.Joystick1Button6) || Input.GetKeyUp(KeyCode.LeftShift)) { 
        	exitAim(aimZoomAmount, aimShiftAmount);	
        }
	}

	private void Move (float h, float v, float u) {

		Vector3 movementHorizontal = (camera.transform.right * h) * speed * Time.deltaTime;
		Vector3 movementForward = (camera.transform.forward * v) * speed * Time.deltaTime;
        Vector3 movementVertical = (Vector3.up * u) * speed/2f * Time.deltaTime;
		Vector3 dir = movementHorizontal + movementForward;
		if ((Mathf.Abs(h) >= 0.2f || Mathf.Abs(v) >= 0.2f) && !isAiming) turn(dir);
		rigidbody.MovePosition(transform.position + movementHorizontal + movementForward +  movementVertical);

	}


	private void autoTurn () {
	 	transform.rotation = Quaternion.Lerp(transform.rotation, camera.transform.rotation, autoTurnSpeed * Time.deltaTime);
	 	
	}

	private void turn (Vector3 dir) {
		Quaternion lookDirection = Quaternion.LookRotation(dir);
		float turnSpeed = Mathf.Sqrt(Mathf.Pow(h,2) + Mathf.Pow(v,2)) * 4f;
		transform.rotation = Quaternion.Lerp(transform.rotation, lookDirection, turnSpeed * Time.deltaTime);
	}



	private void lightToggle () {
		if (lightOn) lightOn = false;
		else lightOn = true;
		spotlight.gameObject.SetActive(lightOn);
	}

	private void createLightOrb () {
		lightOrb clone = GameObject.Instantiate(lightOrb);
		clone.transform.position = lightOrbPosition.position;
		Vector3 force = transform.forward*throwForce;
		Debug.Log(force.ToString());
		clone.rb.AddForce(force);
		shoot = false; 
	}

	private void callCreateLightOrb (){ 
		if (isAiming) shoot = true;
	}

	public void addEnergy (float orbStrength) {
		if (energy < maxEnergy ){
			energy += orbStrength * Time.deltaTime;
//			Debug.Log(energy);
		}
	}
	public void addEnergyBall (float extra) {
		float newEnergy = 0f;
		if (energy < maxEnergy ){
			newEnergy = energy + extra;
			if (newEnergy >= maxEnergy) {
				energy = maxEnergy;
			}
			else {
				energy = newEnergy;
			}
		}
	}

	/* time decreases energy and other fishes decrease energy; rate fluctuates depeding on whether auliv is near another fish */
	public void removeEnergy (float rate) {
		if (energy > 0f && !charging) {
			energy -= rate * Time.deltaTime;
//			Debug.Log(energy);
		}
	}

	private void aim (float zoomAmount, float shiftAmount) { 
		isAiming = true; 
		camera.translateHorizontal(shiftAmount);	//translate before offset length is altered
		camera.scaleOffsetLength(1f/zoomAmount); //zoom in 
		//TODO: show throwing line
		drawAim(); 

	}

	private void exitAim (float zoomAmount, float shiftAmount) {
		isAiming = false; 
		camera.scaleOffsetLength(zoomAmount); //zoom out
		camera.translateHorizontal(-shiftAmount); //translate after offset is put back to orig length 
	}

	private void drawAim () { 
		

	}


	
	private void controllerButtonTest() {
		if (Input.GetKeyDown (KeyCode.Joystick1Button0)) Debug.Log("Square pressed");
		if (Input.GetKeyDown (KeyCode.Joystick1Button1)) Debug.Log("X pressed");
		if (Input.GetKeyDown (KeyCode.Joystick1Button2)) Debug.Log("O pressed");
		if (Input.GetKeyDown (KeyCode.Joystick1Button3)) Debug.Log("Triangle pressed");
		if (Input.GetKeyDown (KeyCode.Joystick1Button4)) Debug.Log("L1");
		if (Input.GetKeyDown (KeyCode.Joystick1Button5)) Debug.Log("R1");
		if (Input.GetKeyDown (KeyCode.Joystick1Button6)) Debug.Log("L2");
		if (Input.GetKeyDown (KeyCode.Joystick1Button7)) Debug.Log("R2");
		if (Input.GetKeyDown (KeyCode.Joystick1Button10)) Debug.Log("L3");
		if (Input.GetKeyDown (KeyCode.Joystick1Button11)) Debug.Log("R3");
	}
}
