using UnityEngine;
using System.Collections.Generic;
using Extensions;

public class SoftBoundary : MonoBehaviour {

    [SerializeField] private List<SoftBoundaryComponent> SoftBoundaryComponents = new List<SoftBoundaryComponent>();

	void Start()
    {
        this.EnforceLayerMembership("Soft Boundaries");
    }

    public bool Contains(SoftBoundaryComponent component)
    {
        return this.SoftBoundaryComponents.Contains(component);
    }
}
