using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bullet : MonoBehaviour {
	public bool[]          b_Player_Bullet;

	public GameObject[]           gs_Enemys;
	public float[]                fs_EnemysDis;
	public float                  f_Min;
	public float                  f_moveFloat;
	public float                  f_ShootTime;

	public GameObject             g_BulletPos;
	public GameObject             g_MinEnemy;
	public GameObject             g_ChildImage;

	bool                          b_Shootbool;
	// Use this for initialization
	void Start () {
		StartCoroutine (StartShoot ());
	}
	
	// Update is called once per frame
	void Update () {
		if (b_Player_Bullet [0]) {
			transform.Translate (Vector2.up * 2 * Time.deltaTime);
		}else if (b_Player_Bullet[1]){
			g_ChildImage.transform.Rotate (0, 0, -15);
			transform.Translate (Vector2.up * 0.7f*Time.deltaTime);
		} else if (b_Player_Bullet [2]) {
			if (b_Shootbool) {
				if (gs_Enemys.Length > 0) {
					if (g_MinEnemy != null) {
						float dy = g_MinEnemy.transform.position.y - transform.position.y;
						float dx = g_MinEnemy.transform.position.x - transform.position.x;
						float rotateDegree = Mathf.Atan2 (dy, dx) * Mathf.Rad2Deg;
						transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, 0, rotateDegree), 4.5f * Time.deltaTime);

						this.transform.position = Vector2.MoveTowards (transform.position, g_MinEnemy.transform.position, 0.02f); 
					} else {
						transform.Translate (Vector2.right * 1.3f * Time.deltaTime);
					}
				} else {
					transform.Translate (Vector2.right * 1.2f * Time.deltaTime);
				}
			} else {
				transform.position = g_BulletPos.transform.position;
			}
		} else if (b_Player_Bullet [3]) {
			
		}

		if (transform.localPosition.x <= -395 || transform.localPosition.x >= 395 || transform.localPosition.y > 670 || transform.localPosition.y < -670) 
		{
			Destroy (this.gameObject);
		}
	}
	IEnumerator StartShoot()
	{
		yield return new WaitForSeconds (f_ShootTime);
		if (b_Player_Bullet [2])
		{
			gs_Enemys = GameObject.FindGameObjectsWithTag ("Enemy");

			if (gs_Enemys.Length > 0)
			{
				Array.Resize (ref fs_EnemysDis, gs_Enemys.Length);
				for (int i = 0; i < fs_EnemysDis.Length; i++) 
				{
					fs_EnemysDis [i] = Vector2.Distance (transform.position, gs_Enemys [i].transform.position);
				}

				f_Min = fs_EnemysDis [0];

				for (int i = 0; i < fs_EnemysDis.Length; i++) 
				{
					if (f_Min >= fs_EnemysDis [i])
					{
						f_Min = fs_EnemysDis [i];
						g_MinEnemy = gs_Enemys [i];
					}
				}
			}
		}
		b_Shootbool = true;
	}

	void OnTriggerEnter2D(Collider2D Coll)
	{
		if (Coll.gameObject.CompareTag ("EnemyBullet")) 
		{
			if (b_Player_Bullet [1])
				Destroy (Coll.gameObject);
		}
	}
}
