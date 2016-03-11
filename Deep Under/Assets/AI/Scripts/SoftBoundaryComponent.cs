using UnityEngine;
using Extensions;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class SoftBoundaryComponent : MonoBehaviour {

    public SoftBoundary SoftBoundary;

#if UNITY_EDITOR
    // Automatically assign the parent softboundary as the associated boundary. If non-existent, show a warning and destroy
    void Reset() {
        if (!this.SoftBoundary)
        {
            SoftBoundary parentBoundary = null;
            if (transform.parent)
                { parentBoundary = transform.parent.GetComponent<SoftBoundary>(); }
            if (parentBoundary)
                { this.SoftBoundary = parentBoundary; }
            else
            {
                EditorUtility.DisplayDialog("Invalid Soft Boundary Component Context", "A soft boundary component may only exist as a child of a soft boundary", "I understand");
                DestroyImmediate(this);
            }
        }
    }

#endif

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
