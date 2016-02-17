using UnityEngine;
using System.Collections.Generic;
using Extensions;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class BoidsFish : MonoBehaviour
{
	public enum SIZE { SMALL, MEDIUM, LARGE }
	public enum STATE { IDLE, SWIMMING, FLEEING, HUNTING, EATING }
	
	[SerializeField] public Rigidbody RigidBody;
	[SerializeField] private SphereCollider RepelVolume;
	
	public float RepelRadius { get { return this.transform.localScale.magnitude * this.RepelVolume.radius; } }
	
	[SerializeField] private SIZE size;
	private float stateTimer;
	private float hungerSpan;

	public float maxSpeed;
	public float minSpeed;

	protected float speed;
	public STATE state;
	
	public SIZE Size
	{
		get             { return this.size; }
		protected set   { this.size = value; }
	}
	public float Speed {
		get 			{ return this.speed; }
		protected set 	{ this.speed = value; }
	}
	
	public STATE State
	{
		get				{ return this.state; }
		protected set   
		{ 
			this.state = value; 
			if (value == STATE.EATING)
				Speed = minSpeed;
			else if (value == STATE.FLEEING)
				Speed = maxSpeed;
			else if (value == STATE.IDLE)
				Speed = minSpeed + (maxSpeed-minSpeed)*0.1f;
			else if (value == STATE.SWIMMING)
				Speed = minSpeed + (maxSpeed-minSpeed)*0.5f;
			else if (value == STATE.HUNTING)
				Speed = maxSpeed;
		}
	}

	private List<BoidsFish> Repellants = new List<BoidsFish>();
	private List<BoidsFish> Predators = new List<BoidsFish>();

	public int PredatorsSize { get { return this.Predators.Count; } }

	// A physical target will always take precedence over a standard target
	private bool IsFollowingTarget = false;
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
	private List<BoidsBoundary> BoundaryVolumes = new List<BoidsBoundary>();
	private BoidsBoundary LastBoundary;
	private bool IsOutOfBounds = false;
	
	// Make sure that this fish belongs to the Fish layer
	void Start()
	{
		// Make sure that this fish belongs to the Fish layer
		this.EnforceLayerMembership("Fish");
		State = STATE.SWIMMING;
		this.stateTimer = 0f;
		this.hungerSpan = Random.Range(10f,50f);
		//		Debug.Log("This fish hunger span is : " + hungerSpan);
	}
	
	public void OutsideBounds(BoidsBoundary boundary)
	{
		this.BoundaryVolumes.Remove(boundary);
		if (this.BoundaryVolumes.Count <= 0)
		{
			this.IsOutOfBounds = true;
			this.LastBoundary = boundary;
		}
	}
	
	public void InsideBounds(BoidsBoundary boundary)
	{
		this.BoundaryVolumes.Add(boundary);
		
		// If this fish just returned from being out of bounds
		if (this.IsOutOfBounds)
		{
			// TODO : Direct fishies more inward, otherwise they stay along the boundary edge
			
			this.IsOutOfBounds = false;
			this.LastBoundary = null;
		}
	}
	
	/// <summary> This message is called by the child RepelVolume gameobject </summary>
	public void AddRepellant(BoidsFish repellant)
	{
		this.Repellants.Add(repellant);
	}
	
	/// <summary> This message is called by the child RepelVolume gameobject </summary>
	public void RemoveRepellant(BoidsFish repellant)
	{
		this.Repellants.Remove(repellant);
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
		
		this.stateTimer+= Time.fixedDeltaTime;
		
		// FSM IMPLEMENTATION
		if (State == STATE.SWIMMING && stateTimer > hungerSpan)
		{
			//TODO: Fish goes hungry and detaches from group
			State = STATE.IDLE; 	// Idle fish does not want to flock and is just swimming away
			StopFollowingTarget();		// Stop flocking
			stateTimer = 0f; 			// reset timer
		}
		else if (State == STATE.IDLE && stateTimer > 10.0f)
		{
			// Resume swimming and joining crowd
			State = STATE.SWIMMING;
			stateTimer = 0f;
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
			if (PredatorsSize == 0)
			{
				State = STATE.IDLE;
			}
		}
		else if (State == STATE.EATING)
		{
			//TODO: be disabled for like 2 seconds or something
		}
	}

	public Vector3 RandomizeDestination(){
		float radius = 20.0f;				// depends on the size of terrain?
		Vector3 random = Random.insideUnitSphere * radius;
		random = new Vector3(random.x, random.y, random.z);
		random += transform.position;
		random.y = Mathf.Abs (random.y);
		return random;
	}
	public float CalculateDistance(MonoBehaviour obj){
		//TODO: calculate distance
		return 0f;
	}

	protected abstract Vector3 CalculateVelocity();
}

