using UnityEngine;
using System.Collections;

public class Fish : MonoBehaviour {

	public float speed;	// Default
	public enum STATE { IDLE, SWIMMING, FLEEING, HUNTING }
	public STATE state;
	public Vector3 destination;
	public Vector3 dangerPoint;
	
	public enum SIZE { SMALL, MEDIUM, LARGE }
	public SIZE size;

	protected float runAwayTime;
	
	public void Initialize(){
		this.destination = randomizeDestination();
	}

	// Use this for initialization
	void Start () {
		state = STATE.IDLE;			// fish start off IDLE
		this.gameObject.tag="Fish"; // set fish tag
	}

	// Update is called once per frame
	void Update () {
		this.transform.LookAt(destination);
		//FSM implementation
		switch (state) {
			case STATE.IDLE:
				Wait(4.0f); 	// idle for 4 seconds then start swimming
				state = STATE.SWIMMING;
				//TODO: make this a list of tasks iterating through maybe
				this.destination = randomizeDestination(); 	
				this.state = STATE.SWIMMING;
				break;
			case STATE.SWIMMING:
				transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
				
				//if (AtDestination()) ...?
				if(transform.position == destination){
					state = STATE.IDLE;
				}
				break;
			case STATE.FLEEING:
				transform.position = -Vector3.MoveTowards(transform.position, dangerPoint, speed * Time.deltaTime);
				// runs away for 5 seconds
				if(runAwayTime > 5.0f){	
					state = STATE.IDLE;
				}
				runAwayTime += Time.deltaTime;
				break;
		}
	}

	void OnTriggerEnter(Collider otherCollider) {
		
		if (otherCollider.tag == "Fish") 
		{
			// encountered a bigger fish
			if ((int)otherCollider.gameObject.GetComponent<Fish>().size > (int)this.size){
				dangerPoint = otherCollider.gameObject.transform.position;
				state = STATE.FLEEING;
			}
			if (state == STATE.HUNTING)
			{
				bool eaten = Hunt(otherCollider.gameObject);
				if (eaten){
					state = STATE.IDLE;	// No longer hunting
				}
			}
		}
	} // end OnTriggerEnter 

	//Helper methods
	public Vector3 randomizeDestination(){
		float radius = 45.0f;				// depends on the size of terrain?
		Vector3 random = Random.insideUnitSphere * radius;
		random = new Vector3(random.x, Mathf.Abs(random.y), random.z);
		return random;
	}

	// NOTE: not used anywhere yet	
	protected bool AtDestination() {
		// if more or less reached destination
		if (DistanceFromDestination() < 0.5f){
			return true;
		}
		
		return false;
	}
	public float DistanceFromDestination() {
		float distance = Vector3.Dot (this.transform.position - destination, Vector3.Normalize (CurrentDirection()));
		return distance;
	}
	public Vector3 CurrentDirection() {
		return destination - this.transform.position;
	}
	protected bool Hunt(GameObject food){
		destination = food.transform.position;	
		state = STATE.IDLE;						//TODO: Change later so that fish might not catch other fish
		return true;
	}
	IEnumerator Wait(float time) {
		yield return new WaitForSeconds(time);
	}
}
