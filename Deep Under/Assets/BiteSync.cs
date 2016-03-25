using UnityEngine;
using System.Collections;

public class BiteSync : MonoBehaviour {

	[SerializeField] BoidsFish ParentFish;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Bite() {
		Instantiate(FishManager.Instance.EnergyBall, ParentFish.BeingEaten.transform.position, FishManager.Instance.EnergyBall.transform.rotation);
		if (ParentFish.audioSource == null) ParentFish.audioSource = GetComponent<AudioSource>();
		ParentFish.audioSource.clip = ParentFish.eatSound;
		ParentFish.audioSource.pitch = Random.Range(1f, 2f);
		ParentFish.audioSource.Play();

		FishManager.Instance.DestroyFish(ParentFish.BeingEaten);
		ParentFish.BeingEaten = null;
	}
}
