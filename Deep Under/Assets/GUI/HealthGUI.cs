using UnityEngine;
using System.Collections;

public class HealthGUI : MonoBehaviour {
	public float bar; 
	public Vector2 pos = new Vector2(20,40);
	public Vector2 size = new Vector2(200,20);
	public Texture2D empty;
	private Texture2D currentT;
	Color redColor = Color.red;
	Color greenColor = Color.green;
	Color yellowColor = Color.yellow;

	public Player auliv;

	void Start(){
		currentT = new Texture2D(1,1);
		auliv = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}
	
	void OnGUI() {
	
		GUI.contentColor = (auliv.energy < 20f)? Color.red : Color.white;
		if (auliv.energy < 20f){
			currentT.SetPixel(1, 1, redColor);
		} else if (auliv.energy < 60f) {
			currentT.SetPixel(1, 1, yellowColor);
		} else {
			currentT.SetPixel(1, 1, greenColor);
		}

		GUI.Label(new Rect(pos.x+size.x+5f, pos.y, 100f, 20f), (int)auliv.energy+"%");
		//draw the bar background:
		GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
		GUI.Box(new Rect(0,0, size.x, size.y), empty);

		//draw the filled-in part:
		GUI.BeginGroup(new Rect(0,0, size.x * bar, size.y));
		GUI.Box(new Rect(0,0, size.x, size.y), currentT);
		GUI.EndGroup();
		GUI.EndGroup();
	}
	
	void Update() {
		bar = auliv.energy*0.01f;
	}
}