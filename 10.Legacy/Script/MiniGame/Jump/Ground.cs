using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour {
	public GameObject                g_Player;
	public GameObject                g_Mine_Ground;
	float                            f_Playerdis;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		f_Playerdis = Vector2.Distance (transform.position, g_Player.transform.position);

		if (f_Playerdis >= 13)
		{
			g_Mine_Ground.SetActive (false);
		} else {
			g_Mine_Ground.SetActive (true);
		}
	}
}
