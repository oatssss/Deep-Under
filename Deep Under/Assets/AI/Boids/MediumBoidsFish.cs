using UnityEngine;
using Extensions;

public class MediumBoidsFish : BoidsFish {

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
        this.EnforceLayerMembership("Medium Fish");
        this.Size = SIZE.MEDIUM;
    }

#if UNITY_EDITOR
    protected override void Update()
    {
        // this.State = this.State;
        this.IdleMin = BoidsSettings.Instance.MediumFish_IdleMin;
        this.IdleMax = BoidsSettings.Instance.MediumFish_IdleMax;
        // this.SwimMin = BoidsSettings.Instance.MediumFish_SwimMin;
        // this.SwimMax = BoidsSettings.Instance.MediumFish_SwimMax;
        this.AbsoluteMax = BoidsSettings.Instance.MediumFish_AbsoluteMax;
        base.Update();
    }
#endif

}
