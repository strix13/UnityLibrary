using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetBreak : MonoBehaviour {

	public static PlanetBreak            instance;

	int                                  i_Random_Num;
	int                                  i_Random_Shine;
	public int                           i_BreakNum;
	public int                           i_minNum;
	public int                           i_MaxNum;

	public bool                          b_Speed_Down;

	public UILabel                       UIL_Break_Num;

	void Awake()
	{
		instance     = this;

		PlanetBreakGM.instance.b_TimeExist = true;

		i_Random_Num = Random.Range(i_minNum,i_MaxNum+1);
		i_Random_Shine = Random.Range (1, 101);
		transform.localPosition = new Vector2 (1097, 0);
		transform.localScale = new Vector2 (3.615806f,3.615806f);
		GetComponent<UISprite> ().SetDimensions (72, 72);


	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		UIL_Break_Num.text = i_Random_Num.ToString ();

		if (!b_Speed_Down)
			transform.Translate (Vector2.left * 1f * Time.deltaTime);
		else {
			transform.Translate (Vector2.left * 0.5f * Time.deltaTime);
		}

		if (i_BreakNum >= i_Random_Num) 
		{
			Debug.Log ("부숨");
			Destroy (gameObject);
		}


		if (i_Random_Shine >= 100) 
		{
			Color Blue=GetComponent<UISprite> ().color;
			Blue.r = 0;
			Blue.g = 0;
			Blue.b = 1;
			GetComponent<UISprite> ().color = Blue;
		}
	}
}
