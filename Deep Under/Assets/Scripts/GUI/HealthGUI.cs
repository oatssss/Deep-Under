using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthGUI : MonoBehaviour {
	public float bar;
	public Vector2 pos = new Vector2(20,40);
	public Vector2 size = new Vector2(200,20);
	public Texture2D empty;
	public Texture2D full;
	public Player auliv;

    [SerializeField] private Image EnergyBar;
    [SerializeField] private Image GhostBar;

	void Start(){
		auliv = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}

	void OnGUI() {

        EnergyBar.transform.localScale = new Vector3(bar, 1, 1);
        GhostBar.transform.localScale = new Vector3(bar, 1, 1);

        if (bar < 0.5)
        {
            EnergyBar.color = new Color(1,0,0,0.5f);
        }

		// GUI.contentColor = (auliv.energy < 20f)? Color.red : Color.white;

		// GUI.Label(new Rect(pos.x+size.x+5f, pos.y, 100f, 20f), (int)auliv.energy+"%");
		// //draw the bar background:
		// GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
		// GUI.Box(new Rect(0,0, size.x, size.y), empty);

		// //draw the filled-in part:
		// GUI.BeginGroup(new Rect(0,0, size.x * bar, size.y));
		// GUI.Box(new Rect(0,0, size.x, size.y), full);
		// GUI.EndGroup();
		// GUI.EndGroup();
	}

	void Update() {
		bar = auliv.energy*0.01f;
	}
}