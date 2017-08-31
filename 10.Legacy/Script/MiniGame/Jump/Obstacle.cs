using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {
	public bool                     b_Up;
	public bool                     b_Trampoline; 

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.x<=-3.4f && GetComponent<BoxCollider2D> ().enabled.Equals(false))
			GetComponent<BoxCollider2D> ().enabled = true;
	}

	void OnTriggerEnter2D(Collider2D Coll)
	{
		if (Coll.gameObject.CompareTag ("Player"))
			GetComponent<BoxCollider2D> ().enabled = false;
	}
}
