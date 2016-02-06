using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class BoidsFish : MonoBehaviour
{
    public enum SIZE { SMALL, MEDIUM, LARGE }
    public enum STATE { IDLE, SWIMMING, FLEEING, HUNTING }
    
    [SerializeField] private Rigidbody RigidBody;
    [SerializeField] private SphereCollider RepelVolume;
    [SerializeField] private SphereCollider FlockVolume;
    
    private float RepelRadius;
    private float FlockRadius;
    
    void Awake()
    {
        RepelRadius = this.transform.localScale.magnitude * this.RepelVolume.radius;
        FlockRadius = this.transform.localScale.magnitude * this.FlockVolume.radius;
    }
    
    [SerializeField] private SIZE size;
    public SIZE Size
    {
        get             { return this.size; }
        protected set   { this.size = value; }
    }
    public STATE State;
    
    private List<BoidsFish> Flock = new List<BoidsFish>();
    private List<BoidsFish> Repellants = new List<BoidsFish>();
    
    // A physical target will always take precedence over a standard target
    private bool WasFollowingPhysicalTarget = false;
    [HideInInspector] public FishTarget PhysicalTarget;
    private Vector3 Target = Vector3.zero;
    
    
    /// <summary> This message is called by the child FlockVolume gameobject </summary>
    void AddPeer(BoidsFish peer)
    {
        this.Flock.Add(peer);
    }
    
    /// <summary> This message is called by the child FlockVolume gameobject </summary>
    void RemovePeer(BoidsFish peer)
    {
        this.Flock.Remove(peer);
    }
    
    /// <summary> This message is called by the child AdjacentsVolume gameobject </summary>
    void AddRepellant(BoidsFish repellant)
    {
        this.Repellants.Add(repellant);
    }
    
    /// <summary> This message is called by the child AdjacentsVolume gameobject </summary>
    void RemoveRepellant(BoidsFish repellant)
    {
        this.Repellants.Remove(repellant);
    }
    
    /// <summary> This gets called whenever this fish stops following a target </summary>
    protected virtual void StopFollowingTarget()
    {
        this.WasFollowingPhysicalTarget = false;
        // TODO
            // Maybe get a new random point to swim towards?
            // Or just switch to idle?
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
        
        cohesion.Normalize();
        if (distance < this.FlockRadius/4)
            { cohesion = this.transform.forward*100; }
        // We want to attract farther fish less than closer fish
        cohesion *= ((this.FlockRadius - distance) * BoidsSettings.Instance.Cohesion / 1000 * distance) + this.FlockRadius*1.2f;
        
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

            repulsion.Normalize();
            // We want to repel fish that are close faster than fish that are far
            repulsion *= (this.RepelRadius - distance) * BoidsSettings.Instance.Separation / distance;
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
        
        return ((alignment - this.RigidBody.velocity) * BoidsSettings.Instance.Alignment) / 2;
    }
    
    private Vector3 VectorTowardsTarget()
    {
        // Follow a physical target if it exists
        if (this.PhysicalTarget != null)
        {
            // If this fish wasn't following a target before, mark it as following a target now
            if (!this.WasFollowingPhysicalTarget)
                { this.WasFollowingPhysicalTarget = true; }
            return ((this.PhysicalTarget.transform.position - this.transform.position) * BoidsSettings.Instance.Target) / 10;
        }
        // Otherwise, follow a standard target
        else
        {
            // If this fish just stopped following a target call the method
            if (this.WasFollowingPhysicalTarget)
            {
                this.StopFollowingTarget();
            }
            return this.Target;
        }
    }
    
    void FixedUpdate()
    {
        // Handle rigidbody velocity updates
        Vector3 cohesion = (this.State != STATE.FLEEING) ? this.VectorTowardsFlock() : -this.VectorTowardsFlock();
        Vector3 separation = this.VectorAwayFromNeighbours();
        Vector3 alignment = this.VectorTowardsAlignment();
        Vector3 target = (this.State != STATE.IDLE) ? this.VectorTowardsTarget() : Vector3.zero;
        
        // (this.transform.right * Random.Range(-1f, 1f)) + (this.transform.up * Random.Range(-1f, 1f))
        
        Vector3 updatedVelocity = this.transform.forward * 5;                                       // Fish is always moving a minimum speed
        updatedVelocity += cohesion + separation + alignment + target;                              // Glue all the stages together
        updatedVelocity = Vector3.Slerp(this.RigidBody.velocity, updatedVelocity, 2*Time.fixedDeltaTime);
        updatedVelocity = Vector3.ClampMagnitude(updatedVelocity, BoidsSettings.Instance.FishSpeed);    // Limit the speed of the fish to a maximum
        this.RigidBody.velocity = updatedVelocity;
        
        // Add a bob (thx Ali)
        this.RigidBody.MovePosition(transform.position + (Vector3.up * (Mathf.Sin(Time.time * 2f)) * 0.005f));
        
        // Steer the fish's transform to face the velocity vector
        Quaternion dirQ = Quaternion.LookRotation(updatedVelocity);
        Quaternion slerp = Quaternion.Slerp (transform.rotation, dirQ, updatedVelocity.magnitude * 2 * Time.fixedDeltaTime);
        this.RigidBody.MoveRotation(slerp);
    }
}
