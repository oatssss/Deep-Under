using UnityEngine;
using Extensions;

[RequireComponent(typeof(SphereCollider))]
public class FlockVolume : MonoBehaviour {

    [SerializeField] private BoidsFish ParentFish;
    [SerializeField] private SphereCollider Volume;
    
    void Start()
    {
        InvokeRepeating("UpdateRadius", 0f, 1f);
        
        this.EnforceLayerMembership("Flock Volumes");
    }
	
    /// <summary> For enabling the flock radius to be adaptive. Flock radius will decrease as this fish's flock grows. </summary>
    void UpdateRadius()
    {
        float shrink = (BoidsSettings.Instance.FlockRadius-BoidsSettings.Instance.RepelRadius)/BoidsSettings.Instance.MaxFlockSize*this.ParentFish.FlockSize;
        float newRadius = BoidsSettings.Instance.FlockRadius - shrink;
        newRadius = Mathf.Lerp(this.Volume.radius, newRadius, 0.1f);
        this.Volume.radius = newRadius;
    }

	void OnTriggerEnter(Collider other)
    {
        // Is the triggering object a BoidsFish?
        BoidsFish peer = other.gameObject.GetComponent<BoidsFish>();
        if (peer != null)
        {
            // Is the triggering BoidsFish the same size as this one?
            if (peer.Size == this.ParentFish.Size)
            {
                this.ParentFish.AddPeer(peer);
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        // Is the triggering object a BoidsFish?
        BoidsFish peer = other.gameObject.GetComponent<BoidsFish>();
        if (peer != null)
        {
            // Is the triggering BoidsFish the same size as this one?
            if (peer.Size == this.ParentFish.Size)
            {
                this.ParentFish.RemovePeer(peer);
            }
        }
    }
}
