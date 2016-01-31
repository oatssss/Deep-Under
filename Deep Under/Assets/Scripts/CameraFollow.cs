using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	Vector3 offset; 
	public Player player;  
	private float cameraSpeed = 60f;
	private float smoothing = 15f; 
	
	void Start () {
		//dplayer = GetComponent<Player>();
		offset = transform.position - player.transform.position;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false; 
	}

	void FixedUpdate () {

		Vector3 oldRot = transform.rotation * Vector3.forward; //get current/old rotation in vector form
		transform.Rotate(Vector3.up * Input.GetAxis ("Mouse X") * cameraSpeed * Time.deltaTime); //rotate camera on Y Axis according to MouseX
		transform.Rotate(Vector2.right * Input.GetAxis ("Mouse Y") * cameraSpeed * Time.deltaTime); //rotate camera on X axis according to MouseY
		transform.rotation = Quaternion.Euler(new Vector3 (transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0f));
		Vector3 newRot = transform.rotation * Vector3.forward; // save new rotation in vector form 
		Quaternion change = Quaternion.FromToRotation(oldRot, newRot); //record change in rotation using the 2 vectors
		offset = change * offset; //apply change in rotation to offset
		Vector3 targetPos = player.transform.position + offset; // apply offset
		transform.position = Vector3.Lerp(transform.position, targetPos, smoothing * Time.deltaTime);


	}

		

}
