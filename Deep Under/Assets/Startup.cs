using UnityEngine;
using System.Collections;

public class Startup : MonoBehaviour {

	[SerializeField] private Menu StartupMenu;

	void Start()
    {
        GameManager.Instance.DisallowPauseMenu = true;
        GUIManager.Instance.OpenMenu(this.StartupMenu);
    }
}
