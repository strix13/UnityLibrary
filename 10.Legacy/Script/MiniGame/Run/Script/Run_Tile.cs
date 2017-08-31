using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run_Tile : MonoBehaviour {
	public bool                       A;//false면 B true면 A
	public bool                       Red;//true면 red
	public bool                       first;
	public string                     StartImage;
	// Use this for initialization
	void Start () {
		StartImage = GetComponent<UISprite> ().spriteName;
		transform.localPosition = new Vector2 (694, 187);
		GetComponent<UISprite> ().SetDimensions (100, 100);
		GetComponent<BoxCollider2D> ().size = new Vector2 (91, 94);
	}
	
	// Update is called once per frame
	void Update () {

		if (first) 
		{
			RunGM.instance.G_firstObject = this.gameObject;
		}


		if (!RunGM.instance.b_Stop) 
		{
			if (!RunGM.instance.b_SpeedUp) {
				transform.Translate (Vector2.left * 1.5f * Time.deltaTime);
			} else {
				transform.Translate (Vector2.left * 1.5f*1.5f * Time.deltaTime);
			}
		}

		if (transform.localPosition.x <= -450f) 
		{
			if (RunGM.instance.Gs_Tile.Length >= 2) 
			{
				RunGM.instance.Gs_Tile [1].GetComponent<Run_Tile> ().first = true;
				Destroy (gameObject);
			}
		}
	}
}
