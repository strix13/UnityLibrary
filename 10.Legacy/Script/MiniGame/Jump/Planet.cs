using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.localPosition = new Vector2 (1576, -78);
		transform.localScale = new Vector2 (0.5786445f, 0.5786445f);
	}
	
	// Update is called once per frame
	void Update () {
		if(!Lean.Touch.LeanSwipeDirection4.instance.b_Obstacle_SpeedUp)
		    transform.Translate (Vector2.left * 5 * Time.deltaTime);
		if(Lean.Touch.LeanSwipeDirection4.instance.b_Obstacle_SpeedUp)
			transform.Translate (Vector2.left * 7 * Time.deltaTime);
		if (transform.localPosition.x < -735)
			Destroy (gameObject);
	}
}
