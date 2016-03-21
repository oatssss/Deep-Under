using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawn : MonoBehaviour {

    [SerializeField] private SoftBoundary AssociatedSoftBoundary;
	[SerializeField] private float spawnRate;
	[SerializeField] private float spawnQuantity;
	[Range(0,1f)] [SerializeField] private float spawnChance;
	[SerializeField] private BoidsFish.SIZE typeOfFish;
	[SerializeField] private int maxFish;

	void Start ()
    {
		InvokeRepeating("spawnCreature", spawnRate, spawnRate);
	}

	void spawnCreature()
	{
        // Spawn based on a probability
		if (Random.value > spawnChance)
		    { return; }

        GameObject spawned = null;

        if (typeOfFish == BoidsFish.SIZE.SMALL && FishManager.Instance.SmallFishList.Count < maxFish)
        {
            for (int i = 0; i < spawnQuantity; i++)
            {
                // Spawn small fish
                spawned = (GameObject)Instantiate(FishManager.Instance.SmallFish.gameObject, this.transform.position + new Vector3(0, 4, 0), Quaternion.Euler(0.0f, (float)Random.Range(0, 360), 0.0f));
                spawned.GetComponent<BoidsFish>().SetSoftBoundary(this.AssociatedSoftBoundary);
                // , this.transform.position + new Vector3(0, 4, 0), Quaternion.Euler(0.0f, (float)Random.Range(0, 360), 0.0f)
            }
        }

        else if (typeOfFish == BoidsFish.SIZE.MEDIUM && FishManager.Instance.MediumFishList.Count < maxFish)
        {
            for (int i = 0; i < spawnQuantity; i++)
            {
                // Spawn medium
                spawned = (GameObject)Instantiate(FishManager.Instance.MediumFish.gameObject, this.transform.position + new Vector3(0, 4, 0), Quaternion.Euler(0.0f, (float)Random.Range(0, 360), 0.0f));
                spawned.GetComponent<BoidsFish>().SetSoftBoundary(this.AssociatedSoftBoundary);
            }
        }

        else if (typeOfFish == BoidsFish.SIZE.LARGE && FishManager.Instance.LargeFishList.Count < maxFish)
        {
            for (int i = 0; i < spawnQuantity; i++)
            {
                // Spawn large
                spawned = (GameObject)Instantiate(FishManager.Instance.LargeFish.gameObject, this.transform.position + new Vector3(0, 4, 0), Quaternion.Euler(0.0f, (float)Random.Range(0, 360), 0.0f));
                spawned.GetComponent<BoidsFish>().SetSoftBoundary(this.AssociatedSoftBoundary);
            }
        }

		else if (typeOfFish == BoidsFish.SIZE.GOD && FishManager.Instance.LightEatersList.Count < maxFish)
		{
			for (int i = 0; i < spawnQuantity; i++)
			{
				// Spawn large
				spawned = (GameObject)Instantiate(FishManager.Instance.LightEaterFish.gameObject, this.transform.position + new Vector3(0, 4, 0), Quaternion.Euler(0.0f, (float)Random.Range(0, 360), 0.0f));
                spawned.GetComponent<BoidsFish>().SetSoftBoundary(this.AssociatedSoftBoundary);
            }
		}

        

        //if (spawned) 
        //{
        //	spawned.transform.position = this.transform.position + new Vector3(0, 4, 0);
        //	spawned.RigidBody.MoveRotation(Quaternion.Euler(0.0f, (float)Random.Range(0, 360), 0.0f));
        //}
    }
}