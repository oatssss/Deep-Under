using UnityEngine;
using Extensions;

[RequireComponent(typeof(SphereCollider))]
public class RepelVolume : MonoBehaviour {

    [SerializeField] private BoidsFish ParentFish;
    [SerializeField] private SphereCollider Volume;
    

    void Start()
    {
        
#if UNITY_EDITOR
        InvokeRepeating("UpdateRadius", 0f, 1f);
#endif

        this.EnforceLayerMembership("Repel Volumes");
    }
    
    /// <summary> For enabling the flock radius to be adaptive. Flock radius will decrease as this fish's flock grows. </summary>
    void UpdateRadius()
    {
        this.Volume.radius = BoidsSettings.Instance.RepelRadius;
    }

	void OnTriggerEnter(Collider other)
    {
        // Is the triggering object a BoidsFish?
        BoidsFish repellant = other.gameObject.GetComponent<BoidsFish>();
        if (repellant != null)
        {
            // Is the triggering BoidsFish the same size as this one?
            if (repellant.Size == this.ParentFish.Size)
            {
                // this.ParentFish.SendMessage("AddRepellant", repellant, SendMessageOptions.RequireReceiver);
                this.ParentFish.AddRepellant(repellant);
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        // Is the triggering object a BoidsFish?
        BoidsFish repellant = other.gameObject.GetComponent<BoidsFish>();
        if (repellant != null)
        {
            // Is the triggering BoidsFish the same size as this one?
            if (repellant.Size == this.ParentFish.Size)
            {
                // this.ParentFish.SendMessage("RemoveRepellant", repellant, SendMessageOptions.RequireReceiver);
                this.ParentFish.RemoveRepellant(repellant);
            }
        }
    }
}
