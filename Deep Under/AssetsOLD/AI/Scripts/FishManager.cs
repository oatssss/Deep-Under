using UnityEngine;
using System.Collections.Generic;

public class FishManager : UnitySingleton<FishManager> {

	public List<BoidsFish> LargeFishList = new List<BoidsFish>();
	public List<BoidsFish> MediumFishList = new List<BoidsFish>();
	public List<BoidsFish> SmallFishList = new List<BoidsFish>();

	[Range(0,5f)] public int numberOfLFish = 2;
	[Range(0,25f)] public int numberOfMFish = 15;
	[Range(0,300f)] public int numberOfSFish = 35;

	public GameObject LargeFish;
	public GameObject MediumFish;
	public GameObject SmallFish;
    public GameObject EnergyBall;
	
	// Use this for initialization
	void Start () {
        BoidsFish _fish;

		for (int i = 0; i < numberOfLFish; i++) {
			_fish = ((GameObject) Instantiate(LargeFish, GetRandomPos(), transform.rotation)).GetComponent<BoidsFish>();
			_fish.transform.parent = this.transform;
		}
		for (int j = 0; j < numberOfMFish; j++) {
			_fish = ((GameObject) Instantiate(MediumFish, GetRandomPos(), transform.rotation)).GetComponent<BoidsFish>();
			_fish.transform.parent = this.transform;
		}
		for (int k = 0; k < numberOfSFish; k++) {
			_fish = ((GameObject) Instantiate(SmallFish, GetRandomPos(), transform.rotation)).GetComponent<BoidsFish>();
			_fish.transform.parent = this.transform;
		}
	}
	

    public void RegisterFish(BoidsFish fish)
    {
        if (fish.Size == BoidsFish.SIZE.SMALL)
            { this.SmallFishList.Add(fish); }

        else if (fish.Size == BoidsFish.SIZE.MEDIUM)
            { this.MediumFishList.Add(fish); }

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
		LargeFishList.Remove (fishToDestroy);
		MediumFishList.Remove (fishToDestroy);
		SmallFishList.Remove (fishToDestroy);
		Destroy (fishToDestroy.gameObject);
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
