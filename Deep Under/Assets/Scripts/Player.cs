using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class Player : SmallBoidsFish {

	public float speed = 25f;
	public float normalSpeed = 25f;
	public float acceleration= 20f;
	public float maxSpeed = 100f;
	private float autoTurnSpeed = 10f;
	public bool otherControls = true; 
	bool boosting = false;
	public bool isMoving = false;
    public String nextLevel;

	new private Rigidbody rigidbody;
	new private AudioSource audioSource;

	public AudioClip moveSound; 
	public AudioClip screamSound; 
	public AudioClip aulivEatSound;
	private float eatVolume = 0.35f; 
	private float volumeSave; 
	private float pitchSave; 

	new public CameraFollow camera;
	public GameObject defaultCameraPosition; //is constantly updated by player, so is situated here
	private float aimZoomAmount = 2f; //scalar
	private float aimShiftAmount = 15f; //translational
	private bool isAiming = false;

	public Light spotlight;
	private bool lightOn = true;

	public lightOrb lightOrb;
	public Transform lightOrbPosition;
	private bool shoot = false;
	private float throwForce = 1600f;

	public float h;
	public float v;
	private float a;
	private float l2;
	private float r2;

	public float energy;
	private float maxEnergy = 100f;
	public float energyDrainRate = 8f;
	private float minEnergyDrainRate = 8f;
	public float maxEnergyDrainRate = 24f;
	public float energyDrainRateAcceleration = 2f;
	public bool charging = false;

	public float ghostbar;
	private float maxGhost = 100f;
    public float ghostValue;

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
//	public Alert guiAlert;
	public Light dangerLight;
	[SerializeField] private Color normalcolor;
	[SerializeField] private Color dangercolor;

	[SerializeField] private Animator Animator;
    [SerializeField] private bool Dead;

	public GameObject soundSphere;

	// Use this for initialization
	protected override void Start () {

		rigidbody = GetComponent<Rigidbody>();
		spotlight.gameObject.SetActive(lightOn);
		Cursor.lockState = CursorLockMode.Locked;
		camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>();
		defaultCameraPosition.transform.position = camera.transform.position;
		defaultCameraPosition.transform.rotation = camera.transform.rotation;
        base.Start();
        soundCollider.radius = 0f;
		startPosition = this.transform.position;
		minEnergyDrainRate = energyDrainRate;
//		generalLight = GameObject.Find("Caustics Effect").GetComponent<SceneLight>();
//		guiAlert = GameObject.Find("Alert").GetComponent<Alert>();
		audioSource = GetComponent<AudioSource>();

		this.ghostbar = 0.0f; 	// start have none
	}

	protected override void FixedUpdate () {

		h = Input.GetAxis("Horizontal");
		v = Input.GetAxis("Vertical");
		l2 = Input.GetAxis("Fire2");
		r2 = Input.GetAxis("Fire3");
		if (Input.GetKey(KeyCode.JoystickButton4)) a = 1;
		else if (Input.GetKey(KeyCode.JoystickButton5)) a = -1;
        else {
       		a = Input.GetAxis("Altitude");
        }
        isMoving = !(h == 0f && v == 0f && a == 0f);
        if (shoot) createLightOrb();
		if (isMoving) Move(h,v,a);
		boostAndDrain();
		if (!audioSource.isPlaying || timer > soundDuration) { 
				audioSource.volume = volumeSave;
				audioSource.pitch = pitchSave;
				audioSource.loop = true; 
				audioSource.clip = moveSound;
				audioSource.Play();
			}
		//put sound stuff in a new method
		if (makingSound)
		{
			soundSphere.transform.localScale *= 1.1f;

			timer += Time.deltaTime;

			if (timer > soundDuration)
			{
				timer = 0f;
				makingSound = false;
				soundCollider.radius = 0f;
				soundCollider.enabled = false;
				soundSphere.SetActive (false);
				soundSphere.transform.localScale = new Vector3 (5, 5, 5);
			}
		}

		if (isMoving) {
			if (audioSource.volume <= 1f) audioSource.volume += 0.5f * Time.deltaTime;
		}
		else {
			if (audioSource.volume >= 0.3f) audioSource.volume -= 0.5f * Time.deltaTime;
		}
		if (boosting){
			if (audioSource.pitch < 2) audioSource.pitch += 0.6f * Time.deltaTime;
		}
		else {
			if (audioSource.pitch >= 1) audioSource.pitch -= 0.6f * Time.deltaTime;
		}
	}

	protected override void Update() {
		if (Input.GetKeyUp(KeyCode.JoystickButton9) || Input.GetKeyUp(KeyCode.F)
			|| Input.GetKeyUp(KeyCode.JoystickButton11)) lightToggle();
		if ((Input.GetKeyDown(KeyCode.JoystickButton0)|| Input.GetKeyDown(KeyCode.Mouse0)
			|| Input.GetKeyDown(KeyCode.JoystickButton16)) && !GUIManager.Instance.GamePaused) callCreateLightOrb();
		if (Input.GetKeyUp(KeyCode.JoystickButton1) || Input.GetKeyUp(KeyCode.G)
		|| Input.GetKeyUp(KeyCode.JoystickButton17)) makeSound();
		if (Input.GetKey(KeyCode.JoystickButton8) || Input.GetKey(KeyCode.JoystickButton12)) camera.swingToPosition(defaultCameraPosition.transform);
		//controllerButtonTest();
		//xboxControllerButtonTest();
		if(this.energy > 0) removeEnergy(energyDrainRate);
		else{
			Die();
		}

        base.Update();

		if (this.hasRealDanger())
			dangerLight.color = dangercolor;
		else
			dangerLight.color = normalcolor;
	}

	private void Move (float h, float v, float u) {
        this.Animator.SetFloat("Horizontal", h);
        this.Animator.SetFloat("Vertical", v);
		Vector3 movementHorizontal = (camera.transform.right * h) * speed * Time.deltaTime;
		Vector3 movementForward = (camera.transform.forward * v) * speed * Time.deltaTime;
        Vector3 movementVertical = (Vector3.up * u) * speed/2f * Time.deltaTime;
		rigidbody.MovePosition(transform.position + movementHorizontal + movementForward +  movementVertical);
		if (otherControls) {
			h = Mathf.Clamp(h, -0.15f, 0.15f);
			v = Math.Abs(v);
		}
		Vector3 dir = (camera.transform.forward * v) * speed * Time.deltaTime + (camera.transform.right * h) * speed * Time.deltaTime;
		if (otherControls) { 
			if (v >= 0.2f) turn(dir);
		} 
		else {
			if (Mathf.Abs(h) >= 0.2f && Mathf.Abs(v) >= 0.2f) turn(dir);
		}
	}


	private void removeBoost () {
		if (speed > normalSpeed){
			speed = normalSpeed;
		}
	}

	protected override void RandomizeDirection(){

	}

	private void autoTurn () {
	 	transform.rotation = Quaternion.Lerp(transform.rotation, camera.transform.rotation, autoTurnSpeed * Time.deltaTime);

	}

	private void turn (Vector3 dir) {
		Quaternion lookDirection = Quaternion.LookRotation(dir);
		float turnSpeed = Mathf.Sqrt(Mathf.Pow(h,2) + Mathf.Pow(v,2)) * 32f;
		transform.rotation = Quaternion.Lerp(transform.rotation, lookDirection, turnSpeed * Time.deltaTime);
	}

	private void lightToggle () {
		if (lightOn) lightOn = false;
		else lightOn = true;
		spotlight.gameObject.SetActive(lightOn);
	}

	private void callCreateLightOrb (){
		shoot = true;
	}

	private void createLightOrb () {
       energy -= 5;
        GUIManager.Instance.flashEnergy();
		lightOrb clone = GameObject.Instantiate(lightOrb);
		clone.transform.position = lightOrbPosition.position;
		Vector3 force = transform.forward*throwForce;
		//Debug.Log(force.ToString());
		clone.rb.AddForce(force);
		shoot = false;
	}


	private void makeSound () {
        energy -= 15;
        GUIManager.Instance.flashEnergy();
        soundSphere.transform.localScale = new Vector3 (5, 5, 5);
		volumeSave = audioSource.volume;
		pitchSave = audioSource.pitch;
		audioSource.loop = false; 
		audioSource.clip = screamSound;
		audioSource.Play();
		makingSound = true;
		timer = 0f;
		soundCollider.enabled = true;
		soundCollider.radius = soundRadius;
		soundSphere.SetActive (true);
	}

	public void addEnergy (float orbStrength) {
		if (energy < maxEnergy ){
			energy += orbStrength * Time.deltaTime;
		}
	}
	public void addEnergyBall (float extra) {
		float newEnergy = 0f;
//		guiAlert.Display("Picked up energy",0.5f);
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
	public void addGhost() {
		float newG = 0f;
		if (ghostbar < maxGhost){
			newG = ghostbar + ghostValue;
			audioSource.clip = aulivEatSound;
			audioSource.volume = eatVolume;
			audioSource.loop = false; 
			audioSource.Play();
			if (newG >= maxGhost){
				ghostbar = maxGhost;
				// reload next scene
				// SceneManager.LoadScene (nextLevel);
                GameManager.LoadLevel(nextLevel);
			}
			else {
				ghostbar = newG;
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

	private void boostAndDrain () {

		if (l2 > 0) autoTurn();
		if ((l2 == 1f || Input.GetKey(KeyCode.LeftShift)) && isMoving) {
			// add boost
			if (speed < maxSpeed) {
			speed = speed + (acceleration*Time.fixedDeltaTime);


		}
		boosting = true;
        }

        else {
        	//remove boost
			if (speed > normalSpeed){
			speed = normalSpeed;
		}

		boosting = false;
        }
        //manage energy drain rate
		if (boosting && isMoving) {
			if (energyDrainRate < maxEnergyDrainRate) energyDrainRate += energyDrainRateAcceleration*Time.fixedDeltaTime;
		}
		else {
			if (energyDrainRate > minEnergyDrainRate) energyDrainRate = minEnergyDrainRate ;
		}
	}

	public void Die()
    {
        if (this.Dead)
            { return; }

        this.Dead = true;

        Action reload = () => {
            FishManager.Instance.Reset();
            OrbManager.Instance.Reset();
            GameManager.Instance.WaitForInputToReload();
        };

        GUIManager.Instance.FadeToBattery(reload);
	}

    public override void Eaten(BoidsFish eater)
    {
        if (this.Dead)
            { return; }

        this.Dead = true;

        Action reload = () => {
            FishManager.Instance.Reset();
            OrbManager.Instance.Reset();
            GameManager.Instance.WaitForInputToReload();
        };

        GUIManager.Instance.FadeToEaten(reload);

        base.Eaten(eater);
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

	private void xboxControllerButtonTest() {
		if (Input.GetKeyDown (KeyCode.JoystickButton0)) Debug.Log("A pressed");
		if (Input.GetKeyDown (KeyCode.JoystickButton1)) Debug.Log("B pressed");
		if (Input.GetKeyDown (KeyCode.JoystickButton2)) Debug.Log("X pressed");
		if (Input.GetKeyDown (KeyCode.JoystickButton3)) Debug.Log("Y pressed");
		if (Input.GetKeyDown (KeyCode.JoystickButton4)) Debug.Log("LB");
		if (Input.GetKeyDown (KeyCode.JoystickButton5)) Debug.Log("RB");
		//if (Input.GetKeyDown (KeyCode.JoystickButton11)) Debug.Log("L2");
		//if (Input.GetKeyDown (KeyCode.JoystickButton10)) Debug.Log("R2");
		if (l2 != 0) Debug.Log("LT");
		if (r2 != 0) Debug.Log("RT");
		if (Input.GetKeyDown (KeyCode.JoystickButton9)) Debug.Log("R3");
		if (Input.GetKeyDown (KeyCode.JoystickButton8)) Debug.Log("L3");
	}

	private void xboxControllerButtonTestMAC() {
		if (Input.GetKeyDown (KeyCode.JoystickButton16)) Debug.Log("A pressed");
		if (Input.GetKeyDown (KeyCode.JoystickButton17)) Debug.Log("B pressed");
		if (Input.GetKeyDown (KeyCode.JoystickButton18)) Debug.Log("X pressed");
		if (Input.GetKeyDown (KeyCode.JoystickButton19)) Debug.Log("Y pressed");
		if (Input.GetKeyDown (KeyCode.JoystickButton13)) Debug.Log("LB");
		if (Input.GetKeyDown (KeyCode.JoystickButton14)) Debug.Log("RB");
		//if (Input.GetKeyDown (KeyCode.JoystickButton11)) Debug.Log("L2");
		//if (Input.GetKeyDown (KeyCode.JoystickButton10)) Debug.Log("R2");
		if (l2 != 0) Debug.Log("LT");
		if (r2 != 0) Debug.Log("RT");
		if (Input.GetKeyDown (KeyCode.JoystickButton11)) Debug.Log("R3");
		if (Input.GetKeyDown (KeyCode.JoystickButton12)) Debug.Log("L3");
	}
}
