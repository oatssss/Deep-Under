using UnityEngine;
using Extensions;

public class PredatorVolume : MonoBehaviour {

    [SerializeField] private BoidsFish ParentFish;

    [SerializeField] private SphereCollider Volume;

    void Start()
    {

#if UNITY_EDITOR
        InvokeRepeating("UpdateRadius", 0f, 1f);
#endif

        if (this.ParentFish.Size == BoidsFish.SIZE.SMALL)
            { this.EnforceLayerMembership("Small Predator Volumes"); }

        else if (this.ParentFish.Size == BoidsFish.SIZE.MEDIUM)
            { this.EnforceLayerMembership("Medium Predator Volumes"); }
    }

    void UpdateRadius()
    {
        if (this.ParentFish.Size == BoidsFish.SIZE.SMALL)
            { this.Volume.radius = BoidsSettings.Instance.SmallPredatorRadius; }

        else if (this.ParentFish.Size == BoidsFish.SIZE.MEDIUM)
            { this.Volume.radius = BoidsSettings.Instance.MediumPredatorRadius; }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("SDFSDFDSSDDF");
        // Get the triggering BoidsFish
        BoidsFish predator = other.gameObject.GetComponent<BoidsFish>();
        if (predator != null)
        {
            // Is the triggering BoidsFish the same size as this one?
            if (predator.Size > this.ParentFish.Size)
            {
                this.ParentFish.AddPredator(predator);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        BoidsFish predator = other.gameObject.GetComponent<BoidsFish>();
        if (predator != null)
        {
            if (predator.Size > this.ParentFish.Size)
            {
                this.ParentFish.RemovePredator(predator);
            }
        }
    }
}
