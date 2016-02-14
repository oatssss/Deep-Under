using UnityEngine;

public class BoidsSettings : UnitySingleton<BoidsSettings> {

	[Range(1,50f)] public float FlockRadius = 20f;
    [Range(1,25f)] public float RepelRadius = 5f;
    
    [Range(0,1f)] public float Cohesion = 0.7f;
    // [Range(0,20f)] public float Separation = 1f;
    public float Separation { get { return (2f-Cohesion); } }
    [Range(0,2f)] public float Alignment = 1f;
    [Range(0,1f)] public float Target = 1f;
    
    [Range(0,30f)] public float MinFishSpeed = 1f;
    [Range(1f,30f)] public float MaxFishSpeed = 5f;
    [Range(1f,5f)] public float FishSpeedMultiplier = 1f;
    
    [Range(0,100)] public int MaxFlockSize = 10;
}
