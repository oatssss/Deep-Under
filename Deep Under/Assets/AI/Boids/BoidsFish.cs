using UnityEngine;
using System.Collections.Generic;
using Extensions;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class BoidsFish : MonoBehaviour
{
    public enum SIZE { SMALL, MEDIUM, LARGE }
    public enum STATE { IDLE, SWIMMING, FLEEING, HUNTING }
    
    [SerializeField] private Rigidbody RigidBody;
    [SerializeField] private SphereCollider RepelVolume;
    [SerializeField] private SphereCollider FlockVolume;
    
    public float RepelRadius { get { return this.transform.localScale.magnitude * this.RepelVolume.radius; } }
    public float FlockRadius  { get { return this.transform.localScale.magnitude * this.FlockVolume.radius; } }
    
    [SerializeField] private SIZE size;
    public SIZE Size
    {
        get             { return this.size; }
        protected set   { this.size = value; }
    }
    public STATE State;
    
    [SerializeField] private List<BoidsFish> Flock = new List<BoidsFish>();
    [SerializeField] private List<BoidsFish> Repellants = new List<BoidsFish>();
    public int FlockSize { get { return this.Flock.Count; } }
    
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
    private Vector3 Target = Vector3.zero;
    
    // If fish are outside the bounding volumes, they try to move back inside them
    private List<BoidsBoundary> BoundaryVolumes = new List<BoidsBoundary>();
    private MonoBehaviour LastPhysicalTarget;
    private bool IsOutOfBounds = false;
    
    // Make sure that this fish belongs to the Fish layer
    void Start()
    {
        this.EnforceLayerMembership("Fish");
    }
    
    public void OutsideBounds(BoidsBoundary boundary)
    {
        this.BoundaryVolumes.Remove(boundary);
        if (this.BoundaryVolumes.Count <= 0)
        {
            this.IsOutOfBounds = true;
            this.LastPhysicalTarget = this.PhysicalTarget;
            this.PhysicalTarget = boundary;
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
            this.PhysicalTarget = this.LastPhysicalTarget;
            this.LastPhysicalTarget = null;
        }
    }
    
    /// <summary> This message is called by the child FlockVolume gameobject </summary>
    public void AddPeer(BoidsFish peer)
    {
        if (this.Flock.Count < BoidsSettings.Instance.MaxFlockSize)
        {
            this.Flock.Add(peer);
        }
        else
        {
            BoidsFish randomPeer = this.Flock[Random.Range(0, this.FlockSize)];
            
            this.RemovePeer(randomPeer);
            this.AddPeer(peer);
        }
    }
    
    /// <summary> This message is called by the child FlockVolume gameobject </summary>
    public void RemovePeer(BoidsFish peer)
    {
        this.Flock.Remove(peer);
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
    
    private void SetTarget(Vector3 target)
    {
        this.Target = target;
        this.IsFollowingTarget = true;
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
    
    private Vector3 VectorAwayFromNeighbours()
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
    
    private Vector3 VectorTowardsTarget()
    {
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
        // Handle rigidbody velocity updates
        Vector3 cohesion = (this.State != STATE.FLEEING) ? this.VectorTowardsFlock() : -this.VectorTowardsFlock();
        // Vector3 separation = this.WasFollowingPhysicalTarget ? this.VectorAwayFromNeighbours() : this.VectorAwayFromNeighbours();
        Vector3 separation = this.VectorAwayFromNeighbours();
        Vector3 alignment = this.VectorTowardsAlignment();
        Vector3 target = (this.State != STATE.IDLE) ? this.VectorTowardsTarget() : Vector3.zero;
        float cohesionMagnitude = cohesion.magnitude;
        
        // Glue all the stages together
        Vector3 updatedVelocity = this.transform.forward * BoidsSettings.Instance.MinFishSpeed;     // Fish is always moving a minimum speed
        updatedVelocity += Vector3.ClampMagnitude(cohesion + alignment + target, cohesionMagnitude);
        updatedVelocity += separation;
        updatedVelocity *= BoidsSettings.Instance.FishSpeedMultiplier;
        updatedVelocity = Vector3.Slerp(this.RigidBody.velocity, updatedVelocity, 2*Time.fixedDeltaTime);
        updatedVelocity = Vector3.ClampMagnitude(updatedVelocity, BoidsSettings.Instance.MaxFishSpeed);    // Limit the speed of the fish to a maximum
        this.RigidBody.velocity = updatedVelocity;
        
        // Add a bob (thx Ali)
        this.RigidBody.MovePosition(transform.position + (Vector3.up * (Mathf.Sin(Time.time * 2f)) * 0.005f));
        
        // Steer the fish's transform to face the velocity vector
        Quaternion dirQ = Quaternion.LookRotation(updatedVelocity);
        Quaternion slerp = Quaternion.Slerp (transform.rotation, dirQ, updatedVelocity.magnitude * 2 * Time.fixedDeltaTime);
        this.RigidBody.MoveRotation(slerp);
    }
}
