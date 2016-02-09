using UnityEngine;
using System.Collections;

public class circling : MonoBehaviour {

	private Vector3 originalPosition;
	private float angle;

	// Use this for initialization
	void Start ()
	{
		originalPosition = this.transform.position;
		angle = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		angle++;
		angle = angle > 360 ? angle - 360 : angle;
		this.transform.position = originalPosition + new Vector3 (Mathf.Cos (Mathf.Deg2Rad*angle),0 , Mathf.Sin (Mathf.Deg2Rad*angle));
	}
}
