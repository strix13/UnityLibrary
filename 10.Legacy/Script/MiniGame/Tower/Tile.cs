using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Tile : MonoBehaviour {
	SkeletonAnimation              Mine;
	bool                           b_TileStatic;
	void Awake(){
		Mine = GetComponent<SkeletonAnimation> ();
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	void SpineAni(float AniTime, bool Loop, string AniName)
	{		
		Mine.timeScale = AniTime;
		Mine.loop = Loop;
		Mine.AnimationName = AniName;
	}

	void OnCollisionEnter2D(Collision2D Coll)
	{
		if (Coll.gameObject) 
		{
			SpineAni (1, true,"boxlight");
			if(!b_TileStatic)
			{
				b_TileStatic = true;
				GetComponent<Rigidbody2D> ().bodyType = RigidbodyType2D.Static;
				if (TowerGM.instance.g_beforeTile != null) {
					if (Mathf.Abs (transform.localPosition.x-Coll.transform.localPosition.x) >= 75) {
						if(TowerGM.instance.f_fever!=0)
						TowerGM.instance.f_fever -= 0.15f;
						if (transform.localPosition.x > Coll.transform.localPosition.x) {
							SpineAni (1, false,"boxmissdropr");
						} else if (transform.localPosition.x < Coll.transform.localPosition.x) {
							SpineAni (1, false,"boxmissdropl");
						}
						Destroy (this.gameObject.GetComponent<Rigidbody2D> ());
						gameObject.GetComponent<BoxCollider2D> ().enabled = false;
					} else {
						if (!TowerGM.instance.b_fever)
							TowerGM.instance.i_Score += 100;
						else
							TowerGM.instance.i_Score += 500;

						if (!TowerGM.instance.b_fever) 
						{
							TowerGM.instance.f_fever += 0.15f;
						}
						TowerGM.instance.i_Tile_Num++;
						TowerGM.instance.b_CamMove = false;
					}
				} else {
					if (!TowerGM.instance.b_fever)
						TowerGM.instance.i_Score += 100;
					else
						TowerGM.instance.i_Score += 500;

					if (!TowerGM.instance.b_fever) 
					{
						TowerGM.instance.f_fever += 0.15f;
					}
					TowerGM.instance.i_Tile_Num++;
					TowerGM.instance.b_CamMove = false;
				}
			}
		}
	}
}