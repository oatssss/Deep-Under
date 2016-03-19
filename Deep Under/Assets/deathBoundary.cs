using UnityEngine;
using System.Collections;

public class deathBoundary : MonoBehaviour
{

	void OnTriggerExit(Collider other)
	{
		BoidsFish fish = other.gameObject.GetComponent<BoidsFish>();
		if (fish && fish != GameManager.Instance.Player)
			FishManager.Instance.DestroyFish(fish);
	}
}
