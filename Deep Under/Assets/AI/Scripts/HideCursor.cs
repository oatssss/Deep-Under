using UnityEngine;

public class HideCursor : MonoBehaviour {

	// Use this for initialization
	void Start () {
	   Cursor.visible = false;
       Cursor.lockState = CursorLockMode.Locked;
	}
}
