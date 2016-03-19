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

        BoidsFish spawned = null;

        if (typeOfFish == BoidsFish.SIZE.SMALL && FishManager.Instance.SmallFishList.Count < maxFish)
        {
            for (int i = 0; i < spawnQuantity; i++)
            {
                // Spawn small fish
                spawned = Instantiate<SmallBoidsFish>(FishManager.Instance.SmallFish);
                // , this.transform.position + new Vector3(0, 4, 0), Quaternion.Euler(0.0f, (float)Random.Range(0, 360), 0.0f)
            }
        }

        else if (typeOfFish == BoidsFish.SIZE.MEDIUM && FishManager.Instance.MediumFishList.Count < maxFish)
        {
            for (int i = 0; i < spawnQuantity; i++)
            {
                // Spawn medium
                spawned = Instantiate<MediumBoidsFish>(FishManager.Instance.MediumFish);
            }
        }

        else if (typeOfFish == BoidsFish.SIZE.LARGE && FishManager.Instance.LargeFishList.Count < maxFish)
        {
            for (int i = 0; i < spawnQuantity; i++)
            {
                // Spawn large
                spawned = Instantiate<LargeBoidsFish>(FishManager.Instance.LargeFish);
            }
        }

        spawned.RigidBody.MovePosition(this.transform.position + new Vector3(0, 4, 0));
        spawned.RigidBody.MoveRotation(Quaternion.Euler(0.0f, (float)Random.Range(0, 360), 0.0f));
        spawned.SetSoftBoundary(this.AssociatedSoftBoundary);
	}
}