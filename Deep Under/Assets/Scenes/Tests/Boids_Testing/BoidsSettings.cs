using UnityEngine;

public class BoidsSettings : UnitySingleton<BoidsSettings> {

	[Range(1,100)] public float FlockRadius = 10;
    [Range(1,10)] public float RepelRadius = 5;
    
    [Range(0,500)] public float Cohesion = 100;
    [Range(0,100)] public float Separation = 15;
    [Range(0,1.5f)] public float Alignment = 0.7f;
    [Range(0,100)] public float Target = 1;
    
    [Range(0,30)] public float FishSpeed = 5;
}
