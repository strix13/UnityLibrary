using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public static Player           instance;

	public int                     i_StrikingPower;
	public GameObject              g_World;
	void Awake()
	{
		instance        = this;
		i_StrikingPower = 1;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D Coll)
	{
		if (Coll.gameObject) 
		{
			PlanetBreakGM.instance.f_Timelimit -= 0.1f;
			iTween.ShakePosition(g_World, iTween.Hash("x", 0.02f, "y", 0.02f, "time", 1f)); 
			PlanetBreakGM.instance.b_TimeExist = false;
			Debug.Log ("맞음");
			Destroy (Coll.gameObject);
		}
	}
}
