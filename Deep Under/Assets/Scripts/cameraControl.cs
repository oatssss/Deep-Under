using UnityEngine;
using System.Collections;

public class cameraControl : MonoBehaviour {

	public float moveSpeed;
	public float rotateSpeed;
	public Transform firstPersonCamera;
	private Vector3 mousePosition;

	// Use this for initialization
	void Start () {
		Random.seed = (int)System.DateTime.Now.Ticks;
		mousePosition = Input.mousePosition;
	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown("space"))
		{
			firstPersonCamera.GetComponent<Camera>().depth *= -1;
			return;
		}

		Vector3 keyMove = new Vector3 (0, 0, 0);
		if (Input.GetKey ("w")) 
		{
			keyMove.z = 1;
		}
		else if (Input.GetKey("s"))
		{
			keyMove.z = -1;
		}
		if (Input.GetKey ("a")) 
		{
			keyMove.x = -1;
		}
		else if (Input.GetKey("d"))
		{
			keyMove.x = 1;
		}
		if (Input.GetKey("q"))
		{
			keyMove.y = 1;
		}
		else if (Input.GetKey("e"))
		{
			keyMove.y = -1;
		}
		Vector3 mouseMove = Input.mousePosition - mousePosition;

		transform.localPosition += transform.rotation * keyMove * moveSpeed;

		transform.localRotation = Quaternion.AngleAxis (rotateSpeed * mouseMove.x, Vector3.up)*Quaternion.AngleAxis (rotateSpeed * mouseMove.y, Vector3.left);
	}
}
