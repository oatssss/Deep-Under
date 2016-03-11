using UnityEngine;
using System.Collections;

public class RefreshScene : MonoBehaviour {

	private GameObject myFishManager;
	public GameObject FishManagerPrefab;

	// Use this for initialization
	void Start () {
		Instantiate(FishManagerPrefab, new Vector3(0f,0f,0f), Quaternion.identity);
	}

	// Update is called once per frame
	void Update () {
		Refresh();
	}

	protected void Refresh() {
		if (Input.GetKey(KeyCode.R))
		{
			Destroy(GameObject.Find("FishManager(Clone)").gameObject);
			Instantiate(FishManagerPrefab, new Vector3(0f,0f,0f), Quaternion.identity);
		}
	}
}
