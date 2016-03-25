using UnityEngine;
using System.Collections;

public class Startup : MonoBehaviour {

	[SerializeField] private Menu StartupMenu;

    /*
    void Awake()
    {
        GUIManager.Instance.EnergyBar.alpha = 0;
        GUIManager.Instance.GhostBar.alpha = 0;
    }
    */

	void Start()
    {
        GameManager.Instance.DisallowPauseMenu = true;
        GUIManager.Instance.OpenMenu(this.StartupMenu);
        GUIManager.Instance.EnergyBar.alpha = 0;
        GUIManager.Instance.GhostBar.alpha = 0;
    }
}
