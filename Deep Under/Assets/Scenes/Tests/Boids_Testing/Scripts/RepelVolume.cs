using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class RepelVolume : MonoBehaviour {

    [SerializeField] private BoidsFish ParentFish;
    
    [SerializeField] private SphereCollider Volume;
    
    void Update()
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
                this.ParentFish.SendMessage("AddRepellant", repellant, SendMessageOptions.RequireReceiver);
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
                this.ParentFish.SendMessage("RemoveRepellant", repellant, SendMessageOptions.RequireReceiver);
            }
        }
    }
}
