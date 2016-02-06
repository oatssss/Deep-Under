using UnityEngine;

public class BoidsSettings : UnitySingleton<BoidsSettings> {

	[Range(1,50)] public float FlockRadius = 20;
    [Range(1,40)] public float RepelRadius = 10;
    
    [Range(0,50)] public float Cohesion = 1;
    [Range(0,100)] public float Separation = 7;
    [Range(0,5f)] public float Alignment = 2f;
    [Range(0,50)] public float Target = 7;
    
    [Range(0,30)] public float FishSpeed = 5;
}
