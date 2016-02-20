﻿using UnityEngine;
using Extensions;

public class MediumBoidsFish : BoidsFish {

    private float IdleMin = 4f;
    private float IdleMax = 8f;
    private float AbsoluteMax = 14f;

    public override STATE State
	{
		get				{ return this.state; }
		protected set
		{
			if (this.state != value)
				StateTimer = 0;

			this.state = value;
			if (value == STATE.EATING)
                { this.MinSpeed = this.MaxSpeed = this.IdleMin; }
			else if (value == STATE.FLEEING)
				{ this.MinSpeed = this.MaxSpeed = this.AbsoluteMax; }
			else if (value == STATE.IDLE || value == STATE.SWIMMING)
            {
                this.state = STATE.IDLE;
                this.MinSpeed = this.IdleMin;
                this.MaxSpeed = this.IdleMax;
            }
			/*else if (value == STATE.SWIMMING)
            {
                this.MinSpeed = this.SwimMin;
                this.MaxSpeed = this.SwimMax;
            }*/
			else if (value == STATE.HUNTING)
				{ this.MinSpeed = this.MaxSpeed = this.AbsoluteMax; }
		}
	}

	protected override void Start()
    {
        base.Start();
        this.EnforceLayerMembership("Medium Fish");
        this.Size = SIZE.MEDIUM;
	}

    protected override Vector3 CalculateVelocity()
	{
		// Handle rigidbody velocity updates
		Vector3 separation = this.VectorAwayFromNeighbours();
		Vector3 target = this.VectorTowardsTarget();
        Vector3 avoid = this.VectorAwayFromPredators();

		// Glue all the stages together
		Vector3 updatedVelocity = this.transform.forward * BoidsSettings.Instance.MediumFish_IdleMin;     // Fish is always moving a minimum speed
        updatedVelocity += target;
		updatedVelocity += separation;
        updatedVelocity += avoid;
		updatedVelocity *= BoidsSettings.Instance.FishSpeedMultiplier;
		updatedVelocity = Vector3.Slerp(this.RigidBody.velocity, updatedVelocity, 2*Time.fixedDeltaTime);

		return updatedVelocity;
	}

    protected override void AnalyzePrey()
    {
        // when eating, dont try to hunt anyone
		if (this.State == STATE.EATING)
			return;

        BoidsFish predatee = this.PhysicalTarget as BoidsFish;
        if (predatee != null)
        {
            foreach (BoidsFish potentialSwitch in this.Predatees)
            {
                if (potentialSwitch == predatee || potentialSwitch.Size < predatee.Size)
                    { continue; }

                float sqrDistToCurrent = (this.transform.position - predatee.transform.position).sqrMagnitude;
                float sqrDistToPotential = (this.transform.position - potentialSwitch.transform.position).sqrMagnitude;
                if (sqrDistToPotential < sqrDistToCurrent)
                    { predatee = potentialSwitch; }
            }

            this.PhysicalTarget = predatee;
        }
        else
        {
            float closestSqrDist = float.PositiveInfinity;
            BoidsFish closestFish = null;
            foreach (BoidsFish fish in this.Predatees)
            {
                float sqrDistToFish = (this.transform.position - fish.transform.position).sqrMagnitude;
                if (sqrDistToFish < closestSqrDist)
                {
                    closestSqrDist = sqrDistToFish;
                    closestFish = fish;
                }
            }

            if (closestFish != null)
            {
                this.PhysicalTarget = closestFish;
                predatee = closestFish;
            }
        }

        // Only medium fish are scared of approaching flocks
        if (this.Size == SIZE.MEDIUM)
        {
            SmallBoidsFish smallPredatee = predatee as SmallBoidsFish;
            if (smallPredatee != null)
            {
                if (smallPredatee.FlockSize > BoidsSettings.Instance.MinFlockSizeToScareMediumFish)
                    { this.PhysicalTarget = null; }
            }
        }

        if (predatee != null)
			this.Hunt ();
		else
		{
			this.Idle ();
		}
    }

#if UNITY_EDITOR
    protected override void FixedUpdate()
    {
        this.State = this.State;
        this.IdleMin = BoidsSettings.Instance.MediumFish_IdleMin;
        this.IdleMax = BoidsSettings.Instance.MediumFish_IdleMax;
        // this.SwimMin = BoidsSettings.Instance.MediumFish_SwimMin;
        // this.SwimMax = BoidsSettings.Instance.MediumFish_SwimMax;
        this.AbsoluteMax = BoidsSettings.Instance.MediumFish_AbsoluteMax;
        base.FixedUpdate();
    }
#endif

}
