using UnityEngine;
using System.Collections;

public class fogSetup : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Color fogColor = Color.gray;
		fogColor.b += 0.15f;
		fogColor.r -= 0.1f;
		RenderSettings.fogColor = fogColor;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
