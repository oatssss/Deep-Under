using UnityEngine;
using System.Collections;

public class JellyFishMovement : MonoBehaviour {

    public Vector2 rangeOfMovement;
    public float speed;
    public bool goingDown;
    public float height;

	// Use this for initialization
	void Start () {
        goingDown = false;
        height = rangeOfMovement.x;
	}
	
	// Update is called once per frame
	void Update () {

        if (goingDown)
        {
            transform.Translate(new Vector3(1.0f, 1.0f, 0) * speed * Time.deltaTime);
            height++;
            if (height > rangeOfMovement.y)
                goingDown = false;
        }

        if (!goingDown)
        {
            transform.Translate(new Vector3(0, -1.0f, 0) * speed * Time.deltaTime);
            height--;
            if (height < rangeOfMovement.x)
                goingDown = true;
        }
	}
}
