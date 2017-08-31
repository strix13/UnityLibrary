using UnityEngine;
using System.Collections;
using System;
public class Quest : MonoBehaviour {
	public static Quest           instance;

	public GameObject             Solution_Object;
	public int                    Count;//점에 연결된 선의 개수
	public int                    Sol_Count;//몇개 연결되어야 하는지
	public bool                   Compare;//예외처리용

	void Awake(){
		instance = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Count == Sol_Count && !Compare) 
		{
			Compare = true;
			Solution_Object.GetComponent<Solution> ().Count += 1;
		}
	}
}
