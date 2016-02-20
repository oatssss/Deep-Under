using UnityEngine;
using System.Collections.Generic;
using Extensions;

public class HuntVolume : MonoBehaviour {

	[SerializeField] private BoidsFish ParentFish;

    [SerializeField] private SphereCollider Volume;

    private List<BoidsFish> Predatees = new List<BoidsFish>();

    void Start()
    {

#if UNITY_EDITOR
        InvokeRepeating("UpdateRadius", 0f, 1f);
#endif

        if (this.ParentFish.Size == BoidsFish.SIZE.MEDIUM)
            { this.EnforceLayerMembership("Medium Hunt Volumes"); }

        else if (this.ParentFish.Size == BoidsFish.SIZE.LARGE)
            { this.EnforceLayerMembership("Large Hunt Volumes"); }
    }

    void UpdateRadius()
    {
        if (this.ParentFish.Size == BoidsFish.SIZE.MEDIUM)
            { this.Volume.radius = BoidsSettings.Instance.MediumHuntRadius; }

        else if (this.ParentFish.Size == BoidsFish.SIZE.LARGE)
            { this.Volume.radius = BoidsSettings.Instance.LargeHuntRadius; }
    }

    void Update()
    {
		// when eating, dont try to hunt anyone
		if (this.ParentFish.State == BoidsFish.STATE.EATING)
			return;

        BoidsFish predatee = this.ParentFish.PhysicalTarget as BoidsFish;
        if (predatee != null)
        {
            foreach (BoidsFish potentialSwitch in this.Predatees)
            {
                if (potentialSwitch == predatee || potentialSwitch.Size < predatee.Size)
                    { continue; }

                float sqrDistToCurrent = (this.ParentFish.transform.position - predatee.transform.position).sqrMagnitude;
                float sqrDistToPotential = (this.ParentFish.transform.position - potentialSwitch.transform.position).sqrMagnitude;
                if (sqrDistToPotential < sqrDistToCurrent)
                    { predatee = potentialSwitch; }
            }

            this.ParentFish.PhysicalTarget = predatee;
        }
        else
        {
            float closestSqrDist = float.PositiveInfinity;
            BoidsFish closestFish = null;
            foreach (BoidsFish fish in this.Predatees)
            {
                float sqrDistToFish = (this.ParentFish.transform.position - fish.transform.position).sqrMagnitude;
                if (sqrDistToFish < closestSqrDist)
                {
                    closestSqrDist = sqrDistToFish;
                    closestFish = fish;
                }
            }

            if (closestFish != null)
            {
                this.ParentFish.PhysicalTarget = closestFish;
                predatee = closestFish;
            }
        }

        // Only medium fish are scared of approaching flocks
        if (this.ParentFish.Size == BoidsFish.SIZE.MEDIUM)
        {
            SmallBoidsFish smallPredatee = predatee as SmallBoidsFish;
            if (smallPredatee != null)
            {
                if (smallPredatee.FlockSize > BoidsSettings.Instance.MinFlockSizeToScareMediumFish)
                    { this.ParentFish.PhysicalTarget = null; }
            }
        }

		if (predatee != null)
			this.ParentFish.Hunt ();
		else
		{
			this.ParentFish.Idle ();
		}
    }

    void OnTriggerEnter(Collider other)
    {
        // Get the triggering BoidsFish
        BoidsFish predatee = other.gameObject.GetComponent<BoidsFish>();
        if (predatee != null)
        {
            // Is the triggering BoidsFish lower on the food chain?
            if (predatee.Size < this.ParentFish.Size)
            {
                // predatee.AddPredator(this.ParentFish);
                this.Predatees.Add(predatee);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        BoidsFish predatee = other.gameObject.GetComponent<BoidsFish>();
        if (predatee != null)
        {
            if (predatee.Size < this.ParentFish.Size)
            {
                // predatee.RemovePredator(this.ParentFish);
                this.Predatees.Remove(predatee);

                if (predatee.PredatorCount <= 0)
                {
                    predatee.Relax();
                }
            }
        }
    }

	public void willDestroyFish(BoidsFish fishToDestroy)
	{
		Predatees.Remove(fishToDestroy);
	}

	public bool isFishPredatees(BoidsFish fish)
	{
		return Predatees.Contains (fish);
	}
}
