using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class MediumBoidsFish : BoidsFish
{

	// Use this for initialization
	void Start () {
	
	}

	protected override Vector3 CalculateVelocity()
	{
		// Handle rigidbody velocity updates
		Vector3 separation = this.VectorAwayFromNeighbours();
		Vector3 target = (this.State != STATE.IDLE) ? this.VectorTowardsTarget() : Vector3.zero;
		if (State == STATE.HUNTING)
			target = Vector3.Normalize (target) * maxSpeed;

		// Glue all the stages together
		Vector3 updatedVelocity = this.transform.forward * BoidsSettings.Instance.MinFishSpeed;     // Fish is always moving a minimum speed
		updatedVelocity += target;
		updatedVelocity += separation;
		updatedVelocity *= BoidsSettings.Instance.FishSpeedMultiplier;
		updatedVelocity = Vector3.Slerp(this.RigidBody.velocity, updatedVelocity, 2*Time.fixedDeltaTime);
		updatedVelocity = Vector3.Normalize(updatedVelocity) * Speed;

		return updatedVelocity;
	}

	public override void willDestroyFish(BoidsFish fishToDestroy)
	{
		base.willDestroyFish (fishToDestroy);
	}
}
