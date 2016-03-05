using UnityEngine;
using System.Collections;

public class pod : MonoBehaviour {

	private float rotSpeed = 40f;
	public float orbStrength = 20f;

	Player auliv;
	// Use this for initialization
	void Start () {
//		auliv = GameObject.Find("auliv/SUB_RIG_007").GetComponent<Player>();
		auliv = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
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
		if (other.CompareTag("Player") && auliv != null) {
			auliv.charging = true;
			auliv.addEnergy(this.orbStrength);
			// Debug.Log ("Auliv is hereee");
		}
	}

	void OnTriggerExit (Collider other) {
		if (other.CompareTag("Player") && auliv != null) {
			auliv.charging = false;
			// Debug.Log ("Auliv has left the building");
		}
	}
}
