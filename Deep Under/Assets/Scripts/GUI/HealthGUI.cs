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
	private bool flashing;

	void Awake() {
		InvokeRepeating("Switch", 0.0f, 0.2f);
	}

	void Start(){
		GhostBar.color = gbarColor;
		blink = false;
		flashing = false;
	}

	void OnGUI() {

        EnergyBar.transform.localScale = new Vector3(ebar, 1, 1);
		GhostBar.transform.localScale = new Vector3(gbar, 1, 1);

        if (ebar < 0.3 && blink || flashing)
        {
            EnergyBar.color = new Color(0.87f,0f,0f,0.5f);
        }
		else
		{
			EnergyBar.color = ebarColor;
		}
	}

	public void flashEnergyBar()
	{
		flashing = true;
		Invoke("flashBack", .2f);
	}

	void flashBack()
	{
		flashing = false;
	}

	void Update() {
		ebar = GameManager.Instance.Player.energy*0.01f;
		gbar = GameManager.Instance.Player.ghostbar*0.01f;
				
	}
	void Switch() {
		blink = !blink;
	}
}