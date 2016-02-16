using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class FishTarget : MonoBehaviour {
    
    [SerializeField] public List<BoidsFish.SIZE> Attracts = new List<BoidsFish.SIZE>();

	void OnTriggerEnter(Collider other)
    {
        // Is the triggering object a BoidsFish that is attracted to this target?
        BoidsFish attracted = other.gameObject.GetComponent<BoidsFish>();
        if (attracted != null && this.Attracts.Contains(attracted.Size))
        {
            // attracted.SendMessage("AddFlockTarget", this, SendMessageOptions.RequireReceiver);
            attracted.PhysicalTarget = this;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        // Is the triggering object a BoidsFish?
        BoidsFish attracted = other.gameObject.GetComponent<BoidsFish>();
        if (attracted != null && attracted.PhysicalTarget == this)
        {
            // attracted.SendMessage("AddFlockTarget", this, SendMessageOptions.RequireReceiver);
            attracted.PhysicalTarget = null;
        }
    }
}
