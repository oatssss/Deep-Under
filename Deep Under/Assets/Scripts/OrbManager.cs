using UnityEngine;
using System.Collections.Generic;

public class OrbManager : UnitySingletonPersistent<OrbManager> {

	protected List<lightOrb> OrbList = new List<lightOrb>();
	public int MaxOrbNumber = 2;
	private lightOrb _orb;
	// Use this for initialization
	void Start () {
	
	}
	public void addOrb(lightOrb o)
	{
		this.OrbList.Add(o);
		while (OrbList.Count > MaxOrbNumber)
		{
			_orb = OrbList[0];
			destroyOrb(_orb);
		}
	}

	public void destroyOrb(lightOrb o)
	{
		OrbList.Remove(o);
		GameObject.Destroy(o.gameObject);
	}
	// Update is called once per frame
	void Update () {
	
	}
}
