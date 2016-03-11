using UnityEngine;
using System.Collections;

public class SceneLight : MonoBehaviour {

	private Light myLight;
	private float maxBrightness;
	private bool lightsOn = true;

	// Use this for initialization
	void Start () {
		maxBrightness = 2.5f;
		myLight = GetComponent<Light>();
		myLight.intensity = maxBrightness;
	}
	
	// Update is called once per frame
	void Update () {
		if (lightsOn)
		{
			if(myLight.intensity < maxBrightness)
			{			
				myLight.intensity = maxBrightness; 
				Debug.Log("lights on");
			}
//			if(myLight.intensity < maxBrightness)
//			{ myLight.intensity += Time.deltaTime; }
		}
		else
		{
			if(myLight.intensity > 0.0f)
			{
				myLight.intensity = 0.0f; 
				Debug.Log("lights off");
			}
//			if(myLight.intensity > 0.0f)
//			{ myLight.intensity -= Time.deltaTime; }
		}
	}

	public void lights(bool toggle){
		lightsOn = toggle;
	}

}
