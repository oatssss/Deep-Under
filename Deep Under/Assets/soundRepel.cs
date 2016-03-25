using UnityEngine;
using System.Collections;

public class soundRepel : MonoBehaviour {

	protected void OnTriggerEnter(Collider other)
	{
		if (other.isTrigger == false)
		{
			BoidsFish fish = other.gameObject.GetComponent<BoidsFish> ();
			if (fish && other.gameObject != GameManager.Instance.Player)
			{
				Debug.Log ("Repel!");
				fish.beingRepelled = true;
				fish.delayCancelRepel (2);
			}
		}
	}
}
