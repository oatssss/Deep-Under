using UnityEngine;
using System.Collections.Generic;
using Extensions;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class BoidsFish : MonoBehaviour
{
	public enum SIZE { SMALL, MEDIUM, LARGE }
	public enum STATE { IDLE, SWIMMING, FLEEING, HUNTING, EATING, EATEN }

	[SerializeField] public Rigidbody RigidBody;
	[SerializeField] private SphereCollider RepelVolume;
    [SerializeField] private SphereCollider PredatorVolume;

	public float RepelRadius { get { return this.transform.localScale.magnitude * this.RepelVolume.radius; } }

	private float StateTimer;
	private float HungerSpan;

    protected float MinSpeed;
	protected float MaxSpeed;

	public float Speed { get; protected set; }

    [SerializeField] private SIZE size;
	public SIZE Size
	{
		get             { return this.size; }
		protected set   { this.size = value; }
	}

    [SerializeField] protected STATE state;
	public abstract STATE State { get; protected set; }

	private List<BoidsFish> Repellants = new List<BoidsFish>();
	private List<BoidsFish> Predators = new List<BoidsFish>();

	// A physical target will always take precedence over a standard target
	protected bool IsFollowingTarget = false;
	private MonoBehaviour physicalTarget;
	public MonoBehaviour PhysicalTarget
	{
		get { return this.physicalTarget; }
		set
		{
			this.physicalTarget = value;
			if (value == null)
			{ this.StopFollowingTarget(); }
			else
			{ this.IsFollowingTarget = true; }
		}
	}

	// standard Target
	private Vector3 Target = Vector3.zero;

	// If fish are outside the bounding volumes, they try to move back inside them
	[SerializeField] private List<BoidsBoundary> BoundaryVolumes = new List<BoidsBoundary>();
	[SerializeField] private BoidsBoundary LastBoundary;
	[SerializeField] protected bool IsOutOfBounds = false;

	// Make sure that this fish belongs to the Fish layer
	void Start()
	{
		// Make sure that this fish belongs to the Fish layer
		// this.EnforceLayerMembership("Fish");
		State = STATE.SWIMMING;
		this.StateTimer = 0f;
		this.HungerSpan = Random.Range(10f,50f);
		//		Debug.Log("This fish hunger span is : " + hungerSpan);
	}

	public void OutsideBounds(BoidsBoundary boundary)
	{
		this.BoundaryVolumes.Remove(boundary);
		if (this.BoundaryVolumes.Count <= 0)
		{
			this.IsOutOfBounds = true;
		}
	}

	public void InsideBounds(BoidsBoundary boundary)
	{
		this.BoundaryVolumes.Add(boundary);
        this.LastBoundary = boundary;

		// If this fish just returned from being out of bounds
		if (this.IsOutOfBounds)
		{
			// TODO : Direct fishies more inward, otherwise they stay along the boundary edge

			this.IsOutOfBounds = false;
		}
	}

	/// <summary> Called by the child RepelVolume gameobject </summary>
	public void AddRepellant(BoidsFish repellant)
	{
		this.Repellants.Add(repellant);
	}

	/// <summary> Called by the child RepelVolume gameobject </summary>
	public void RemoveRepellant(BoidsFish repellant)
	{
		this.Repellants.Remove(repellant);
	}

    /// <summary> Called by the child PredatorVolume gameobject </summary>
	public void AddPredator(BoidsFish predator)
	{
		this.Predators.Add(predator);
	}

	/// <summary> Called by the child PredatorVolume gameobject </summary>
	public void RemovePredator(BoidsFish predator)
	{
		this.Predators.Remove(predator);
	}

	/// <summary> This gets called whenever this fish stops following a target </summary>
	protected virtual void StopFollowingTarget()
	{
		this.IsFollowingTarget = false;
		// TODO
		// Maybe get a new random point to swim towards?
		// Or just switch to idle?
	}

	protected void SetTarget(Vector3 target)
	{
		this.Target = target;
		this.IsFollowingTarget = true;
	}

	protected virtual Vector3 VectorAwayFromNeighbours()
	{
		if (this.Repellants.Count <= 0)
            { return Vector3.zero; }

		// Get a velocity vector that will move this fish away from close neighbours by averaging repellant vectors
		Vector3 separation = Vector3.zero;
		foreach (BoidsFish repellant in this.Repellants)
		{
			// Get a vector going away from this repellant
			Vector3 repulsion = this.transform.position - repellant.transform.position;
			float distance = repulsion.magnitude;

			// We want to repel fish that are close faster than fish that are far
			repulsion *= ((this.RepelRadius - distance + (this.RepelRadius/2)) * BoidsSettings.Instance.Separation) / distance;
			separation += repulsion;
		}
		separation /= this.Repellants.Count;

		return separation;
	}

	protected Vector3 VectorTowardsTarget()
	{
		// Move towards the boundary if out of bounds
		if (this.IsOutOfBounds)
            { return (this.LastBoundary.transform.position - this.transform.position) * BoidsSettings.Instance.Target; }

		// Only continue if the fish is following a target
		if (!this.IsFollowingTarget)
            { return Vector3.zero; }

		// Follow a physical target if it exists
		if (this.PhysicalTarget != null)
            { return (this.PhysicalTarget.transform.position - this.transform.position) * BoidsSettings.Instance.Target; }

		// Otherwise, follow a standard target
		else
            { return (this.Target - this.transform.position) * BoidsSettings.Instance.Target; }
	}

    protected Vector3 VectorAwayFromPredators()
    {
        if (this.Predators.Count <= 0)
            { return Vector3.zero; }

        // Get a velocity vector that will move this fish away from nearby predators
		Vector3 avoid = Vector3.zero;
		foreach (BoidsFish predator in this.Predators)
		{
			// Get a vector going away from this predator
			Vector3 away = transform.position - predator.transform.position;
			float distance = away.magnitude;

			// We want to repel fish that are close faster than fish that are far
			away *= ((this.RepelRadius - distance + (this.RepelRadius/2)) * BoidsSettings.Instance.Separation) / distance;
			avoid += away;
		}
		avoid /= this.Predators.Count;

        return Vector3.zero;
    }

	void FixedUpdate()
	{
		Vector3 updatedVelocity = this.CalculateVelocity();
		this.RigidBody.velocity = updatedVelocity;

		// Add a bob (thx Ali)
		this.RigidBody.MovePosition(transform.position + (Vector3.up * (Mathf.Sin(Time.time * 2f)) * 0.005f));

		// Steer the fish's transform to face the velocity vector
		Quaternion dirQ = Quaternion.LookRotation(updatedVelocity);
		Quaternion slerp = Quaternion.Slerp (transform.rotation, dirQ, updatedVelocity.magnitude * 2 * Time.fixedDeltaTime);
		this.RigidBody.MoveRotation(slerp);

		this.StateTimer+= Time.fixedDeltaTime;
	}

    void Update()
    {
        // FSM IMPLEMENTATION
		if (State == STATE.SWIMMING && StateTimer > HungerSpan)
		{
			//TODO: Fish goes hungry and detaches from group
			State = STATE.IDLE; 	// Idle fish does not want to flock and is just swimming away
			StopFollowingTarget();		// Stop flocking
			StateTimer = 0f; 			// reset timer
		}
		else if (State == STATE.IDLE && StateTimer > 10.0f)
		{
			// Resume swimming and joining crowd
			State = STATE.SWIMMING;
			StateTimer = 0f;
		}
		else if (State == STATE.HUNTING)
		{
			float distance = CalculateDistance(this.PhysicalTarget);
			if (distance < 1.0)	// if target is caught
			{
				State = STATE.EATING;
			}
			else if (distance > 100.0)		// Target is too far, give up can be changed later
			{
				State = STATE.IDLE;
			}
		}
		else if (State == STATE.FLEEING)
		{
			if (this.Predators.Count == 0)
			{
				State = STATE.IDLE;
			}
		}
		else if (State == STATE.EATING)
		{
			//TODO: be disabled for like 2 seconds or something
		}
        // Else this fish is going to be eaten and is about to be destroyed
    }

	public float CalculateDistance(MonoBehaviour obj){
		//TODO: calculate distance
		return 0f;
	}

	protected abstract Vector3 CalculateVelocity();
}

