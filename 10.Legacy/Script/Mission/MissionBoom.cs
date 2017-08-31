using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class MissionBoom : MonoBehaviour {
//	bool                    b_Move=false;
	int                     i_MoveY=-900;
	public float            f_Y;

	SkeletonAnimation       Mine;
	// Use this for initialization
	void Start () {
		Mine=GetComponent<SkeletonAnimation> ();
	}
	
	// Update is called once per frame
	void Update () {

//		if (!b_Move) {
//			transform.Translate (Vector2.up * 2 * Time.deltaTime);
//		} else {
//			Mine.loop = false;
//			Mine.timeScale = 0.6f;
//			Mine.AnimationName = "bang";
//		}

//		if (transform.localPosition.y >= f_Y) 
//		{
//			transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y + 50);
//			transform.localScale = new Vector3(80,80,80);
////			b_Move = true;
//			StartCoroutine (destroy ());
//		}
	}

	IEnumerator destroy()
	{
		yield return new WaitForSeconds (5f);
		Destroy (gameObject);
	}
}
