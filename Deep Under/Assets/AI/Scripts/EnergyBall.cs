using UnityEngine;
using System.Collections;

public class EnergyBall : MonoBehaviour {

	public float energy;
	public float ghostvalue;
    [SerializeField] private Material Normal;
    [SerializeField] private Material Sucking;
    [SerializeField] private Renderer Renderer;

//	private float timer;

	void Awake () {
		OrbManager.Instance.addEnergy(this);
	}

	void OnCollisionEnter(Collision other) {
		if (other.gameObject.CompareTag("Player") && GameManager.Instance.Player != null) {
//			energy = Random.Range(15f, 25f);
			//ghostvalue = Random.Range(10f, 15f);
			GameManager.Instance.Player.addEnergyBall(this.energy);
			GameManager.Instance.Player.addGhost();
			OrbManager.Instance.destroyEnergy(this);
		}
		else if (other.gameObject.CompareTag("LightEater"))
		{
			OrbManager.Instance.destroyEnergy(this);
		}
	}

    public void SetSucking(bool sucking)
    {
        this.Renderer.material = sucking ? this.Sucking : this.Normal;
    }
}
