using UnityEngine;
using System.Collections;

public class lightOrb : MonoBehaviour {

	private float timer = 0f; 
	public Rigidbody rb; 

	void Awake() {
		rb = GetComponent<Rigidbody>();
		OrbManager.Instance.addOrb(this);
	}
	
	// Update is called once per frame
	void Update () {
		if (rb != null) { 
			timer += Time.deltaTime; 
			//if (timer > 10f) Destroy(this.gameObject);
			if (timer > 1f) GameObject.Destroy(rb);
		}
		else bob();
	}

	private void bob () {
		transform.position = transform.position + (Vector3.up * (Mathf.Sin(Time.time * 3f)) * 0.01f);
	}

}