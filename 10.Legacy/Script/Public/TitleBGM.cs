using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBGM : MonoBehaviour {

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if (Application.loadedLevel == 7) {
			GetComponent<AudioSource> ().enabled=false;
		} else {
			GetComponent<AudioSource> ().enabled=true;
		}
	}
}
