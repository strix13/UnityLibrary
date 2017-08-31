using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offset_Jump : MonoBehaviour {
	public bool                    b_Pedometer;

	public int                     i_UnSpeed;

	public float                   f_Minus_vector;
	public float                   f_Speed;

	public UILabel                 UIL_meter;

	Vector2                        V2_StartPos;
    Vector2                        V2_EndPos;

    int                            i_meter;

	void Awake(){
		i_meter = 0;
	}
	// Use this for initialization
	void Start () {
		V2_StartPos = transform.localPosition;
		V2_EndPos = new Vector2 (V2_StartPos.x - f_Minus_vector, V2_StartPos.y);
	}
	
	// Update is called once per frame
	void Update () {
		f_Speed = RunGM_Jump.instance.i_count / i_UnSpeed;

		if(UIL_meter != null)
		UIL_meter.text = i_meter.ToString () + "m";

		if (transform.localPosition.x <= V2_EndPos.x)
		{
			transform.localPosition = V2_StartPos;
			if (b_Pedometer) 
			{
				i_meter++;
			}
		}

		if(RunGM_Jump.instance.i_count > 0)
			transform.Translate (Vector2.left * f_Speed * Time.deltaTime);
	}
}
