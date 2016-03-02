using UnityEngine;
using System.Collections;

// just delegate the trigger event to the main script

public class triggerEventDelegate : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		this.gameObject.GetComponentInParent<Fish>().OnTriggerEnterEx(other, this.gameObject.GetComponent<Collider>());
	}
}
