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
	private List<BoidsFish> preys = new List<BoidsFish>();

	public int PredatorsSize { get { return this.Predators.Count; } }

	// Hunting target is always the closest prey
	Vector3 huntingTargetPosition()
	{
		float minDistance = 99999f;
		BoidsFish nearestPrey = null;
		foreach (BoidsFish aPrey in preys)
		{
			float distance = Vector3.Distance (aPrey.transform.position, transform.position);
			if (distance < minDistance)
			{
				minDistance = distance;
				nearestPrey = aPrey;
			}
		}

		return nearestPrey.transform.position;
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

	public void addPrey(BoidsFish prey)
	{
		preys.Add (prey);
		State = STATE.HUNTING;
		stateTimer = 0f;
	}

	public void removePrey(BoidsFish prey)
	{
		preys.Remove (prey);
		if (preys.Count == 0)
		{
			State = STATE.IDLE;
			stateTimer = 0f;
		}
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

		// If hunting, towards prey
		if (State == STATE.HUNTING)
			return huntingTargetPosition() - transform.position;

		// Otherwise, follow a standard target
		else
		{ return (this.Target - this.transform.position) * BoidsSettings.Instance.Target; }
	}

	Vector3 eatingDirection()
	{
		// when it eats it faces towards horizon
		Vector3 forwardDirection = transform.TransformDirection (Vector3.forward);
		forwardDirection.y = 0f;
		return forwardDirection;
	}
	
	void FixedUpdate()
	{
		Vector3 updatedVelocity = this.CalculateVelocity();
		// Add a bob (thx Ali)
		updatedVelocity += Vector3.left * (Mathf.Sin (Time.time * 2f)) * 0.03f;
		// if eaeting, stay still
		if (State == STATE.EATING)
			updatedVelocity = Vector3.Normalize (eatingDirection()) * 0.01f;
		this.RigidBody.velocity = updatedVelocity;

		// Steer the fish's transform to face the velocity vector
		Quaternion dirQ = Quaternion.LookRotation(updatedVelocity);
		Quaternion slerp = Quaternion.Slerp (transform.rotation, dirQ, updatedVelocity.magnitude * 2 * Time.fixedDeltaTime);
		this.RigidBody.MoveRotation(slerp);
		
		this.stateTimer+= Time.fixedDeltaTime;
		
		// FSM IMPLEMENTATION
		if (State == STATE.SWIMMING && stateTimer > hungerSpan)
		{
			//TODO: Fish goes hungry and detaches from group
			State = STATE.IDLE; 		// Idle fish does not want to flock and is just swimming away
			//StopFollowingTarget();	// Stop flocking //calvinz but follow flock is defined in VectorTowardsFlock, not target
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
			float distance = Vector3.Distance(huntingTargetPosition(), transform.position);
			if (distance < 1.0)	// if target is caught 
			{
				State = STATE.EATING;
				stateTimer = 0f;
			}
			else if (distance > 100.0)		// Target is too far, give up can be changed later
			{
				State = STATE.IDLE;
				stateTimer = 0f;
			}
		}
		else if (State == STATE.FLEEING)
		{
			if (PredatorsSize == 0)
			{
				State = STATE.IDLE;
				stateTimer = 0f;
			}
		}
		else if (State == STATE.EATING)
		{
			if (stateTimer > 2f)
			{
				checkOtherPreysAndStateChange ();
			}
		}
	}

	void checkOtherPreysAndStateChange()
	{
		stateTimer = 0f;
		if (preys.Count > 0)
			State = STATE.HUNTING;
		else
			State = STATE.IDLE;
	}

	public Vector3 RandomizeDestination(){
		float radius = 20.0f;				// depends on the size of terrain?
		Vector3 random = Random.insideUnitSphere * radius;
		random = new Vector3(random.x, random.y, random.z);
		random += transform.position;
		random.y = Mathf.Abs (random.y);
		return random;
	}

	void OnCollisionEnter(Collision collision)
    {
		BoidsFish collidedFish = collision.gameObject.GetComponent<BoidsFish> ();
		if (collidedFish != null && preys.Contains (collidedFish)) 
		{
			// collided with prey, eat it
			State = STATE.EATING;
			stateTimer = 0f;
			destroyFish (collidedFish);
		}
    }

	void destroyFish(BoidsFish fishToDestroy)
	{
		FishManager fishManager = GameObject.Find ("FishManager").GetComponent<FishManager> ();
		fishManager.DestroyFish (fishToDestroy);
	}

	public virtual void willDestroyFish(BoidsFish fishToDestroy)
	{
		preys.Remove (fishToDestroy);
		if (State != STATE.EATING)
			checkOtherPreysAndStateChange ();
		Repellants.Remove (fishToDestroy);
		Predators.Remove (fishToDestroy);
		// do something if predators are gone
	}

	protected abstract Vector3 CalculateVelocity();
}

