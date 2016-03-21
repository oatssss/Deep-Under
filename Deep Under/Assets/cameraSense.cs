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
		//Debug.Log (cF.offsetLength);
	}
	
	// becasue physics; raycast in the direction of the camera's movement 

	void FixedUpdate () {
		Vector3 dir = cF.getMovementDir();
		Vector3 ray = dir; 
		ray.Scale(new Vector3(10f, 10f, 10f));
		Debug.DrawRay(transform.position, ray);
		detect(dir);   
	}

	private void detect (Vector3 dir) { 
		Ray ray = new Ray(cF.player.transform.position, -1*transform.forward); //make the ray go toward the camera instead of camera.backwards
		if (Physics.Raycast(ray, out hitInfo, rayDistance)) { 
			if (hitInfo.collider.tag.Equals("Environment") && hitInfo.distance > cF.offsetLength) { 
				if (!scaled){
					amount = hitInfo.distance/rayDistance;
					cF.scaleOffsetLength(0.333f);
					scaled = true;
				}
			}

		}
		else {
			if (scaled) { 
					amount = 1f/amount;
				 	cF.scaleOffsetLength(3f);
					scaled = false;
				}
			}
	}



}
