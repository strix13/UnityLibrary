using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinMove : MonoBehaviour {
	GameObject          g_Player;
	bool                b_CoinMove=false;
	int                 i_RandomVec=0;
	public float               f_Playerdis;
	// Use this for initialization
	void Start () {
		g_Player = GameObject.FindGameObjectWithTag ("Player");
		i_RandomVec = Random.Range (1, 3);
	}

	// Update is called once per frame
	void Update () {
		f_Playerdis = Vector2.Distance (transform.position, g_Player.transform.position);
		if (f_Playerdis <= 0.2f) 
		{
			Destroy (gameObject);
		}
		if (b_CoinMove) {
			Vector3 vec3dir = g_Player.transform.localPosition - transform.localPosition;
			vec3dir.Normalize ();
			transform.Translate (vec3dir * 1 * Time.deltaTime);
		} else {
			StartCoroutine (Effect ());
			if (i_RandomVec == 1) 
			{
				Vector3 vec3dir = new Vector3(transform.localPosition.x-0.03f,transform.localPosition.y+0.03f,transform.localPosition.z) - transform.localPosition;
				vec3dir.Normalize ();
				transform.Translate (vec3dir * 2f * Time.deltaTime);
			} else if (i_RandomVec == 2) 
			{
				Vector3 vec3dir = new Vector3(transform.localPosition.x+0.03f,transform.localPosition.y+0.03f,transform.localPosition.z) - transform.localPosition;
				vec3dir.Normalize ();
				transform.Translate (vec3dir * 2f * Time.deltaTime);
			}				
		}			

	}

	IEnumerator Effect()
	{
		yield return new WaitForSeconds (0.2f);
		b_CoinMove = true;
	}

//	void OnTriggerEnter2D(Collider2D Coll)
//	{
//		if (Coll.gameObject.CompareTag ("Player")) 
//		{
//			Destroy (gameObject);
//		}
//	}
}

