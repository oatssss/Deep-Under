using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthGUI : MonoBehaviour {
	public float ebar;
	public float gbar;
	public Player auliv;

    [SerializeField] private Image EnergyBar;
    [SerializeField] private Image GhostBar;

	[SerializeField] private Color ebarColor;
	[SerializeField] private Color gbarColor;

	private bool blink;

	void Awake() {
		InvokeRepeating("Switch", 0.0f, 0.2f);
	}

	void Start(){
		auliv = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		GhostBar.color = gbarColor;
		blink = false;
	}

	void OnGUI() {

        EnergyBar.transform.localScale = new Vector3(ebar, 1, 1);
		GhostBar.transform.localScale = new Vector3(gbar, 1, 1);

        if (ebar < 0.3 && blink)
        {
            EnergyBar.color = new Color(0.5f,0f,0f,0.5f);
        }
		else
		{
			EnergyBar.color = ebarColor;
		}
	}

	void Update() {
		ebar = auliv.energy*0.01f;
		gbar = auliv.ghostbar*0.01f;
				
	}
	void Switch() {
		blink = !blink;
	}
}