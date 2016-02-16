using UnityEngine;
using Extensions;

[RequireComponent(typeof(Collider))]
public class BoidsBoundary : MonoBehaviour {
    
    void Start()
    {
        this.EnforceLayerMembership("Level Boundaries");
    }

    void OnTriggerEnter(Collider other)
    {
        // Is the triggering object a BoidsFish?
        BoidsFish fish = other.gameObject.GetComponent<BoidsFish>();
        if (fish != null)
        {
            fish.InsideBounds(this);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        // Is the triggering object a BoidsFish?
        BoidsFish fish = other.gameObject.GetComponent<BoidsFish>();
        if (fish != null)
        {
            fish.OutsideBounds(this);
        }
    }
}
