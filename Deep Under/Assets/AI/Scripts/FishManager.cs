using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishManager : MonoBehaviour {

	public List<Transform> LargeFishList;
	public List<Transform> MediumFishList;
	public List<Transform> SmallFishList;

	public int numberOfLFish = 2;
	public int numberOfMFish = 15;
	public int numberOfSFish = 35;

	public GameObject Manager;
	public GameObject LargeFish;
	public GameObject MediumFish;
	public GameObject SmallFish;
	private GameObject _fish;

	// Use this for initialization
	void Start () {
		Manager = GameObject.Find("FishManager");
		for (int i = 0; i < numberOfLFish; i++) {
			_fish = (GameObject) Instantiate(LargeFish, GetRandomPos(), transform.rotation); 
			LargeFishList.Add(_fish.transform);
			_fish.transform.parent = Manager.transform;
		}
		for (int j = 0; j < numberOfMFish; j++) {
			_fish = (GameObject) Instantiate(MediumFish, GetRandomPos(), transform.rotation); 
			MediumFishList.Add(_fish.transform);
			_fish.transform.parent = Manager.transform;
		}
		for (int k = 0; k < numberOfSFish; k++) {
			_fish = (GameObject) Instantiate(SmallFish, GetRandomPos(), transform.rotation); 
			SmallFishList.Add(_fish.transform);
			_fish.transform.parent = Manager.transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Helper methods
	protected Vector3 GetRandomPos() {
		float radius = 50.0f;				// depends on the size of terrain?
		Vector3 randPos = Random.insideUnitSphere * radius;
		randPos.y += 50.0f;
		return randPos;   
	}
}
