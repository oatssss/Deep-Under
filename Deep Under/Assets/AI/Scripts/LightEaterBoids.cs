using UnityEngine;
using Extensions;

public class LightEaterBoids : BoidsFish {
	
	private float IdleMin = 4f;
	private float IdleMax = 8f;
	private float AbsoluteMax = 14f;
	
	public override STATE State
	{
		get				{ return this.state; }
		protected set
		{
			if (this.state != value)
				StateTimer = 0;
			
			this.state = value;
			if (value == STATE.EATING)
			{ this.MinSpeed = this.MaxSpeed = this.IdleMin; }
			else if (value == STATE.FLEEING)
			{ this.MinSpeed = this.MaxSpeed = this.AbsoluteMax; }
			else if (value == STATE.IDLE || value == STATE.SWIMMING)
			{
				this.state = STATE.IDLE;
				this.MinSpeed = this.IdleMin;
				this.MaxSpeed = this.IdleMax;
			}
			else if (value == STATE.HUNTING)
			{ this.MinSpeed = this.MaxSpeed = this.AbsoluteMax; }
		}
	}
	
	protected override void Awake()
	{
		base.Awake();
		this.EnforceLayerMembership("Light Orb");
		this.Size = SIZE.GOD;
	}
	
	#if UNITY_EDITOR
	protected override void Update()
	{
		// this.State = this.State;
		this.IdleMin = BoidsSettings.Instance.LargeFish_IdleMin;
		this.IdleMax = BoidsSettings.Instance.LargeFish_IdleMax;
		// this.SwimMin = BoidsSettings.Instance.MediumFish_SwimMin;
		// this.SwimMax = BoidsSettings.Instance.MediumFish_SwimMax;
		this.AbsoluteMax = BoidsSettings.Instance.LargeFish_AbsoluteMax;
		base.Update();
	}
	#endif

	protected void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player" && other.isTrigger == true)
		{
			this.GodBeingRepelled = true;
		}
	}

	protected void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player" && other.isTrigger == true)
		{
			this.GodBeingRepelled = false;
		}
	}

	protected override Vector3 CalculateVelocity()
	{
		if (this.GodBeingRepelled) 
		{
			return (this.transform.position - GameManager.Instance.Player.transform.position).normalized*this.MaxSpeed;
		}
		else
		{
			return base.CalculateVelocity ();
		}
	}
}
