using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class TargetVolume : MonoBehaviour {

	[SerializeField] private BoidsFish parentFish;
	[SerializeField] private SphereCollider volume;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	void OnTriggerEnter(Collider other)
	{
		// Is the triggering object a BoidsFish?
		BoidsFish target = other.gameObject.GetComponent<BoidsFish>();
		if (target != null)
		{
			// Is the triggering BoidsFish has smaller size?
			if (target.Size < this.parentFish.Size)
			{
				this.parentFish.addPrey(target);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		// Is the triggering object a BoidsFish?
		BoidsFish target = other.gameObject.GetComponent<BoidsFish>();
		if (target != null)
		{
			// Is the triggering BoidsFish has smaller size?
			if (target.Size < this.parentFish.Size)
			{
				this.parentFish.removePrey(target);
			}
		}
	}
}
