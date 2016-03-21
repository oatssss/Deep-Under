using UnityEngine;
using System.Collections;

public class EnergyBall : MonoBehaviour {

	public float energy;

//	private float timer;

	void Awake () {
		OrbManager.Instance.addEnergy(this);
	}

//	void Update () {
//		// Energy ball disappears after 10 seconds
//		timer += Time.deltaTime;
//		if (timer > 20f) Destroy(this.gameObject);
//	}

	void OnCollisionEnter(Collision other) {
		if (other.gameObject.CompareTag("Player") && GameManager.Instance.Player != null) {
			energy = Random.Range(15f, 25f);
			GameManager.Instance.Player.addEnergyBall(this.energy);
//			Debug.Log ("Orb picked up! +"+energy);
			Destroy(this.gameObject);
		}
		else if (other.gameObject.CompareTag("LightEater"))
		{
			Destroy(this.gameObject);
		}
	}
}
