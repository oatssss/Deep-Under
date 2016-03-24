using UnityEngine;
using System.Collections.Generic;

public class OrbManager : UnitySingletonPersistent<OrbManager> {

	public List<lightOrb> OrbList = new List<lightOrb>();
	public List<EnergyBall> EnergyList = new List<EnergyBall>();
	public int MaxOrbNumber = 1;
	public int AttractNumber = 5;

	private lightOrb _orb;
	private EnergyBall _eball;

	public void addOrb(lightOrb o)
	{
		this.OrbList.Add(o);
		while (OrbList.Count > MaxOrbNumber)
		{
			_orb = OrbList[0];
			destroyOrb(_orb);
		}
	}
	public void addEnergy(EnergyBall e)
	{
		this.EnergyList.Add(e);
	}

	public void destroyOrb(lightOrb o)
	{
		OrbList.RemoveAll(orb => orb == o);
		GameObject.Destroy(o.gameObject);
	}

	public void destroyEnergy(EnergyBall ball)
	{
		EnergyList.RemoveAll(orb => orb == ball);
		GameObject.Destroy(ball.gameObject);
	}
	// Update is called once per frame
	void Update () {
	
	}


}
