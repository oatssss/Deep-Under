using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class SuctionVolume : MonoBehaviour {

    [SerializeField] private Player Player;
    private float SuckStrength = 0;
    private bool Sucking;

	void OnTriggerStay(Collider other)
    {
        if (Input.GetButton("Suck"))
        {
            if (!this.Sucking)
                { this.SuckStrength = 1.1f; this.Sucking = true; }

            this.Player.removeEnergy(SuckStrength/4);
            this.SuckStrength = Mathf.Clamp(this.SuckStrength * 1.1f, 1, 50);
            Vector3 towardsPlayer = (transform.parent.position - other.transform.position).normalized;
            other.GetComponent<Rigidbody>().velocity = towardsPlayer * SuckStrength;
			GamePad.SetVibration (PlayerIndex.One, 100, 100);
        }

        else
        {
            this.Sucking = false;
            this.SuckStrength *= 0.5f;
        }
    }
}
