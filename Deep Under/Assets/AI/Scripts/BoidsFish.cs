using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class BoidsFish : MonoBehaviour
{
	public enum SIZE { SMALL, MEDIUM, LARGE, GOD }
	public enum STATE { IDLE, SWIMMING, FLEEING, HUNTING, EATING, EATEN }
	public Light fLight;
    public Material medLight;

	[SerializeField] public Rigidbody RigidBody;
	[SerializeField] private SphereCollider RepelVolume;
	public AudioSource audioSource;
	private float soundTimer = 0f;
	public AudioClip eatSound;
	public AudioClip mediumFishSound;

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

	[SerializeField] private List<BoidsFish> Repellants = new List<BoidsFish>();
	[SerializeField] private List<BoidsFish> Predators = new List<BoidsFish>();
	[SerializeField] private List<BoidsFish> LightEaters = new List<BoidsFish>();
    public int PredatorCount { get { return this.Predators.Count; } }
    /*[HideInInspector]*/ public List<BoidsFish> Predatees = new List<BoidsFish>();

	// A physical target will always take precedence over a standard target
	public bool IsFollowingTarget = false;
	[SerializeField] private MonoBehaviour physicalTarget;
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
	[SerializeField] private Vector3 Target = Vector3.zero;

	// If fish are outside the bounding volumes, they try to move back inside them
	private List<HardBoundary> HardBoundaries = new List<HardBoundary>();
	private HardBoundary LastHardBoundary;
    protected bool IsOutsideHardBounds = false;
    private List<SoftBoundaryComponent> SoftBoundaryComponents = new List<SoftBoundaryComponent>();
    [SerializeField] private SoftBoundaryComponent LastSoftBoundaryComponent;
    private SoftBoundary CurrentExternalSoftBoundary;
    [SerializeField] private SoftBoundary currentSoftBoundary;
    private SoftBoundary CurrentSoftBoundary
    {
        get { return this.currentSoftBoundary; }
        set
        {
            if (this.IsolatedSoftBoundary != null)
            {
                Destroy(this.IsolatedSoftBoundary.gameObject);
                this.IsolatedSoftBoundary = null;
            }
            this.currentSoftBoundary = value;
        }
    }

    private SoftBoundary IsolatedSoftBoundary;
    [SerializeField] protected bool IsOutsideSoftBounds = true;

	protected bool GodBeingRepelled = false;

	[SerializeField] public Animator Animator;
	public BoidsFish BeingEaten;

    protected virtual void Awake()
    {
		this.State = STATE.SWIMMING;
		this.StateTimer = 0f;
		this.HungerSpan = Random.Range(10f,50f);
    }

	// Make sure that this fish belongs to the Fish layer
	protected virtual void Start()
	{
        // Each boids fish is responsible for registering itself to the fish manager
        FishManager.Instance.RegisterFish(this);
		InvokeRepeating("RandomizeDestination",5.0f,3.0f);
		if (this.size == SIZE.MEDIUM) audioSource = GetComponent<AudioSource>();
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
        if (this.CurrentExternalSoftBoundary == boundaryComponent.SoftBoundary)
            { this.CurrentExternalSoftBoundary = null; }

        this.SoftBoundaryComponents.Remove(boundaryComponent);
        if (this.SoftBoundaryComponents.Count <= 0)
        {
            this.IsOutsideSoftBounds = true;
        }
    }

    public void InsideSoftBounds(SoftBoundaryComponent boundaryComponent)
    {
        this.CurrentExternalSoftBoundary = boundaryComponent.SoftBoundary;

        // For fish that don't have a current soft boundary assigned, do nothing
        if (this.CurrentSoftBoundary == null)
            { return; }

        if (this.CurrentSoftBoundary.Contains(boundaryComponent))
        {
            this.CurrentExternalSoftBoundary = null;
            this.SoftBoundaryComponents.Add(boundaryComponent);
            this.LastSoftBoundaryComponent = boundaryComponent;

            if (this.IsOutsideSoftBounds)
            {
                this.IsOutsideSoftBounds = false;
            }
        }
    }

    public void SetSoftBoundary(SoftBoundary boundary)
    {
        this.CurrentSoftBoundary = boundary;

		this.LastSoftBoundaryComponent = boundary ? boundary.GetFirstComponent() : null;

        this.IsOutsideSoftBounds = true;
    }

    protected virtual float IsolatedSoftBoundaryRadius { get { return 10f; } }
    private void CreateIsolatedSoftBoundary()
    {
        SoftBoundary isolatedSoftBoundary = Instantiate<SoftBoundary>(FishManager.Instance.IsolatedSoftBoundaryPrefab);
        this.SetSoftBoundary(isolatedSoftBoundary);
        this.IsolatedSoftBoundary = isolatedSoftBoundary;
        isolatedSoftBoundary.transform.position = this.transform.position;
        isolatedSoftBoundary.GetComponent<SphereCollider>().radius = this.IsolatedSoftBoundaryRadius;
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
		if (this.Size == SIZE.MEDIUM)
		{
            medLight.SetColor("_EmissionColor", Color.red);
            fLight.color = Color.red;
            
        }
	}

	public void Idle()
	{
		this.State = STATE.IDLE;
		StopFollowingTarget ();
		if (this.Size == SIZE.MEDIUM)
		{
            medLight.SetColor("_EmissionColor", Color.yellow);
            fLight.color = Color.yellow;
            
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
        this.physicalTarget = null;

        // Make a new soft bound if while following the target, this fish moved out of all soft boundary volumes, otherwise take the soft boundary it's currently in
        if (this.IsOutsideSoftBounds)
        {
            if (this.CurrentExternalSoftBoundary != null)
            {
                this.CurrentSoftBoundary = this.CurrentExternalSoftBoundary;
            }
            else
            {
                //this.CreateIsolatedSoftBoundary();
            }
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


		FishTarget orbTarget = this.PhysicalTarget as FishTarget;
		if (orbTarget)
		{
			// light orb presents, ignore all else
			return Vector3.zero;
		}

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

        if (this.IsOutsideSoftBounds && !this.IsFollowingTarget)
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
		updatedVelocity = Vector3.Slerp(this.RigidBody.velocity, updatedVelocity, 3*Time.fixedDeltaTime);
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
		this.soundTimer += Time.fixedDeltaTime;
		makeSoundRandom();

		// Apply changes to animator controller
		if (this.Animator != null)
		{
			this.Animator.SetFloat("Horizontal", this.transform.InverseTransformDirection(updatedVelocity).x);
//			this.Animator.SetFloat("Speed Monitor", updatedVelocity.magnitude);
//			this.Animator.speed = Mathf.Clamp(updatedVelocity.magnitude, 1, 5);
		}
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
			// State = STATE.IDLE; 	// Idle fish does not want to flock and is just swimming away
			// StopFollowingTarget();		// Stop flocking
            this.Idle();
		}
		else if (State == STATE.IDLE)
		{
			// Resume swimming and joining crowd
			if  (StateTimer > 10.0f)
				State = STATE.SWIMMING;
		}
		else if (State == STATE.HUNTING)
		{
			float distance = CalculateDistance (this.PhysicalTarget);
			if (distance < 1.0)	// if target is caught
			{
				State = STATE.EATING;
			}
			else if (distance > 300 || this.PhysicalTarget == null)			// Target is too far, give up can be changed later
			{
				this.Idle();
			}
		}
		else if (State == STATE.FLEEING)
		{
			if (this.Predators.Count == 0)
			{
				this.Idle();
			}
		}
		else if (State == STATE.EATING)
		{
			if (StateTimer > 2f)
			{
				this.Idle();
			}
		}
        // Else this fish is going to be eaten and is about to be destroyed
    }
	protected EnergyBall checkForEnergy()
	{
		List<EnergyBall> balls = OrbManager.Instance.EnergyList;

		if (balls.Count == 0)
			return null;

		balls.Sort(delegate(EnergyBall a, EnergyBall b)
		          {return Vector3.Distance(this.transform.position,a.transform.position)
			.CompareTo(
				Vector3.Distance(this.transform.position,b.transform.position) );
		});
		return balls[0];
	}

	protected bool checkIfVisible(BoidsFish target)
	{
		// if target is too close, it is always visible
		if (Vector3.Distance (target.transform.position, transform.position) < 20)
			return true;

		// if target is out of sight
		if (Vector3.Angle (transform.forward, target.transform.position - transform.position) > BoidsSettings.Instance.MedFishVisualAngle)
			return false;

		// if target is obscured
		RaycastHit raycastHit;
		int layerMask = 1 << 8 | 1 << 9 | 1 << 14 | 1 << 15 | 1 << 16 | 1 << 17 | 1 << 18 | 1 << 20;
		layerMask = ~layerMask;
		Physics.Raycast (transform.position, target.transform.position - transform.position, out raycastHit, Mathf.Infinity, layerMask);
		if (raycastHit.collider != target.GetComponent<Collider> ())
		{
			return false;
		}
		return true;
	}

    protected void AnalyzePrey()
    {
        // when eating, dont try to hunt anyone
		if (this.State == STATE.EATING)
			return;

		if (size == SIZE.GOD)
		{
			// God fish has special logic, it will sense energies and chase them no matter how far away
			EnergyBall eBall = checkForEnergy();
			if (eBall)
			{
				this.PhysicalTarget = eBall;
				Hunt ();
			}
			else
			{
				Idle ();
			}

			return;
		}

		FishTarget orbTarget = this.PhysicalTarget as FishTarget;
		if (orbTarget)
		{
			// light orb presents, ignore all else
			return;
		}

        BoidsFish predatee = this.PhysicalTarget as BoidsFish;

		// if the target is out of sight, stop hunting
		if (predatee && !checkIfVisible (predatee))
			predatee = null;

        if (predatee != null)
        {
            foreach (BoidsFish potentialSwitch in this.Predatees)
            {
				// ignore if not visible
				if (!checkIfVisible (potentialSwitch))
					continue;

				if (BoidsSettings.Instance.AulivTheBestPrey)
				{
					// Don't switch if the predatee is A.U.L.I.V.
					if (predatee == GameManager.Instance.Player) {
						break;
					}

					// Switch to A.U.L.I.V. if within prey
					if (potentialSwitch == GameManager.Instance.Player) {
						predatee = potentialSwitch;
						break;
					}
				}

                // Skip if potential switch is smaller in size or the same fish as already being hunted
                if (potentialSwitch == predatee || potentialSwitch.Size < predatee.Size)
                    { continue; }

                if (this.Size >= SIZE.LARGE && this.Size != SIZE.GOD)
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
				// ignore if not visible
				if (!checkIfVisible (fish))
					continue;

				if (BoidsSettings.Instance.AulivTheBestPrey)
				{
					// Set A.U.L.I.V. as prey immediately if present
					if (fish == GameManager.Instance.Player)
					{
						closestFish = fish;
						break;
					}
				}

                // Large fish skip small fish that aren't in a big enough flock
				if (this.Size >= SIZE.LARGE && this.Size != SIZE.GOD)
                {
                    SmallBoidsFish small = fish as SmallBoidsFish;
                    if (fish == GameManager.Instance.Player && (small != null) && (small.FlockSize < BoidsSettings.Instance.MinFlockSizeToAttractLargeFish))
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
                this.Hunt();
			}
			else
			{
				this.PhysicalTarget = null;
				this.Idle ();
			}
        }

        // Only medium fish are scared of approaching flocks
        if (this.Size == SIZE.MEDIUM)
        {
            SmallBoidsFish smallPredatee = predatee as SmallBoidsFish;
            if (smallPredatee != null)
            {
                if (smallPredatee.FlockSize > BoidsSettings.Instance.MinFlockSizeToScareMediumFish)
                {
                    this.PhysicalTarget = predatee = null;
                    this.Idle();
                }
            }
        }

        // if (this.Size != SIZE.SMALL)
        // {
        //     if (predatee != null)
        //         this.Hunt ();
        //     else
        //     {
        //         this.Idle ();
        //     }
        // }
    }

	protected bool hasRealDanger()
	{
		if (this.PredatorCount < 0)
			return false;
		foreach (BoidsFish predator in Predators)
		{
			if (predator.PhysicalTarget == this)
				return true;
		}
		return false;
	}

	public float CalculateDistance(MonoBehaviour obj)
	{
		BoidsFish targetFish = obj as BoidsFish;
		if (targetFish != null)
			return Vector3.Distance(targetFish.transform.position, transform.position);
		else
			return 200;
	}

	protected virtual void RandomizeDirection(){

		Vector3 random = new Vector3(Random.Range(-80.0f,80.0f), Random.Range (-80.0f,80.0f),0.0f);
		Vector3.Normalize(random);
		this.RigidBody.AddForce(random);
	}

    // USE A COROUTINE INSTEAD OF INVOKEREPEATING
	public void RandomizeDestination(){
        if (this.State != STATE.IDLE || this.IsOutsideHardBounds || this.IsOutsideSoftBounds)
        {
            return;
        }
		float radius = 20.0f;				// depends on the size of terrain?
		Vector3 random = Random.insideUnitSphere * radius;
		random = new Vector3(random.x, random.y, random.z);
		random += transform.position;
		random.y = Mathf.Abs (random.y);
		SetTarget(random);
	}

	void OnCollisionEnter(Collision collision)
    {
		BoidsFish collidedFish = collision.gameObject.GetComponent<BoidsFish> ();

		if (collision.gameObject.tag=="Fish" && collidedFish.Size < this.Size
		    && this.Size != SIZE.GOD && collidedFish.Size != SIZE.GOD)
		{
			// check if contact point is near the mouth
			if (Vector3.Angle (transform.forward, collision.contacts[0].point - transform.position) < 60)
			{
				// collided with prey, eat it
				this.State = STATE.EATING;
				collidedFish.Eaten(this);
			}
		}
		else if (collision.gameObject.tag=="Player" && collidedFish.Size < this.Size && collidedFish.Size != SIZE.GOD && this.Size != SIZE.GOD)
		{
			// check if contact point is near the mouth
			if (Vector3.Angle (transform.forward, collision.contacts [0].point - transform.position) < 60)
			{
				// collided with Auliv, kill?
				Player auliv = collision.gameObject.GetComponent<Player> ();
				auliv.Eaten (this);
			}
		}
    }

    public virtual void Eaten(BoidsFish eater)
    {
		eater.BeingEaten = this;
		eater.Animator.SetTrigger("Eat");
		if (eater.audioSource == null) eater.audioSource = GetComponent<AudioSource>(); 
		eater.audioSource.clip = eatSound; 
		eater.audioSource.pitch = 1.0f; 
		eater.audioSource.Play(); 
		eater.audioSource.clip = mediumFishSound;
		eater.audioSource.Play();
        /*GameObject energyBall = (GameObject) */
    }

	public virtual void RemoveFishReferences(BoidsFish referencedFish)
	{
		if (this.PhysicalTarget == referencedFish)
		{
			this.Idle ();
		}
		Repellants.RemoveAll(fish => fish == referencedFish);
		Predators.RemoveAll(fish => fish == referencedFish);
		Predatees.RemoveAll(fish => fish == referencedFish);
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

   	public void makeSoundRandom () {
    	if (soundTimer > 5f) {
    		if (this.size == SIZE.MEDIUM) {
    			float r = Random.Range(1f, 10f);
    			//if (r > 5f) {
    				//Debug.Log("Play medium Fish Sound");
					audioSource.clip = mediumFishSound;
					audioSource.Play();
    			//}
    			soundTimer = 0f;
    		}
    		else if (this.size == SIZE.LARGE) {

    		}

    		else if (this.size == SIZE.SMALL) {

    		}
			soundTimer = 0f; //reset soundTimer regardless
    	}
    }
}

