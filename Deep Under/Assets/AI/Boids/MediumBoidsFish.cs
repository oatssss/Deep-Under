using UnityEngine;
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
		updatedVelocity = Vector3.ClampMagnitude(updatedVelocity, BoidsSettings.Instance.MediumFish_IdleMax);

		return updatedVelocity;
	}

    protected override void FixedUpdate()
    {

#if UNITY_EDITOR
        this.State = this.State;
        this.IdleMin = BoidsSettings.Instance.MediumFish_IdleMin;
        this.IdleMax = BoidsSettings.Instance.MediumFish_IdleMax;
        // this.SwimMin = BoidsSettings.Instance.SmallFish_SwimMin;
        // this.SwimMax = BoidsSettings.Instance.SmallFish_SwimMax;
        this.AbsoluteMax = BoidsSettings.Instance.MediumFish_AbsoluteMax;
#endif

        base.FixedUpdate();
    }
}
