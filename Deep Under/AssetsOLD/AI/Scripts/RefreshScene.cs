using UnityEngine;
using System.Collections;

public class RefreshScene : MonoBehaviour {

	private GameObject myFishManager;
	public GameObject FishManagerPrefab;
//	FishManager manager;

	// Use this for initialization
	void Start () {
		GameObject myFishManager = (GameObject) Instantiate(FishManagerPrefab, new Vector3(0f,0f,0f), Quaternion.identity);
//		manager = myFishManager.GetComponent<FishManager>();
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
