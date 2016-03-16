using UnityEngine;
using System.Collections;

public class Player : SmallBoidsFish {

	public float speed = 80f;
	private float autoTurnSpeed = 10f;
	new private Rigidbody rigidbody;

	new public CameraFollow camera;
	public GameObject defaultCameraPosition; //is constantly updated by player, so is situated here
	private float aimZoomAmount = 2f; //scalar
	private float aimShiftAmount = 15f; //translational
	private bool isAiming = false;

	public Light spotlight;
	private bool lightOn = false;

	public lightOrb lightOrb;
	public Transform lightOrbPosition;
	private bool shoot = false;
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

	public SphereCollider soundCollider;
	private bool makingSound = false;
	public float soundRadius = 40f;
	public float soundDuration = 10f;
	private float timer = 0f;

	public pod lastPod = null;
	private Vector3 startPosition;
//	public SceneLight generalLight;
	public Alert guiAlert;

	// Use this for initialization
	protected override void Start () {
		energy = 80f;
		rigidbody = GetComponent<Rigidbody>();
		spotlight.gameObject.SetActive(lightOn);
		Cursor.lockState = CursorLockMode.Locked;
		//lineRenderer = GetComponent<LineRenderer>();
		//lineRenderer.SetVertexCount(lineSmoothness); // set according to how smooth we want line to be
		//lineRenderer.useWorldSpace = true;
		camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>();
		defaultCameraPosition.transform.position = camera.transform.position;
		defaultCameraPosition.transform.rotation = camera.transform.rotation;
		energyDrainRate = 3f;
        base.Start();
        soundCollider.radius = 0f;
		startPosition = this.transform.position;
//		generalLight = GameObject.Find("Caustics Effect").GetComponent<SceneLight>();
		guiAlert = GameObject.Find("Alert").GetComponent<Alert>();
	}

	protected override void FixedUpdate () {

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
		if (makingSound) timer += Time.deltaTime;
		if (timer > soundDuration) {
			timer = 0f;
			makingSound = false;
			soundCollider.radius = 0f;
		}
		//autoTurn();
		//bob();

	}

	protected override void Update() {
		if (Input.GetKeyUp(KeyCode.Joystick1Button11) || Input.GetKeyUp(KeyCode.F)) lightToggle();
		if (Input.GetKeyUp(KeyCode.Joystick1Button7) || Input.GetKeyUp(KeyCode.Mouse0)) callCreateLightOrb();
		if (Input.GetKeyUp(KeyCode.Joystick1Button2)) makeSound();
		//controllerButtonTest();
		if(this.energy > 0) removeEnergy(energyDrainRate);
		else Die();

		if (Input.GetKeyDown(KeyCode.Joystick1Button6) || Input.GetKeyDown(KeyCode.LeftShift)) {
        	aim(aimZoomAmount, aimShiftAmount);
        }
		if (Input.GetKeyUp(KeyCode.Joystick1Button6) || Input.GetKeyUp(KeyCode.LeftShift)) {
        	exitAim(aimZoomAmount, aimShiftAmount);
        }
        base.Update();
	}

	private void Move (float h, float v, float u) {

		Vector3 movementHorizontal = (camera.transform.right * h) * speed * Time.deltaTime;
		Vector3 movementForward = (camera.transform.forward * v) * speed * Time.deltaTime;
        Vector3 movementVertical = (Vector3.up * u) * speed/2f * Time.deltaTime;
		Vector3 dir = movementHorizontal + movementForward;
		if ((Mathf.Abs(h) >= 0.2f || Mathf.Abs(v) >= 0.2f) && !isAiming) turn(dir);
		rigidbody.MovePosition(transform.position + movementHorizontal + movementForward +  movementVertical);

	}

	protected override void RandomizeDirection(){

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
		//Debug.Log(force.ToString());
		clone.rb.AddForce(force);
		shoot = false;
	}

	private void callCreateLightOrb (){
		if (isAiming) shoot = true;
	}

	private void makeSound () {
		makingSound = true;
		timer = 0f;
		soundCollider.radius = soundRadius;
	}

	public void addEnergy (float orbStrength) {
		if (energy < maxEnergy ){
			energy += orbStrength * Time.deltaTime;
//			Debug.Log(energy);
		}
	}
	public void addEnergyBall (float extra) {
		float newEnergy = 0f;
		guiAlert.Display("Picked up energy",0.5f);
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
		camera.swingToPosition(defaultCameraPosition.transform);
	}

	private void drawAim () {


	}

	public void Die() {
		guiAlert.Display("You died.",1.5f);
		if (this.lastPod == null)
			Teleport(startPosition);
		else
			Teleport(this.lastPod.transform.position);
		this.energy = maxEnergy;
	}

	private void Teleport(Vector3 p){
//		generalLight.lights(false);
//		Wait();
		this.transform.position = p;
//		generalLight.lights(true);
	}

	IEnumerator Wait(){
		//TODO: add disable movement
		yield return new WaitForSeconds (2.0f);
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
