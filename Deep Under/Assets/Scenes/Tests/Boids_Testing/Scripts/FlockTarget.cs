using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class FlockTarget : MonoBehaviour {

	void OnTriggerEnter(Collider other)
    {
        // Is the triggering object a BoidsFish?
        BoidsFish attracted = other.gameObject.GetComponent<BoidsFish>();
        if (attracted != null)
        {
            // attracted.SendMessage("AddFlockTarget", this, SendMessageOptions.RequireReceiver);
            attracted.Target = this;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        // Is the triggering object a BoidsFish?
        BoidsFish attracted = other.gameObject.GetComponent<BoidsFish>();
        if (attracted != null && attracted.Target == this)
        {
            // attracted.SendMessage("AddFlockTarget", this, SendMessageOptions.RequireReceiver);
            attracted.Target = null;
        }
    }
}
