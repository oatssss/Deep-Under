using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawn : MonoBehaviour {
	
	public float spawnRate;
	public float spawnQuantity;
	public float spawnChance;
	public GameObject creature;
	public GameObject fishManager;
	public int typeOfFish;
	public int maxFish;
	
	FishManager fManager;
	
	// Use this for initialization
	void Start () {
		
		InvokeRepeating("spawnCreature", spawnRate, spawnRate);
		fManager = fishManager.GetComponent<FishManager>();
	}
	
	void spawnCreature()
	{
		if (Random.value < spawnChance)
		{
			if (typeOfFish == 1 && fManager.SmallFishList.Count < maxFish)
			{
				for (int i = 0; i < spawnQuantity; i++)
				{
					Debug.Log("Spawning small fish");
					Instantiate(creature, this.transform.position + new Vector3(0, 4, 0), Quaternion.Euler(0.0f, (float)Random.Range(0, 360), 0.0f));
				}
			}
			
			else if (typeOfFish == 2 && fManager.MediumFishList.Count < maxFish)
			{
				for (int i = 0; i < spawnQuantity; i++)
				{
					Debug.Log("Spawning medium fish");
					Instantiate(creature, this.transform.position + new Vector3(0, 4, 0), Quaternion.Euler(0.0f, (float)Random.Range(0, 360), 0.0f));
				}
			}
			
			else if (typeOfFish == 3 && fManager.LargeFishList.Count < maxFish)
			{
				for (int i = 0; i < spawnQuantity; i++)
				{
					Debug.Log("Spawning large fish");
					Instantiate(creature, this.transform.position + new Vector3(0, 4, 0), Quaternion.Euler(0.0f, (float)Random.Range(0, 360), 0.0f));
				}
			}
			
			else { }
		}
	}
}