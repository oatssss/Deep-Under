using UnityEngine;
using System.Collections.Generic;
using Extensions;

public class PredatorVolume : MonoBehaviour {

    [SerializeField] private BoidsFish ParentFish;

    [SerializeField] private SphereCollider Volume;

    void Start()
    {

#if UNITY_EDITOR
        InvokeRepeating("UpdateRadius", 0f, 1f);
#endif

        if (this.ParentFish.Size == BoidsFish.SIZE.MEDIUM)
            { this.EnforceLayerMembership("Medium Predator Volumes"); }

        else if (this.ParentFish.Size == BoidsFish.SIZE.LARGE)
            { this.EnforceLayerMembership("Large Predator Volumes"); }
    }

    void UpdateRadius()
    {
        if (this.ParentFish.Size == BoidsFish.SIZE.MEDIUM)
            { this.Volume.radius = BoidsSettings.Instance.MediumPredatorRadius; }

        else if (this.ParentFish.Size == BoidsFish.SIZE.LARGE)
            { this.Volume.radius = BoidsSettings.Instance.LargePredatorRadius; }
    }

    void OnTriggerEnter(Collider other)
    {
        // Get the triggering BoidsFish
        BoidsFish predatee = other.gameObject.GetComponent<BoidsFish>();
        if (predatee != null)
        {
            // Is the triggering BoidsFish lower on the food chain?
            if (predatee.Size < this.ParentFish.Size)
            {
                predatee.AddPredator(this.ParentFish);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        BoidsFish predatee = other.gameObject.GetComponent<BoidsFish>();
        if (predatee != null)
        {
            if (predatee.Size < this.ParentFish.Size)
            {
                predatee.RemovePredator(this.ParentFish);
            }
        }
    }
}
