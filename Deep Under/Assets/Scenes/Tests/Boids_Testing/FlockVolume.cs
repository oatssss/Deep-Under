using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class FlockVolume : MonoBehaviour {

    [SerializeField] private BoidsFish ParentFish;
    [SerializeField] private SphereCollider Volume;
    
    void Update()
    {
        this.Volume.radius = BoidsSettings.Instance.FlockRadius;
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
                this.ParentFish.SendMessage("AddPeer", peer, SendMessageOptions.RequireReceiver);
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
                this.ParentFish.SendMessage("RemovePeer", peer, SendMessageOptions.RequireReceiver);
            }
        }
    }
}
