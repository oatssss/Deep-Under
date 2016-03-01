using UnityEngine;
using System.Collections;

public class FishManagerGUI: MonoBehaviour { 
	public Vector2 pos = new Vector2(20,80);
	public FishManager fm;

	private Vector3 count;

	void Start(){
	}
	
	void OnGUI() {
		GUI.Label(new Rect(pos.x, pos.y, 100f, 100f), 
		          	"Large: "+count.x+"\n"+
		          	"Medium: "+count.y+"\n"+
		          	"Small: "+count.z
		     	 );
	}
	
	void Update() {
		if (fm == null)
			fm = GameObject.Find ("FishManager(Clone)").GetComponent<FishManager>();

		count = fm.FishCount();
	}
}