using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Vector3 offset;  
	public float offsetLength;
	Quaternion defaultRotation; 

	private float time = 0f; 

	public Player player;  
	public float cameraSpeed = 120f;
	public float defaultSmoothing = 15f;
	public float smoothing = 15f;
    public bool isMoving = false; 
    public bool buffer = false; 

	private float x;
	private float y; 
	private float maxLookUp = 70f;
	private float maxLookDown = 70f;
	public float startLook = 0f; 

	public Vector3 oldPosition;


	void Start () {
		//dplayer = GetComponent<Player>();
		offset = transform.position - player.transform.position;
		offsetLength = offset.magnitude;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false; 
	}

	void FixedUpdate () {
		y = Input.GetAxis("Mouse X"); //rotation on y axis
		x = Input.GetAxis("Mouse Y"); //rotation on x axis
		oldPosition = transform.position; 
		follow();
		getMovementDir();
		Vector3 camToDefault = player.defaultCameraPosition.transform.position - transform.position; 
		//time += Time.deltaTime;
	}



	public void follow () { 
		Vector3 oldRot = transform.rotation * Vector3.forward; //get current/old rotation in vector form
		if ((player.h > 0f && y < 0f) || (player.h < 0f && y > 0f)) cameraRotate(x,y);
		else cameraRotate(x, y + player.h);
		Vector3 newRot = transform.rotation * Vector3.forward; // save new rotation in vector form 
		Quaternion change = Quaternion.FromToRotation(oldRot, newRot); //record change in rotation using the 2 vectors
		offset = change * offset; //apply change in rotation to offset
		Vector3 targetPos = player.transform.position + offset; //apply offset 
		if (Vector3.Distance(transform.position, targetPos) > 0.1f) transform.position = Vector3.Lerp(transform.position, targetPos,(1 - Mathf.Exp( -smoothing * Time.deltaTime ))); //TODO: jitter caused by smoothing.
		//transform.position = targetPos;
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


    public void scaleOffsetLength (float amount) { 
    	Vector3 scaledOffset = offset;
		scaledOffset.Scale(new Vector3(amount, amount, amount));
		offset = Vector3.Lerp(offset, scaledOffset, cameraSpeed*Time.deltaTime); 
    }

	public Vector3 getMovementDir () { 
		Vector3 updatedPosition = transform.position; 
		Vector3 dir = updatedPosition - oldPosition;
		isMoving = (dir.magnitude > 0f);
		return dir;

	}


}
