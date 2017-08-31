using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offset : MonoBehaviour {
	public static Offset             instance;

	public float                     i_UnSpeed;
	public int                       i_imageSize;
	public int                       i_count;
	Vector2                          StartPos;
	Vector2                          EndPos;
	UISprite                         UIs_mine;
	public bool                      b_Startbool = false;

	void Awake(){
		instance = this;

		i_count = 0;
		StartPos = transform.localPosition;
		EndPos = new Vector2 (StartPos.x - i_imageSize, StartPos.y);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (b_Startbool)
		{
			if (transform.localPosition.x <= EndPos.x) {
				i_count++;
				transform.localPosition = StartPos;
			}

			if (!RunGM.instance.b_Stop) {
				if (!RunGM.instance.b_Boost) {
					transform.Translate (Vector2.left * i_UnSpeed * Time.deltaTime);
				} else {
					transform.Translate (Vector2.left * i_UnSpeed * 1.5f * Time.deltaTime);
				}
			}
		}
	}
}
