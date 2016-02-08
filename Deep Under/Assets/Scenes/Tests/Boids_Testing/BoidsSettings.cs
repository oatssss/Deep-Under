using UnityEngine;

public class BoidsSettings : UnitySingleton<BoidsSettings> {

	[Range(1,50f)] public float FlockRadius = 20f;
    [Range(1,25f)] public float RepelRadius = 5f;
    
    [Range(0,3f)] public float Cohesion = 1f;
    [Range(0,3f)] public float Separation = 1f;
    [Range(0,3f)] public float Alignment = 2f;
    [Range(0,5f)] public float Target = 5f;
    
    [Range(0,30f)] public float FishSpeed = 5f;
}
