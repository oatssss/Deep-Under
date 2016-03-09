using UnityEngine;
using System.Collections.Generic;
using Extensions;

public class SoftBoundary : MonoBehaviour {

    [SerializeField] private bool AutoAddBoundaryComponents = true;
    [SerializeField] private List<SoftBoundaryComponent> SoftBoundaryComponents = new List<SoftBoundaryComponent>();
    [SerializeField] private bool SpawnsFish = true;
    [SerializeField] private int FishCount = 0;

	void Start()
    {
        this.EnforceLayerMembership("Soft Boundaries");

        // Auto-find children boundary components if enabled
        if (this.AutoAddBoundaryComponents)
        {
            foreach (SoftBoundaryComponent component in gameObject.GetComponentsInChildren<SoftBoundaryComponent>(false))
            {
                this.SoftBoundaryComponents.Add(component);
            }
        }
    }

    public bool Contains(SoftBoundaryComponent component)
    {
        return this.SoftBoundaryComponents.Contains(component);
    }

    public void IncreaseFishCount()
    {
        this.FishCount++;
    }

    public void DecreaseFishCount()
    {
        this.FishCount--;
    }
}
