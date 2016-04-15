using UnityEngine;
using System.Collections;

public class cameraSense : MonoBehaviour {

	public CameraFollow cF; //a way to control the camera movement 
	public Player player; 
	private Vector3 movementDir; 
	private float rayDistance;
	private float amount = 1f; 
	private bool scaled = false; 
	RaycastHit hitInfo;

	// Use this for initialization
	void Start () {
		rayDistance = (transform.position - player.transform.position).magnitude * 1.25f;

	}
	
	// becasue physics; raycast in the direction of the camera's movement 

	void FixedUpdate () {
		if (player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		Vector3 dir = cF.getMovementDir();
		Vector3 testRay = dir; 
		testRay.Scale(new Vector3(10f, 10f, 10f));
		Debug.DrawRay(player.transform.position, (transform.position - player.transform.position) + testRay);
		detect(dir);   
	}

	private void detect (Vector3 dir) {
		
		Ray ray = new Ray(player.transform.position, (transform.position - player.transform.position) + dir);
		if (Physics.Raycast(ray, out hitInfo, rayDistance)) { 
			if (hitInfo.collider.tag.Equals("Environment")) { 
				if (!scaled){
					cF.scaleOffsetLength(0.5f);
					scaled = true;
				}
			}
		}
		else {
			if (scaled) { 
				 	cF.scaleOffsetLength(2f);
					scaled = false;
				}
			}
	}



}
