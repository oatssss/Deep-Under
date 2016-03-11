using UnityEngine;
using System.Collections;

public class cameraSense : MonoBehaviour {

	public CameraFollow cF; //a way to control the camera movement 
	private Player player; 
	private Vector3 movementDir; 
	private float rayDistance = 18f;
	private float amount = 1f; 
	private bool scaled = false; 
	RaycastHit hitInfo;
	private bool isHit = false;
	// Use this for initialization
	void Start () {
		player = cF.player;
		Debug.Log (cF.offsetLength);
	}
	
	// becasue physics; raycast in the direction of the camera's movement 

	void FixedUpdate () {
		Vector3 dir = cF.getMovementDir();

		if (dir.magnitude > 0.05f)detect();  //amount updated here. 
	}

	private void detect () { 
		Ray ray = new Ray(transform.position, -1*transform.forward);
		Debug.DrawRay(transform.position, -rayDistance*transform.forward);
		if (Physics.Raycast(ray, out hitInfo, rayDistance)) { 
			if (hitInfo.collider.tag.Equals("Environment")) { 
				//amount = hitInfo.distance/rayDistance;  
				//Debug.Log(amount);
				if (!scaled) { 
					cF.offset = cF.offset * 0.33333f;
					scaled = true;
				}
			}
		}
		else { 
			if (scaled) { 
				cF.offset = cF.offset * 3f;
				scaled = false; 
			}
		}

	}



}
