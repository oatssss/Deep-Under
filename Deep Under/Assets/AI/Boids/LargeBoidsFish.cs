using UnityEngine;
using Extensions;

public class LargeBoidsFish : BoidsFish {

    private float IdleMin = 6f;
    private float IdleMax = 13f;
    private float AbsoluteMax = 20f;

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
        this.EnforceLayerMembership("Large Fish");
        this.Size = SIZE.LARGE;
    }

#if UNITY_EDITOR
    protected override void Update()
    {
        // this.State = this.State;
        this.IdleMin = BoidsSettings.Instance.LargeFish_IdleMin;
        this.IdleMax = BoidsSettings.Instance.LargeFish_IdleMax;
        // this.SwimMin = BoidsSettings.Instance.LargeFish_SwimMin;
        // this.SwimMax = BoidsSettings.Instance.LargeFish_SwimMax;
        this.AbsoluteMax = BoidsSettings.Instance.LargeFish_AbsoluteMax;
        base.Update();
    }
#endif
}
