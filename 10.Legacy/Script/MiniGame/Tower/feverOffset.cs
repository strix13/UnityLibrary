using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class feverOffset : MonoBehaviour {
	public bool                b_Mission;
	
	// Update is called once per frame
	void Update () {
		if (!b_Mission) {
			if (transform.localPosition.y <= -1100) {
				transform.localPosition = new Vector2 (transform.localPosition.x, 0);
			}
			transform.Translate (Vector2.down * 2 * Time.deltaTime);
		} else {
			if (transform.localPosition.y <= -1280) {
				transform.localPosition = new Vector2 (transform.localPosition.x, 0);
			}
			transform.Translate (Vector2.down * 0.3f * Time.deltaTime);
		}
	}
}
