using UnityEngine;
using System.Collections;

public class SuctionVolume : MonoBehaviour
{

    [SerializeField] private Player Player;
    private float SuckStrength = 25;
    private bool Sucking;
    public Light light1;
    public Light light2;
    public Color green;
    public Color yellow;

    public LightShafts lightShaft;
    public LightShafts lightShaft1;
    public Light light3;
    public Light light4;

    void Awake() {
        light3 = lightShaft.GetComponentInParent<Light>();
        light4 = lightShaft1.GetComponentInParent<Light>();
    }

    void OnTriggerStay(Collider other)
    {
        if (Player.lightOn)
        {
            EnergyBall energyBall = other.GetComponent<EnergyBall>();
            if (Input.GetAxis("Suck") > 0)
            {
                if (!this.Sucking)
                { this.SuckStrength = 1.1f; this.Sucking = true; }

                this.SuckStrength = Mathf.Clamp(this.SuckStrength * 1.1f, 1, 50);
                Vector3 towardsPlayer = (transform.parent.position - other.transform.position).normalized;
                energyBall.GetComponent<Rigidbody>().velocity = towardsPlayer * SuckStrength;
                energyBall.SetSucking(true);
            }

            else
            {
                this.Sucking = false;
                this.SuckStrength *= 0.5f;
                energyBall.SetSucking(false);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        this.Sucking = false;
        EnergyBall energyBall = other.GetComponent<EnergyBall>();
        if (energyBall)
            { energyBall.SetSucking(false); }
    }

    void Update()
    {
        if (Input.GetAxis("Suck") > 0 && Player.lightOn)
        {
            this.Player.removeEnergy(SuckStrength / 4);
            light1.color = green;
            light2.color = green;
            light3.color = green;
            light4.color = green;
        }

        else
        {
            light1.color = yellow;
            light2.color = yellow;
            light3.color = yellow;
            light4.color = yellow;
        }
    }
}