using UnityEngine;
using System.Collections.Generic;
using Extensions;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class SmallBoidsFish : BoidsFish
{
	[SerializeField] private SphereCollider FlockVolume;

	public float FlockRadius  { get { return this.transform.localScale.magnitude * this.FlockVolume.radius; } }

    public override STATE State
	{
		get				{ return this.state; }
		protected set
		{
			this.state = value;
			if (value == STATE.EATING)
				Speed = MinSpeed;
			else if (value == STATE.FLEEING)
				Speed = MaxSpeed;
			else if (value == STATE.IDLE)
				Speed = MinSpeed + (MaxSpeed-MinSpeed)*0.1f;
			else if (value == STATE.SWIMMING)
				Speed = MinSpeed + (MaxSpeed-MinSpeed)*0.5f;
			else if (value == STATE.HUNTING)
				Speed = MaxSpeed;
		}
	}

	private List<BoidsFish> Flock = new List<BoidsFish>();
	public int FlockSize { get { return this.Flock.Count; } }

    void Start()
    {
        this.EnforceLayerMembership("Small Fish");
    }

	/// <summary> This message is called by the child FlockVolume gameobject </summary>
	public void AddPeer(BoidsFish peer)
	{
		if (this.Flock.Count < BoidsSettings.Instance.MaxFlockSize)
		{
			this.Flock.Add(peer);
		}
		/*else
		{
			BoidsFish randomPeer = this.Flock[Random.Range(0, this.FlockSize)];

			this.RemovePeer(randomPeer);
			this.AddPeer(peer);
		}*/
	}

	/// <summary> This message is called by the child FlockVolume gameobject </summary>
	public void RemovePeer(BoidsFish peer)
	{
		this.Flock.Remove(peer);
	}

	private Vector3 VectorTowardsFlock()
	{
		if (this.Flock.Count <= 0)
		{ return Vector3.zero; }

		// Get the position of the center of the flock by averaging positions
		Vector3 centerOfMass = Vector3.zero;
		foreach (BoidsFish peer in this.Flock)
		{
			centerOfMass += peer.transform.position;
		}
		centerOfMass /= this.Flock.Count;

		// Get a vector in the direction of the CoM
		Vector3 cohesion = centerOfMass - this.transform.position;
		float distance = cohesion.magnitude;

		// cohesion.Normalize();
		if (distance < this.FlockRadius/7)
		{ cohesion = this.transform.forward*BoidsSettings.Instance.MaxFishSpeed; }
		// We want to attract farther fish more than closer fish
		cohesion *= BoidsSettings.Instance.Cohesion;
		// cohesion *= (((this.FlockRadius - distance) * BoidsSettings.Instance.Cohesion) / ((this.FlockRadius / 5) + distance));

		return cohesion;
	}

	private Vector3 VectorTowardsAlignment()
	{
		if (this.Flock.Count <= 0)
		{ return Vector3.zero; }

		// Get the general velocity the flock is influenced towards by averaging velocities
		Vector3 alignment = Vector3.zero;
		foreach (BoidsFish peer in this.Flock)
		{
			alignment += peer.RigidBody.velocity;
		}
		alignment /= this.Flock.Count;
		alignment.Normalize();

		return alignment * BoidsSettings.Instance.Alignment * 5;
	}

	protected override Vector3 CalculateVelocity()
	{
		// Handle rigidbody velocity updates
		Vector3 cohesion = (this.State != STATE.FLEEING) ? this.VectorTowardsFlock() : -this.VectorTowardsFlock();
		Vector3 separation = this.VectorAwayFromNeighbours();
		Vector3 alignment = this.VectorTowardsAlignment();
		Vector3 target = this.VectorTowardsTarget();
		float cohesionMagnitude = cohesion.magnitude;

		// Glue all the stages together
		Vector3 updatedVelocity = this.transform.forward * BoidsSettings.Instance.MinFishSpeed;     // Fish is always moving a minimum speed
        if (this.Flock.Count > 0)
        {
            updatedVelocity += Vector3.ClampMagnitude(cohesion + alignment + target, cohesionMagnitude);
        }
        else
        {
            updatedVelocity += Vector3.ClampMagnitude(cohesion + alignment, cohesionMagnitude);
            updatedVelocity += target;
        }
		updatedVelocity += Vector3.ClampMagnitude(cohesion + alignment + target, cohesionMagnitude);
		updatedVelocity += separation;
		updatedVelocity *= BoidsSettings.Instance.FishSpeedMultiplier;
		updatedVelocity = Vector3.Slerp(this.RigidBody.velocity, updatedVelocity, 2*Time.fixedDeltaTime);
		updatedVelocity = Vector3.ClampMagnitude(updatedVelocity, BoidsSettings.Instance.MaxFishSpeed);

		return updatedVelocity;
	}
}
