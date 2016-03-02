using UnityEngine;
using System.Collections;

public class shifting : MonoBehaviour {

	private Vector3 originalPosition;
	private float angle;

	// Use this for initialization
	void Start ()
	{
		originalPosition = this.transform.localPosition;
		angle = 0;
	}

	// Update is called once per frame
	void Update ()
	{
		angle+=0.05f;
		angle = angle > 360 ? angle - 360 : angle;
		this.transform.localPosition = originalPosition + new Vector3 (Mathf.Cos (Mathf.Deg2Rad*angle), Mathf.Sin (Mathf.Deg2Rad*angle), 0);
	}
}
