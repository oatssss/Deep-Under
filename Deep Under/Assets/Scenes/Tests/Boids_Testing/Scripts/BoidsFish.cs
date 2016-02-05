using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class BoidsFish : MonoBehaviour
{
    public enum SIZE { SMALL, MEDIUM, LARGE }
    
    [SerializeField] private Rigidbody RigidBody;
    [SerializeField] private SphereCollider RepelVolume;
    
    [SerializeField] private SIZE size = SIZE.SMALL;
    public SIZE Size { get { return this.size; } }
    
    private List<BoidsFish> Flock = new List<BoidsFish>();
    private List<BoidsFish> Repellants = new List<BoidsFish>();
    [HideInInspector] public FlockTarget Target;
    
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
    
    /// <summary> This message is called by any FlockTarget detecting this fish </summary>
    void AddFlockTarget(FlockTarget target)
    {
        this.Target = target;
    }
    
    void RemoveFlockTarget()
    {
        this.Target = null;
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
        
        // Return 1% of the vector so the influence isn't too harsh
        return (cohesion * BoidsSettings.Instance.Cohesion) / 6;
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
            float repelRadius = this.RepelVolume.radius;

            repulsion.Normalize();
            // We want to repel fish that are close faster than fish that are far
            repulsion *= (repelRadius - distance)  * BoidsSettings.Instance.Separation / distance;
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
        if (this.Target != null)
            { return ((this.Target.transform.position - this.transform.position) * BoidsSettings.Instance.Target) / 10; }
        else
            { return Vector3.zero; }
    }
    
    void FixedUpdate()
    {
        // Handle rigidbody velocity updates
        Vector3 cohesion = this.VectorTowardsFlock();
        Vector3 separation = this.VectorAwayFromNeighbours();
        Vector3 alignment = this.VectorTowardsAlignment();
        Vector3 target = this.VectorTowardsTarget();
        
        // (this.transform.right * Random.Range(-1f, 1f)) + (this.transform.up * Random.Range(-1f, 1f))
        
        Vector3 updatedVelocity = this.transform.forward * 5;
        updatedVelocity += cohesion + separation + alignment + target;
        this.RigidBody.velocity = updatedVelocity;
        
        // Steer the fish's heading towards the velocity vector
        // if (updatedVelocity == Vector3.zero)
        //     { return; }
        Quaternion dirQ = Quaternion.LookRotation(updatedVelocity);
        Quaternion slerp = Quaternion.Slerp (transform.rotation, dirQ, updatedVelocity.magnitude * 1 * Time.fixedDeltaTime);
        this.RigidBody.MoveRotation(slerp);
    }
}
