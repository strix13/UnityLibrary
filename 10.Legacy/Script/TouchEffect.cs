using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchEffect : MonoBehaviour {
	public static TouchEffect  instance;
	public GameObject[]        gs_Effect;

	void Awake()
	{
		instance = this;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public IEnumerator Effect()
	{
		gs_Effect [2].SetActive (true);
		gs_Effect [1].SetActive (false);
		gs_Effect [0].SetActive (false);
		yield return new WaitForSeconds (0.2f);
		gs_Effect [2].SetActive (false);
		gs_Effect [1].SetActive (true);
		gs_Effect [0].SetActive (false);
		yield return new WaitForSeconds (0.2f);
		gs_Effect [2].SetActive (false);
		gs_Effect [1].SetActive (false);
		gs_Effect [0].SetActive (true);
		yield return new WaitForSeconds (0.2f);

		StartCoroutine (Effect ());
	}
}
