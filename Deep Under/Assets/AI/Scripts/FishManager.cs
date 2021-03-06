﻿using UnityEngine;
using System.Collections.Generic;

public class FishManager : UnitySingleton<FishManager> {

    public List<Spawn> Spawners = new List<Spawn>();
	public List<BoidsFish> LargeFishList = new List<BoidsFish>();
	public List<BoidsFish> MediumFishList = new List<BoidsFish>();
	public List<BoidsFish> SmallFishList = new List<BoidsFish>();

	public List<BoidsFish> LightEatersList = new List<BoidsFish>();

	public LargeBoidsFish LargeFish;
	public MediumBoidsFish MediumFish;
	public SmallBoidsFish SmallFish;
	public LightEaterBoids LightEaterFish;

    public GameObject EnergyBall;
    public SoftBoundary IsolatedSoftBoundaryPrefab;

    public void RegisterSpawner(Spawn spawner)
    {
        if (this.Spawners.Contains(spawner))
            { return; }

        this.Spawners.Add(spawner);
    }

    public void RegisterFish(BoidsFish fish)
    {
        if (fish.Size == BoidsFish.SIZE.SMALL)
            { this.SmallFishList.Add(fish); }

        else if (fish.Size == BoidsFish.SIZE.MEDIUM)
            { this.MediumFishList.Add(fish); }

		else if (fish.Size == BoidsFish.SIZE.GOD)
            { this.LightEatersList.Add(fish); }
		else
		{ this.LargeFishList.Add(fish); }

    }

	public void DestroyFish(BoidsFish fishToDestroy)
	{
		foreach (BoidsFish aFish in LargeFishList)
		{
			aFish.RemoveFishReferences (fishToDestroy);
		}
		foreach (BoidsFish aFish in MediumFishList)
		{
			aFish.RemoveFishReferences (fishToDestroy);
		}
		foreach (BoidsFish aFish in SmallFishList)
		{
			aFish.RemoveFishReferences (fishToDestroy);
		}
		foreach (BoidsFish aFish in LightEatersList)
		{
			aFish.RemoveFishReferences (fishToDestroy);
		}
		LargeFishList.Remove (fishToDestroy);
		MediumFishList.Remove (fishToDestroy);
		SmallFishList.Remove (fishToDestroy);
		LightEatersList.Remove (fishToDestroy);
		Destroy (fishToDestroy.gameObject);
	}

    public void DisableSpawners()
    {
        foreach (Spawn spawner in this.Spawners)
            { spawner.Spawning = false; }
    }

    public void EnableSpawners()
    {
        foreach (Spawn spawner in this.Spawners)
            { spawner.Spawning = true; }
    }

    public void Reset()
    {
        this.DisableSpawners();
        foreach (BoidsFish aFish in LargeFishList)
		{
			Destroy(aFish.gameObject);
		}
		foreach (BoidsFish aFish in MediumFishList)
		{
			Destroy(aFish.gameObject);
		}
		foreach (BoidsFish aFish in SmallFishList)
		{
			Destroy(aFish.gameObject);
		}
		foreach (BoidsFish aFish in LightEatersList)
		{
			Destroy(aFish.gameObject);
		}
        this.LargeFishList.Clear();
        this.MediumFishList.Clear();
        this.SmallFishList.Clear();
        this.LightEatersList.Clear();
    }

	// Helper methods
	protected Vector3 GetRandomPos() {
		float radius = 50.0f;				// depends on the size of terrain?
		Vector3 randPos = Random.insideUnitSphere * radius;
		randPos.y += 50.0f;
		return randPos;
	}

	public Vector3 FishCount() {
		return new Vector3(this.LargeFishList.Count, this.MediumFishList.Count, this.SmallFishList.Count);
	}
}
