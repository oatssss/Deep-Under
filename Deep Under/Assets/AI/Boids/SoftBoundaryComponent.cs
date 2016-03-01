using UnityEngine;
using Extensions;

[RequireComponent(typeof(Collider))]
public class SoftBoundaryComponent : MonoBehaviour {

    [SerializeField] private SoftBoundary SoftBoundary;

	void Start()
    {
        this.EnforceLayerMembership("Soft Boundaries");
    }

    void OnTriggerEnter(Collider other)
    {
        // Is the triggering object a BoidsFish?
        BoidsFish fish = other.gameObject.GetComponent<BoidsFish>();
        if (fish != null)
        {
            fish.InsideSoftBounds(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Is the triggering object a BoidsFish?
        BoidsFish fish = other.gameObject.GetComponent<BoidsFish>();
        if (fish != null)
        {
            fish.OutsideSoftBounds(this);
        }
    }
}
