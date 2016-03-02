using UnityEngine;
using System.Collections;

public class Fish : MonoBehaviour {

	public GameObject body;
	public GameObject fleeTest;

	public float speed;	// Default
	public enum STATE { IDLE, SWIMMING, FLEEING, HUNTING }
	public STATE state;
	public Vector3 destination;
	public Vector3 dangerPoint;
	
	public enum SIZE { SMALL, MEDIUM, LARGE }
	public SIZE size;

	protected float runAwayTime;
	protected float idleTime;
	
	public void Initialize(){
		this.destination = randomizeDestination();
	}

	// Use this for initialization
	void Start () {
		setSize ();
		state = STATE.IDLE;			// fish start off IDLE
		this.gameObject.tag="Fish"; // set fish tag
	}

	protected virtual void setSize()
	{
		// will be implementated in subclasses
	}

	// Update is called once per frame
	void FixedUpdate () {
		//FSM implementation
		switch (state) {
		case STATE.IDLE:
			{
				idleTime += Time.deltaTime;
				if (idleTime >= 1.0f) {
					//TODO: make this a list of tasks iterating through maybe
					this.destination = randomizeDestination (); 	
					this.state = STATE.SWIMMING;
				}
				break;
			}
		case STATE.SWIMMING:
			{
				this.GetComponent<Rigidbody> ().MoveRotation (Quaternion.LookRotation (destination - transform.position));
				this.GetComponent<Rigidbody> ().MovePosition (Vector3.MoveTowards (transform.position, destination, speed * Time.deltaTime));
				
				//if (AtDestination()) ...?
				if (AtDestination ()) {
					state = STATE.IDLE;
					idleTime = 0;
				}
				break;
			} 
		case STATE.FLEEING:
			{
				this.GetComponent<Rigidbody> ().MoveRotation (Quaternion.LookRotation (dangerPoint - transform.position));
				this.GetComponent<Rigidbody> ().MovePosition (Vector3.MoveTowards (transform.position, dangerPoint, -speed * Time.deltaTime));
				// runs away for 5 seconds
				runAwayTime += Time.deltaTime;
				if (runAwayTime > 5.0f) {	
					state = STATE.IDLE;
					idleTime = 0;
					fleeTest.SetActive (false);
				}
				break;
			}
		}
	}

	//Helper methods
	public Vector3 randomizeDestination(){
		float radius = 20.0f;				// depends on the size of terrain?
		Vector3 random = Random.insideUnitSphere * radius;
		random = new Vector3(random.x, random.y, random.z);
		random += transform.position;
		random.y = Mathf.Abs (random.y);
		return random;
	}

	// NOTE: not used anywhere yet	
	protected bool AtDestination() {
		// if more or less reached destination
		if (Vector3.Distance(this.GetComponent<Rigidbody>().position, destination) < 5){
			return true;
		}
		
		return false;
	}
	public Vector3 CurrentDirection() {
		return destination - this.transform.position;
	}
	protected bool Hunt(GameObject food){
		destination = food.transform.position;	
		state = STATE.IDLE;						//TODO: Change later so that fish might not catch other fish
		return true;
	}

#region triggerDetection

	public void OnTriggerEnterEx(Collider otherCollider, Collider selfCollider)
	{
		if (otherCollider == body.GetComponent<Collider>())
			return;
		if (otherCollider.tag == "Fish") 
		{
			// encountered a bigger fish
			if (state != STATE.FLEEING && (int)otherCollider.gameObject.GetComponentInParent<Fish> ().size > (int)this.size) {
				dangerPoint = otherCollider.gameObject.transform.position;
				state = STATE.FLEEING;
				runAwayTime = 0.0f;
				fleeTest.SetActive (true);
			}
//			if (state == STATE.HUNTING)
//			{
//				bool eaten = Hunt(otherCollider.gameObject);
//				if (eaten){
//					state = STATE.IDLE;	// No longer hunting
//				}
//			}
		}
	}

#endregion
}
