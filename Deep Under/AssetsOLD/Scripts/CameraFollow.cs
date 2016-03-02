using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	Vector3 offset;
	Vector3 defaultLook;  
	Quaternion defaultRotation; 

	public Player player;  
	private float cameraSpeed = 60f;
	private float smoothing = 15f; 
	public float rollSpeed =  3f;

	private float x;
	private float y; 
	private float maxLookUp = 80f;
	private float maxLookDown = 70f;
	public float startLook = 0f; 


	void Start () {
		//dplayer = GetComponent<Player>();
		offset = transform.position - player.transform.position;
		defaultLook = offset;
		defaultRotation = transform.rotation; 

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false; 


	}

	void FixedUpdate () {
		Vector3 oldRot = transform.rotation * Vector3.forward; //get current/old rotation in vector form
		y = Input.GetAxis("Mouse X"); //rotation on y axis
		x = Input.GetAxis("Mouse Y"); //rotation on x axis
		cameraRotate(x,y);
		Vector3 newRot = transform.rotation * Vector3.forward; // save new rotation in vector form 
		Quaternion change = Quaternion.FromToRotation(oldRot, newRot); //record change in rotation using the 2 vectors
		offset = change * offset; //apply change in rotation to offset
		Vector3 targetPos = player.transform.position + offset; // apply offset
		if (Vector3.Distance(transform.position, targetPos) > 0.1f) transform.position = Vector3.Lerp(transform.position, targetPos,(1 - Mathf.Exp( -smoothing * Time.deltaTime ))); //TODO: jitter caused by smoothing.
		//transform.position = targetPos;

	}

	void Update () { 
		/*if (Input.GetKeyDown(KeyCode.Joystick1Button2)) {
			swing(30f);
			Debug.Log ("Swing!");
		}*/
	}

    

    private void cameraRotate (float x, float y) { 
		transform.RotateAround(transform.position, transform.up, y * cameraSpeed * Time.deltaTime); 
		if (x < 0) { 
			if (startLook < maxLookUp) { 
				startLook -= x * cameraSpeed * Time.deltaTime;
				transform.RotateAround(transform.position, transform.right, x * cameraSpeed * Time.deltaTime);
			}
		}
		else if (x > 0) { 
			if (startLook > -maxLookDown) { 
				startLook -= x * cameraSpeed * Time.deltaTime;
				transform.RotateAround(transform.position, transform.right, x * cameraSpeed * Time.deltaTime);
			}
		}
		transform.rotation = Quaternion.Euler(new Vector3 (transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0f));
    }

    private void swing (float degrees) { 
    	Vector3 swingVector = new Vector3 (0f, degrees, 0f);
    	Quaternion swingAmount = Quaternion.Euler(swingVector);
    	offset = swingAmount * offset; 
		Vector3 targetPos = player.transform.position + offset;
		transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 0.1f); 
		transform.Rotate(swingVector);
    }


	/*//OBSOLETE
    private void fixRoll(){
    	if (transform.rotation.eulerAngles.z > 2f && transform.rotation.eulerAngles.z < 358f) {
	    	if (transform.rotation.eulerAngles.z > 180f) { 
	    		transform.RotateAround(transform.position, transform.forward, Time.deltaTime * cameraSpeed * rollSpeed);
	    	}
	    	else {
	    	transform.RotateAround(transform.position, transform.forward, -Time.deltaTime * cameraSpeed * rollSpeed);
	    	}
		}
    }*/
		

}
