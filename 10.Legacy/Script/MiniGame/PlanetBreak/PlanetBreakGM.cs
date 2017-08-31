using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetBreakGM : MonoBehaviour {

	public static PlanetBreakGM             instance;

	float                                   f_GameTime;
	float                                   f_Playerdis;

	GameObject                              g_Player;

	public bool                             b_TimeExist;

	public float                            f_Timelimit;

	public GameObject                       World;
	public GameObject                       g_Timelimit;
	public GameObject[]                     Gs_Planets;
	public GameObject[]                     Gs_PlanetsClone;

	void Awake()
	{
		instance = this;

		f_Timelimit = 1;
		f_GameTime = 0;
		g_Player = GameObject.FindGameObjectWithTag ("Player");
		StartCoroutine (Planet_Instantiate ());
		StartCoroutine (Timelimit ());
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		g_Timelimit.GetComponent<UISlider> ().value = f_Timelimit;

		f_GameTime += Time.deltaTime;
		Gs_PlanetsClone = GameObject.FindGameObjectsWithTag ("Planets");

		if(b_TimeExist)
		f_Playerdis = Vector2.Distance (g_Player.transform.position, Gs_PlanetsClone [0].transform.position);
	}

	IEnumerator Planet_Instantiate()
	{
		yield return new WaitForSeconds (2f);
		if (f_GameTime <= 30) 
		{
			Instantiate (Gs_Planets [0], World.transform);
		} else if (f_GameTime <= 60) {
			Instantiate (Gs_Planets [1], World.transform);
		} else if (f_GameTime <= 90) {
			Instantiate (Gs_Planets [2], World.transform);
		}

		StartCoroutine (Planet_Instantiate ());
	}

	IEnumerator Timelimit()
	{
		yield return new WaitForSeconds (1f);
		f_Timelimit -= 0.01f;

		StartCoroutine (Timelimit ());
	}

	public void BreakButton()
	{
		if (b_TimeExist)
		{
			if (f_Playerdis <= 1.3f) 
			{
				iTween.ShakePosition(Gs_PlanetsClone[0], iTween.Hash("x", 0.02f, "y", 0.02f, "time", 0.1f)); 
				Gs_PlanetsClone [0].GetComponent<PlanetBreak> ().b_Speed_Down = true; 
				Gs_PlanetsClone [0].GetComponent<PlanetBreak> ().i_BreakNum += Player.instance.i_StrikingPower;	
			}
		}
	}
}
