using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour {

    public float spawnRate;
    public float spawnQuantity;
    public float spawnChance;
    public GameObject creature;


	// Use this for initialization
	void Start () {

        InvokeRepeating("spawnCreature", spawnRate, spawnRate);
	
    }

    void spawnCreature()
    {
        if (Random.value < spawnChance)
        {
            for (int i = 0; i < spawnQuantity; i++)
            {
                Debug.Log("Spawning");
                Instantiate(creature, this.transform.position + new Vector3(0,4,0), Quaternion.Euler(0.0f, (float)Random.Range(0, 360), 0.0f));
            }
        }
    }
}
