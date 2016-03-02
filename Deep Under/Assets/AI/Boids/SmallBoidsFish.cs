using UnityEngine;
using System.Collections.Generic;
using Extensions;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class SmallBoidsFish : BoidsFish
{
	[SerializeField] private SphereCollider FlockVolume;

	public float FlockRadius  { get { return this.transform.localScale.magnitude * this.FlockVolume.radius; } }

    private float IdleMin = 2f;
    private float IdleMax = 6f;
    private float SwimMin = 4f;
    private float SwimMax = 8f;
    private float AbsoluteMax = 10f;

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
			else if (value == STATE.IDLE)
            {
                this.MinSpeed = this.IdleMin;
                this.MaxSpeed = this.IdleMax;
            }
			else if (value == STATE.SWIMMING)
            {
                this.MinSpeed = this.SwimMin;
                this.MaxSpeed = this.SwimMax;
            }
			else if (value == STATE.HUNTING)
				{ this.MinSpeed = this.MaxSpeed = this.AbsoluteMax; }
		}
	}

	[SerializeField] private List<BoidsFish> Flock = new List<BoidsFish>();
	public int FlockSize { get { return this.Flock.Count + 1; } }  // +1 since including itself

    protected override void Awake()
    {
        base.Awake();
        this.EnforceLayerMembership("Small Fish");
        this.Size = SIZE.SMALL;
    }

	/// <summary> This message is called by the child FlockVolume gameobject </summary>
	public void AddPeer(BoidsFish peer)
	{
		if (this.Flock.Count < BoidsSettings.Instance.MaxFlockSize)
		{
			this.Flock.Add(peer);
		}
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
            { cohesion = this.transform.forward*BoidsSettings.Instance.SmallFish_IdleMax; }
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
        cohesion = (this.PredatorCount > 0) ? 2*cohesion : cohesion; // When predators are near, flock closer to try and scare
		Vector3 separation = this.VectorAwayFromNeighbours();
		Vector3 alignment = this.VectorTowardsAlignment();
		Vector3 target = this.VectorTowardsTarget();
        Vector3 bounds = this.VectorWithinBounds();
        Vector3 avoid = this.VectorAwayFromPredators();
		float cohesionMagnitude = cohesion.magnitude;

		// Glue all the stages together
		Vector3 updatedVelocity = this.transform.forward * this.MinSpeed;     // Fish is always moving a minimum speed
        if (this.Flock.Count > 0)   // Since we clamp using the magnitude of cohesion, only do it for non-zero values
        {
            updatedVelocity += Vector3.ClampMagnitude(cohesion + alignment + target + bounds, cohesionMagnitude);
        }
        else
        {
            updatedVelocity += Vector3.ClampMagnitude(cohesion + alignment, cohesionMagnitude);
            updatedVelocity += target;
            updatedVelocity += bounds;
        }
		updatedVelocity += separation;
        updatedVelocity += avoid;

		return updatedVelocity;
	}

#if UNITY_EDITOR
    protected override void Update()
    {
        // this.State = this.State;
        this.IdleMin = BoidsSettings.Instance.SmallFish_IdleMin;
        this.IdleMax = BoidsSettings.Instance.SmallFish_IdleMax;
        this.SwimMin = BoidsSettings.Instance.SmallFish_SwimMin;
        this.SwimMax = BoidsSettings.Instance.SmallFish_SwimMax;
        this.AbsoluteMax = BoidsSettings.Instance.SmallFish_AbsoluteMax;
        base.Update();
    }
#endif

	public override void RemoveFishReferences(BoidsFish referencedFish)
	{
		base.RemoveFishReferences(referencedFish);
		Flock.Remove(referencedFish);
	}
}
