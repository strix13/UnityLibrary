using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Public_Offset : MonoBehaviour {
	public bool                           b_Start;
	public bool                           b_Portrait;
	public int                            i_X;
	public int                            i_Y;
	public float                          i_X_Start;
	public float                          i_Y_Start;
	public float                          f_Speed;

	void Awake()
	{
		i_X_Start = transform.localPosition.x;
		i_Y_Start = transform.localPosition.y;
	}

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		if (b_Start)
		{
			if (!b_Portrait) 
			{
				transform.Translate (Vector2.left * f_Speed * Time.deltaTime);
				if (transform.localPosition.x < i_X) 
				{
					transform.localPosition = new Vector2 (i_X_Start, 0);
				}
			} else {
				transform.Translate (Vector2.down * f_Speed * Time.deltaTime);//리소스 바뀔시 Vector2.down으로 변경
				if (transform.localPosition.y < i_Y) 
				{
					transform.localPosition = new Vector2 (0, i_Y_Start);
				}
			}
		}
	}
}