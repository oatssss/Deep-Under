using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class BoidsFish : MonoBehaviour
{
	public enum SIZE { SMALL, MEDIUM, LARGE }
	public enum STATE { IDLE, SWIMMING, FLEEING, HUNTING, EATING, EATEN }

	[SerializeField] public Rigidbody RigidBody;
	[SerializeField] private SphereCollider RepelVolume;

	public float RepelRadius { get { return this.transform.localScale.magnitude * this.RepelVolume.radius; } }
    protected float EvadeRadius
    {
        get
        {
            if (this.Size == SIZE.SMALL)
            {
                return BoidsSettings.Instance.MediumPredatorRadius;
            }
            else
            {
                return BoidsSettings.Instance.LargePredatorRadius;
            }
        }
    }

	protected float StateTimer;
	private float HungerSpan;

    protected float MinSpeed;
	protected float MaxSpeed;

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
    public int PredatorCount { get { return this.Predators.Count; } }
    [HideInInspector] public List<BoidsFish> Predatees = new List<BoidsFish>();

	// A physical target will always take precedence over a standard target
	public bool IsFollowingTarget = false;
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
	private List<HardBoundary> HardBoundaries = new List<HardBoundary>();
	private HardBoundary LastHardBoundary;
    protected bool IsOutsideHardBounds = false;
    private List<SoftBoundaryComponent> SoftBoundaryComponents = new List<SoftBoundaryComponent>();
    private SoftBoundaryComponent LastSoftBoundaryComponent;
    private SoftBoundary CurrentSoftBoundary;
    private SoftBoundary OriginalSoftBoundary;
    protected bool IsOutsideSoftBounds = false;

    protected virtual void Awake()
    {
		this.State = STATE.SWIMMING;
		this.StateTimer = 0f;
		this.HungerSpan = Random.Range(10f,50f);
        this.CurrentSoftBoundary = this.OriginalSoftBoundary;
    }

	// Make sure that this fish belongs to the Fish layer
	protected virtual void Start()
	{
        // Each boids fish is responsible for registering itself to the fish manager
        FishManager.Instance.RegisterFish(this);
	}

	public void OutsideHardBounds(HardBoundary boundary)
	{
		this.HardBoundaries.Remove(boundary);
		if (this.HardBoundaries.Count <= 0)
		{
			this.IsOutsideHardBounds = true;
		}
	}

	public void InsideHardBounds(HardBoundary boundary)
	{
		this.HardBoundaries.Add(boundary);
        this.LastHardBoundary = boundary;

		// If this fish just returned from being out of bounds
		if (this.IsOutsideHardBounds)
		{
			// TODO : Direct fishies more inward, otherwise they stay along the boundary edge

			this.IsOutsideHardBounds = false;
		}
	}

    public void OutsideSoftBounds(SoftBoundaryComponent boundaryComponent)
    {
        this.SoftBoundaryComponents.Remove(boundaryComponent);
        if (this.SoftBoundaryComponents.Count <= 0)
        {
            this.IsOutsideSoftBounds = true;
        }
    }

    public void InsideSoftBounds(SoftBoundaryComponent boundaryComponent)
    {
        // For fish that don't have a current soft boundary assigned, do nothing
        if (this.CurrentSoftBoundary == null)
            { return; }

        if (this.CurrentSoftBoundary.Contains(boundaryComponent))
        {
            this.SoftBoundaryComponents.Add(boundaryComponent);
            this.LastSoftBoundaryComponent = boundaryComponent;

            if (this.IsOutsideSoftBounds)
            {
                this.IsOutsideSoftBounds = false;
            }
        }
    }

    public void Flee()
    {
        this.State = STATE.FLEEING;
    }

    public void Relax()
    {
        this.State = STATE.SWIMMING;
	}

	public void Hunt()
	{
		this.State = STATE.HUNTING;
	}

	public void Idle()
	{
		this.State = STATE.IDLE;
		StopFollowingTarget ();
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

    /// <summary> Called by the predator </summary>
	public void AddPredator(BoidsFish predator)
	{
		this.Predators.Add(predator);
	}

	/// <summary> Called by the predator gameobject </summary>
	public void RemovePredator(BoidsFish predator)
	{
		this.Predators.Remove(predator);
	}

    /// <summary> Called by the hunt volume </summary>
	public void AddPredatee(BoidsFish predatee)
	{
		this.Predatees.Add(predatee);
	}

	/// <summary> Called by the hunt volume </summary>
	public void RemovePredatee(BoidsFish predatee)
	{
		this.Predatees.Remove(predatee);
	}

	/// <summary> This gets called whenever this fish stops following a target </summary>
	protected virtual void StopFollowingTarget()
	{
		this.IsFollowingTarget = false;

        // Make a new soft bound if while following the target, this fish moved out of its current soft boundary
        if (this.IsOutsideSoftBounds)
        {
            // TODO : Move soft bounds, or remove it
        }
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
		// Only continue if the fish is following a target
		if (!this.IsFollowingTarget || this.IsOutsideHardBounds)
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
			away *= ((this.EvadeRadius - distance + (this.EvadeRadius/2)) * BoidsSettings.Instance.Evade) / distance;
			avoid += away;
		}
		avoid /= this.Predators.Count;

        return avoid;
    }

    protected Vector3 VectorWithinBounds()
    {
        if (this.IsOutsideHardBounds)
            { return (this.LastHardBoundary.transform.position - this.transform.position) * BoidsSettings.Instance.Bounds; }

        if (this.IsOutsideSoftBounds)
        {
            if (this.LastSoftBoundaryComponent == null)
                { return Vector3.zero; }

            return (this.LastSoftBoundaryComponent.transform.position - this.transform.position) * BoidsSettings.Instance.Bounds;
        }

        return Vector3.zero;
    }

	protected virtual void FixedUpdate()
	{
		Vector3 updatedVelocity = this.CalculateVelocity();
#if UNITY_EDITOR
        updatedVelocity *= BoidsSettings.Instance.FishSpeedMultiplier;
#endif
		updatedVelocity = Vector3.Slerp(this.RigidBody.velocity, updatedVelocity, 2*Time.fixedDeltaTime);
		//calvinz: Add bob to the velocity, so the fish turns naturally, also bobs on x axis is more realistic
		updatedVelocity += Vector3.left * (Mathf.Sin (Time.time * 2f)) * 0.03f;
#if UNITY_EDITOR
        updatedVelocity = Vector3.ClampMagnitude(updatedVelocity, BoidsSettings.Instance.FishSpeedMultiplier * this.MaxSpeed);
#else
		updatedVelocity = Vector3.ClampMagnitude(updatedVelocity, this.MaxSpeed);
#endif

		//calvinz: if eating, stay (almost) still
		if (State == STATE.EATING)
			updatedVelocity *= 0.5f;
		this.RigidBody.velocity = updatedVelocity;

		// Steer the fish's transform to face the velocity vector
		Quaternion dirQ = Quaternion.LookRotation(updatedVelocity);
		Quaternion slerp = Quaternion.Slerp (transform.rotation, dirQ, updatedVelocity.magnitude * 2 * Time.fixedDeltaTime);
		this.RigidBody.MoveRotation(slerp);

		this.StateTimer += Time.fixedDeltaTime;
	}

    protected virtual void Update()
    {

#if UNITY_EDITOR
        if (BoidsSettings.Instance.DrawTargetRays)
        {
            // Draw a line to predators
            foreach (BoidsFish predator in this.Predators)
            {
                Color drawColor = (this.State == STATE.FLEEING) ? Color.red : Color.yellow;
                Debug.DrawLine(this.transform.position, predator.transform.position, drawColor, 0f, true);
            }

            // Draw a line to the target
            Vector3 offset = new Vector3(0.2f, 0.2f, 0.2f);
            if (this.PhysicalTarget != null)
            {
                Debug.DrawLine(this.transform.position+offset, this.PhysicalTarget.transform.position+offset, Color.green, 0f, false);
            }
        }
#endif

        // Check the conditions of the surrounding prey, should the target switch?
        this.AnalyzePrey();

        // FSM IMPLEMENTATION
		if (State == STATE.SWIMMING && StateTimer > HungerSpan)
		{
			//TODO: Fish goes hungry and detaches from group
			State = STATE.IDLE; 	// Idle fish does not want to flock and is just swimming away
			StopFollowingTarget();		// Stop flocking
		}
		else if (State == STATE.IDLE && StateTimer > 10.0f)
		{
			// Resume swimming and joining crowd
			State = STATE.SWIMMING;
		}
		else if (State == STATE.HUNTING)
		{
			float distance = CalculateDistance (this.PhysicalTarget);
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
			if (StateTimer > 2f)
			{
				State = STATE.IDLE;
			}
		}
        // Else this fish is going to be eaten and is about to be destroyed
    }

    protected void AnalyzePrey()
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

                if (this.Size >= SIZE.LARGE)
                {
                    // Skip small fish that aren't in a big enough flock
                    SmallBoidsFish potentialSmall = potentialSwitch as SmallBoidsFish;
                    SmallBoidsFish predateeSmall = predatee as SmallBoidsFish;
                    if ((predateeSmall != null && potentialSmall != null) && (potentialSmall.FlockSize < BoidsSettings.Instance.MinFlockSizeToAttractLargeFish))
                    {
                        continue;
                    }
                }

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
                // Large fish skip small fish that aren't in a big enough flock
                if (this.Size >= SIZE.LARGE)
                {
                    SmallBoidsFish small = fish as SmallBoidsFish;
                    if ((small != null) && (small.FlockSize < BoidsSettings.Instance.MinFlockSizeToAttractLargeFish))
                        { continue; }
                }

                float sqrDistToFish = (this.transform.position - fish.transform.position).sqrMagnitude;
                if (sqrDistToFish < closestSqrDist)
                {
                    closestSqrDist = sqrDistToFish;
                    closestFish = fish;
                }
            }

            if (closestFish != null)
            {
                this.PhysicalTarget = predatee = closestFish;
            }
        }

        // Only medium fish are scared of approaching flocks
        if (this.Size == SIZE.MEDIUM)
        {
            SmallBoidsFish smallPredatee = predatee as SmallBoidsFish;
            if (smallPredatee != null)
            {
                if (smallPredatee.FlockSize > BoidsSettings.Instance.MinFlockSizeToScareMediumFish)
                    { this.PhysicalTarget = predatee = null; }
            }
        }

        if (this.Size != SIZE.SMALL)
        {
            if (predatee != null)
                this.Hunt ();
            else
            {
                this.Idle ();
            }
        }
    }

	public float CalculateDistance(MonoBehaviour obj)
	{
		BoidsFish targetFish = obj as BoidsFish;
		if (targetFish != null)
			return Vector3.Distance(targetFish.transform.position, transform.position);
		else
			return 0;
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
		if (collision.gameObject.tag=="Fish" && collidedFish.Size < this.Size)
		{
            // collided with prey, eat it
            this.State = STATE.EATING;
            collidedFish.Eaten(this);
		}
    }

    private void Eaten(BoidsFish eater)
    {
        GameObject energyBall = (GameObject) Instantiate(FishManager.Instance.EnergyBall, this.transform.position, FishManager.Instance.EnergyBall.transform.rotation);
        FishManager.Instance.DestroyFish(this);
    }

	public virtual void RemoveFishReferences(BoidsFish referencedFish)
	{
		Repellants.Remove(referencedFish);
		Predators.Remove(referencedFish);
        Predatees.Remove(referencedFish);
		// do something if predators are gone
	}

	protected virtual Vector3 CalculateVelocity()
    {
        // Handle rigidbody velocity updates
        Vector3 minimum = this.transform.forward * this.MinSpeed;     // Fish is always moving a minimum speed
		Vector3 separation = this.VectorAwayFromNeighbours();
		Vector3 target = this.VectorTowardsTarget();
        Vector3 bounds = this.VectorWithinBounds();
        Vector3 avoid = this.VectorAwayFromPredators();

		return minimum + target + bounds + separation + avoid;
    }
}

