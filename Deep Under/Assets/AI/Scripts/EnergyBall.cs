using UnityEngine;
using System.Collections;

public class EnergyBall : MonoBehaviour {

	public float energy;
	public float ghostvalue;

//	private float timer;

	void Awake () {
		OrbManager.Instance.addEnergy(this);
	}
		
	void OnCollisionEnter(Collision other) {
		if (other.gameObject.CompareTag("Player") && GameManager.Instance.Player != null) {
//			energy = Random.Range(15f, 25f);
			ghostvalue = Random.Range(5f, 10f);
//			GameManager.Instance.Player.addEnergyBall(this.energy);
			GameManager.Instance.Player.addGhost(this.ghostvalue);
			OrbManager.Instance.destroyEnergy(this);
		}
		else if (other.gameObject.CompareTag("LightEater"))
		{
			OrbManager.Instance.destroyEnergy(this);
		}
	}
}
