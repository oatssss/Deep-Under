using UnityEngine;
using Extensions;

public class HuntVolume : MonoBehaviour {

	[SerializeField] private BoidsFish ParentFish;

    [SerializeField] private SphereCollider Volume;

    void Start()
    {

#if UNITY_EDITOR
        InvokeRepeating("UpdateRadius", 0f, 1f);
#endif

        if (this.ParentFish.Size == BoidsFish.SIZE.MEDIUM)
            { this.EnforceLayerMembership("Medium Hunt Volumes"); }

        else if (this.ParentFish.Size == BoidsFish.SIZE.LARGE)
            { this.EnforceLayerMembership("Large Hunt Volumes"); }
    }

    void UpdateRadius()
    {
        if (this.ParentFish.Size == BoidsFish.SIZE.MEDIUM)
            { this.Volume.radius = BoidsSettings.Instance.MediumHuntRadius; }

        else if (this.ParentFish.Size == BoidsFish.SIZE.LARGE)
            { this.Volume.radius = BoidsSettings.Instance.LargeHuntRadius; }
    }

    void OnTriggerEnter(Collider other)
    {
        // Get the triggering BoidsFish
        BoidsFish predatee = other.gameObject.GetComponent<BoidsFish>();
        if (predatee != null && (predatee.Size < this.ParentFish.Size))
        {
            this.ParentFish.AddPredatee(predatee);
        }
    }

    void OnTriggerExit(Collider other)
    {
        BoidsFish predatee = other.gameObject.GetComponent<BoidsFish>();
        if (predatee != null && (predatee.Size < this.ParentFish.Size))
        {
            this.ParentFish.RemovePredatee(predatee);

            if (predatee.PredatorCount <= 0)
            {
                predatee.Relax();
            }
        }
    }
}
