using UnityEngine;
using System.Collections;

public class cameraSense : MonoBehaviour {

	public CameraFollow cF; //a way to control the camera movement 
	public Player player; 
	private Vector3 movementDir; 
	private float rayDistance = 18f;
	private float amount = 1f; 
	private bool scaled = false; 
	RaycastHit hitInfo;
	private bool isHit = false; 
	// Use this for initialization
	void Start () {
		//Debug.Log (cF.offsetLength);
	}
	
	// becasue physics; raycast in the direction of the camera's movement 

	void FixedUpdate () {
		if (player != null) { 
			Vector3 dir = cF.getMovementDir();
			Vector3 ray = dir; 
			ray.Scale(new Vector3(10f, 10f, 10f));
			detect();   
		}
	}

	private void detect () {
		Vector3 cameraToPlayer = player.transform.position - transform.position;
		Vector3 cameraToFollowThis = cF.player.transform.position - transform.position; 
		float x = Vector3.Angle(transform.forward, cameraToPlayer);
		float y = Vector3.Angle(transform.forward, cameraToFollowThis);
		Ray ray = new Ray(player.transform.position, -1*transform.forward); //make the ray go toward the camera instead of camera.backwards
		if (Physics.Raycast(ray, out hitInfo, rayDistance)) { 
			if (hitInfo.collider.tag.Equals("Environment") && hitInfo.distance > cF.offsetLength) { 
				if (!scaled){
					//amount = hitInfo.distance/rayDistance;
					//transform.Rotate(new Vector3(x, 0f, 0f));
					cF.scaleOffsetLength(0.5f);
					scaled = true;
				}
			}

		}
		else {
			if (scaled) { 
					//amount = 1f/amount;

				 	cF.scaleOffsetLength(2f);
					scaled = false;
				}
			}
	}



}
