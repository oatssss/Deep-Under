using UnityEngine;
using System.Collections;

public class MedLight : MonoBehaviour {

	public BoidsFish Parent;
	public Material red;
	public Material yellow;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		if (Parent.State == BoidsFish.STATE.HUNTING || Parent.State == BoidsFish.STATE.EATING)
		{
			this.gameObject.GetComponent<Renderer>().material = red;
		} else 
		{
			this.gameObject.GetComponent<Renderer>().material = yellow;
		}
	}
}
