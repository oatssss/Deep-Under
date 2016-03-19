using UnityEngine;

public class ControllerTest : MonoBehaviour {

    [SerializeField] private Animator Animator;
	// Update is called once per frame
	void Update () {
	    float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        this.Animator.SetFloat("Horizontal", horizontal);
        this.Animator.SetFloat("Vertical", vertical);
	}
}
