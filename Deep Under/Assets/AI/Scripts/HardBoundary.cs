using UnityEngine;
using Extensions;

[RequireComponent(typeof(Collider))]
public class HardBoundary : MonoBehaviour {

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
            fish.InsideHardBounds(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Is the triggering object a BoidsFish?
        BoidsFish fish = other.gameObject.GetComponent<BoidsFish>();
        if (fish != null)
        {
            fish.OutsideHardBounds(this);
        }
    }
}
