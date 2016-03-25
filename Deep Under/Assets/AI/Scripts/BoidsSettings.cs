using UnityEngine;

public class BoidsSettings : UnitySingleton<BoidsSettings> {

    [Header("Volumes")]
	[Range(1,50f)] public float FlockRadius = 20f;
    [Range(1,25f)] public float RepelRadius = 8f;
    [Range(1,80f)] public float MediumPredatorRadius = 45f;
    [Range(1,80f)] public float MediumHuntRadius = 30f;
    [Range(1,80f)] public float LargePredatorRadius = 80f;
    [Range(1,80f)] public float LargeHuntRadius = 55f;
	[Range(1,80f)] public float EnergyEaterRadius = 55f;
    [Space(10)]

    [Header("Calculated Stages")]
    [Range(0,1f)] public float Cohesion = 0.7f;
    public float Separation { get { return (2f-Cohesion); } }
    [Range(0,2f)] public float Alignment = 1f;
    [Range(0,20f)] public float Target = 5f;
    [Range(0,2f)] public float Bounds = 1f;
    [Range(0,2f)] public float Evade = 1f;
    [Space(10)]

    [Header("Small Fish Speeds")]
    [Range(0,30f)] public float SmallFish_IdleMin = 2f;
    [Range(1f,30f)] public float SmallFish_IdleMax = 6f;
    [Range(1f,30f)] public float SmallFish_SwimMin = 4f;
    [Range(1f,30f)] public float SmallFish_SwimMax = 8f;
    [Range(1f,30f)] public float SmallFish_AbsoluteMax = 10f;
    [Space(10)]

    [Header("Medium Fish Speeds")]
    [Range(0,30f)] public float MediumFish_IdleMin = 4f;
    [Range(1f,30f)] public float MediumFish_IdleMax = 8f;
    [Range(1f,30f)] public float MediumFish_AbsoluteMax = 14f;
    [Space(10)]

    [Header("Large Fish Speeds")]
    [Range(0,30f)] public float LargeFish_IdleMin = 6f;
    [Range(1f,30f)] public float LargeFish_IdleMax = 12f;
    [Range(1f,30f)] public float LargeFish_AbsoluteMax = 18f;
    [Space(10)]

    [Header("Miscellaneous")]
    //[Range(0,30f)] public float MinFishSpeed = .5f;
    //[Range(1f,30f)] public float MaxFishSpeed = 5f;
    [Range(1f,5f)] public float FishSpeedMultiplier = 1f;
    [Range(0,100)] public int MaxFlockSize = 8;
    [Range(2,15)] public int MinFlockSizeToScareMediumFish = 6;
	[Range(2,15)] public int MinFlockSizeToAttractLargeFish = 6;
	public bool DrawTargetRays = false;
	[Range(30f, 90f)] public float MedFishVisualAngle = 70f;
	public bool AulivTheBestPrey = false;

#if UNITY_EDITOR
    void Update()
    {
        // Hunt radii must be smaller than predator radii
        BoidsSettings.Instance.MediumHuntRadius = Mathf.Clamp(BoidsSettings.Instance.MediumHuntRadius, 1f, BoidsSettings.Instance.MediumPredatorRadius);
        BoidsSettings.Instance.LargeHuntRadius = Mathf.Clamp(BoidsSettings.Instance.LargeHuntRadius, 1f, BoidsSettings.Instance.LargePredatorRadius);
    }
#endif

}
