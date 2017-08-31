using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour {
	public bool b_Drone_Color_Bullet,b_Human_Bullet,b_Turret_Bullet,b_Drone_Bullet,b_Boss_Bullet_1,b_Boss_Bullet_2;
	public bool b_Down;
	float       f_speed;
	Vector3     v3_PlayerPos;
	Vector3     v3_MinePos;
	GameObject  g_Player;
	// Use this for initialization
	void Start () {
		g_Player = GameObject.FindGameObjectWithTag ("Player");
		if(g_Player != null)
		v3_PlayerPos = g_Player.transform.position;
		
		v3_MinePos = transform.position;
		if (b_Drone_Color_Bullet) {
			transform.localScale = new Vector2 (0.1f, 0.1f);
			f_speed = 0.5f;
		} else if (b_Human_Bullet) {
			transform.localScale = new Vector2 (0.4f, 0.4f);
			f_speed = 0.5f;
		} else if (b_Turret_Bullet) {
			transform.localScale = new Vector2 (0.25f, 0.25f);
			f_speed = 0.5f;
		} else if (b_Drone_Bullet) {
			transform.localScale = new Vector2 (0.25f, 0.25f);
			f_speed = 0.5f;
		} else if (b_Boss_Bullet_1) {
			transform.localScale = new Vector2 (0.08f, 0.08f);
			f_speed = 0.5f;
		} else if (b_Boss_Bullet_2) {
			transform.localScale = new Vector2 (0.5f, 0.5f);
			f_speed = 3.5f;
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (!MissionRoulette.instance.b_Roulette)
		{
			if (b_Down)
				transform.Translate (Vector2.down * Time.deltaTime);
			if (transform.localPosition.x <= -395 || transform.localPosition.x >= 395 || transform.localPosition.y > 670 || transform.localPosition.y < -670) {
				Destroy (this.gameObject);
			}
			if (b_Drone_Color_Bullet) {
				Vector3 vec3dir = (v3_PlayerPos - v3_MinePos) * 300000;
				transform.Translate (vec3dir * 0.000001f * Time.deltaTime);
			} else {
				transform.Translate (Vector2.down * f_speed * Time.deltaTime);
			}
		}
	}
}
