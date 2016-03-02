using UnityEngine;
using System.Collections;

public class Player : SmallBoidsFish {

	//private float lsXDeadValue = 0.058f; //fix dead values in both LS and RS
	//private float lsYDeadValue = 1f;	//fix dead value

	private float speed = 80f;
	Rigidbody rigidbody;
	public CameraFollow camera;

	public Light spotlight;
	private bool lightOn = false;

	public Transform lightOrb;
	public Transform lightOrbPosition;

	public Transform [] LWingLights;
	public Transform [] RWingLights;

	private float h;
	private float v;
	private float a;

	public float energy;
	private float maxEnergy = 100f;
	public float energyDrainRate;
	public bool charging = false;

	// Use this for initialization
	void Start () {
		energy = 80f;
		rigidbody = GetComponent<Rigidbody>();
		spotlight.gameObject.SetActive(lightOn);
		energyDrainRate = 3f;
	}

	protected override void FixedUpdate () {

		h = Input.GetAxis("Horizontal");
		v = Input.GetAxis("Vertical");
        a = Input.GetAxis("Altitude");
		Move(h,v,a);
		//autoTurn();
		//bob();

	}

	protected override void Update() {
		if (Input.GetKeyUp(KeyCode.Joystick1Button11) || Input.GetKeyUp(KeyCode.F)) lightToggle();
		if (Input.GetKeyUp(KeyCode.Joystick1Button0) || Input.GetKeyUp(KeyCode.Space)) createLightOrb();
		//controllerButtonTest();
		removeEnergy(energyDrainRate);
        base.Update();
	}

	private void Move (float h, float v, float u) {

		Vector3 movementHorizontal = (camera.transform.right * h) * speed * Time.deltaTime;
		Vector3 movementForward = (camera.transform.forward * v) * speed * Time.deltaTime;
        Vector3 movementVertical = (Vector3.up * u) * speed/2f * Time.deltaTime;
		Vector3 dir = movementHorizontal + movementForward;
		if (Mathf.Abs(h) >= 0.2f || Mathf.Abs(v) >= 0.2f) turn(dir);
		rigidbody.MovePosition(transform.position + movementHorizontal + movementForward +  movementVertical);

	}


	private void autoTurn () {
		if (v > 0f && Mathf.Abs(h) < 0.5f){
	 		transform.rotation = Quaternion.RotateTowards (transform.rotation, camera.transform.rotation, 4f);
	 	}
	}

	private void turn (Vector3 dir) {
		Quaternion lookDirection = Quaternion.LookRotation(dir);
		float turnSpeed = Mathf.Sqrt(Mathf.Pow(h,2) + Mathf.Pow(v,2)) * 4f;
		transform.rotation = Quaternion.Lerp(transform.rotation, lookDirection, turnSpeed * Time.deltaTime);
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
