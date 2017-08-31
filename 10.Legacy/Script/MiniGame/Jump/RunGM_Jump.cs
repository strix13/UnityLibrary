using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunGM_Jump : MonoBehaviour {
	public static RunGM_Jump       instance;

    bool                           b_A;
	bool                           b_B;

	float                          f_WorldTime;
	float                          f_Item_percent;

	public float                   f_Time;
    
	public int                     i_count;

	public GameObject              g_Planet;
	public GameObject              g_Time;
	public GameObject              g_Obstacle_parent;
	public GameObject[]            gs_Obstacles;
	public GameObject[]            gs_Item;

	void Awake(){
		instance = this;
		b_A = false;
		b_B = false;
		f_Item_percent = 0;
		f_WorldTime = 0;
		f_Time = 1;
		i_count = 0;
	}

	// Use this for initialization
	void Start () {
		StartCoroutine (Item_Drop ());
		StartCoroutine (Planet_Instantiate ());
		StartCoroutine (Time_Value ());
	}

	// Update is called once per frame
	void Update () {

		g_Time.GetComponent<UISlider> ().value = f_Time;

		if (f_Time > 1) {
			f_Time = 1;
		} else if (f_Time < 0) 
		{
			f_Time = 0;
			Time.timeScale = 0;
		}

		if (i_count >= 20) {
			i_count = 20;
		} else if (i_count <= 0) {
			i_count = 0;
		}
			

			if (!Lean.Touch.LeanSwipeDirection4.instance.b_Boost)
			{
				f_WorldTime += Time.deltaTime;

				if (f_WorldTime >= 3) {
					f_WorldTime = 0;
					StartCoroutine (Time_Control ());
				}
			}
	

			if (b_A) {
				if (b_B) {
					b_A = false;
					b_B = false;
					i_count++;
					f_WorldTime = 0;
					Debug.Log("AB");
				}
			}

		if (Input.GetKeyDown (KeyCode.Z).Equals (true)) 
		{
			A_Button ();
		}

		if (Input.GetKeyDown (KeyCode.X).Equals (true)) 
		{
			B_Button ();
		}
	}

	IEnumerator Time_Control()
	{
		yield return new WaitForSeconds (0f);
		i_count-=5;
	}

	IEnumerator Time_Value()
	{
		yield return new WaitForSeconds (1f);
		f_Time -= 0.0084f;
		StartCoroutine (Time_Value ());
	}

	IEnumerator Planet_Instantiate()
	{
		yield return new WaitForSeconds (7f);
		Instantiate (g_Planet, g_Obstacle_parent.transform);
		StartCoroutine (Planet_Instantiate ());
	}

	IEnumerator Item_Drop()
	{
		yield return new WaitForSeconds (10);
		f_Item_percent = Random.Range (1, 3 + 1);
		if (f_Item_percent == 1)
			Instantiate (gs_Item [0], g_Obstacle_parent.transform);
		if (f_Item_percent == 2)
			Instantiate (gs_Item [1], g_Obstacle_parent.transform);
		if (f_Item_percent == 3)
			Instantiate (gs_Item [2], g_Obstacle_parent.transform);

		StartCoroutine (Item_Drop ());
	}
		
	public void A_Button()
	{
		if (!b_A) {
			b_A = true;
		} else {
			b_A = false;
		}
	}

	public void B_Button()
	{
		if (!b_B) {
			b_B = true;
		} else {
			b_B = false;
		}
	}
}
