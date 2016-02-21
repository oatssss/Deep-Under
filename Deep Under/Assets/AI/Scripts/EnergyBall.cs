using UnityEngine;
using System.Collections;

public class EnergyBall : MonoBehaviour {

	public Player auliv;
	public float energy;

	private float timer;

	// Use this for initialization
	void Start () {
		auliv = GameObject.Find ("auliv").GetComponent<Player>();
	}

	void Update () {
		// Energy ball disappears after 10 seconds
		timer += Time.deltaTime;
		if (timer > 10f) Destroy(this.gameObject);
	}

	void OnTriggerEnter(Collider other) { 
		if (other.CompareTag("Player") && auliv != null) { 
			energy = Random.Range(15f, 25f);
			auliv.addEnergyBall(this.energy);
			Debug.Log ("Orb picked up! +"+energy);
			Destroy(this.gameObject);
		}
	}
}
