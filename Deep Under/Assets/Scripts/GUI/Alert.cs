using UnityEngine;
using System.Collections;

public class Alert : MonoBehaviour {

	public string displayText;
	private float timer;
	private float maxTime;
	private bool displayingText;

	public Vector2 pos = new Vector2(20,40);
	public Vector2 size = new Vector2(200,20);

	// Use this for initialization
	void Start () {
		displayText = "Start";
		displayingText = true;
		timer = 2.0f;
	}
	
	// Update is called once per frame
	void Update () {

		if (displayingText && timer > 0.0)
		{
			timer -= Time.deltaTime;
		}
		else if (displayingText && timer <= 0.0)
		{
			displayText = "";
			displayingText = false;
		}
	}
	void OnGUI () {
		GUI.Label (new Rect (pos.x+size.x+45f, pos.y, 200, 50), displayText);
	}

	public void Display(string text, float time){
		displayText = text;
		timer = time;
		displayingText = true;
	}
}
