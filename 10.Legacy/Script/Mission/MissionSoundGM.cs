using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSoundGM : MonoBehaviour {
	public static MissionSoundGM           instance;
	public GameObject[]                    gs_Sounds;
	// Use this for initialization
	void Start () {
		instance = this;	

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CoinSound()
	{
		gs_Sounds [0].GetComponent<AudioSource> ().Play ();
	}
}
