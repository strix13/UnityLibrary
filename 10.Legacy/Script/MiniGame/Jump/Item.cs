using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
	public bool                    b_Boost, b_TimeUp, b_ObstacleSpeed;
	// Use this for initialization
	void Start () {
		transform.localPosition = new Vector2 (810.78f, 30);
		transform.localScale = new Vector2 (1, 1);
		GetComponent<UISprite> ().SetDimensions(100, 100);
		GetComponent<BoxCollider2D> ().size = new Vector2 (100, 100);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (Vector2.left * 3 * Time.deltaTime);
	}

	void OnTriggerEnter2D(Collider2D Coll)
	{
		if (Coll.gameObject.CompareTag ("Player"))
			Destroy (this.gameObject);
	}
}
