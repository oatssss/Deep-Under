using UnityEngine;
using System.Collections;

public class pod : MonoBehaviour {

	private float rotSpeed = 40f; 
	public float orbStrength = 20f; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		rotate(); 
	}

	private void rotate () { 
		transform.Rotate(new Vector3 (0f, rotSpeed * Time.deltaTime, 0f));
		transform.Rotate(new Vector3 (rotSpeed * Time.deltaTime, 0f, 0f));
	}

	void OnTriggerStay(Collider other) { 
		if (other.CompareTag("Auliv")) { 
			Player auliv = other.GetComponent<Player>();
			auliv.charging = true; 
			auliv.addEnergy(this.orbStrength);
		}
	}

	void OnTriggerExit (Collider other) { 
		if (other.CompareTag("Auliv")) { 
			Player auliv = other.GetComponent<Player>();
			auliv.charging = false; 
			//Debug.Log ("Auliv has left the building");
		}
	}
}
