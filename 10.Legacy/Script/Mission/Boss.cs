using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Spine.Unity;

public class Boss : MonoBehaviour {
	SkeletonAnimation           Ani;
	float                       f_HP;
	int                         i_RandomPos;
	bool[]                      b_CorutineBool;
	bool                        b_BossStop=false;

	GameObject                  g_HPbar;
	GameObject                  g_BulletParent;
	GameObject                  g_Player;
	public GameObject           g_Bullet;
	public GameObject           g_Bullet2;
	public GameObject           g_Bullet3;

	public GameObject[]         gs_BulletPos;

	int                         ShootCount;

	// Use this for initialization
	void Start () {
		g_Player = GameObject.FindGameObjectWithTag ("Player");
		transform.localScale = new Vector2 (50, 50);
		Ani = this.gameObject.GetComponent<SkeletonAnimation> ();
		g_HPbar = GameObject.FindGameObjectWithTag ("GameController");
		g_BulletParent = GameObject.FindGameObjectWithTag ("Respawn");
		f_HP = 1;
		Array.Resize (ref b_CorutineBool, 3);
		StartCoroutine (BossPos ());
		StartCoroutine (AirPlane_Shoot ());
		StartCoroutine (Boss_Human_Drop ());
	}
	
	// Update is called once per frame
	void Update () {
		g_HPbar.GetComponent<UISlider>().value= f_HP;
		if (transform.localPosition.y > 175)
			transform.Translate (Vector2.down* Time.deltaTime);
		if(!b_BossStop)
		transform.localPosition = Vector2.MoveTowards (transform.localPosition, new Vector2(i_RandomPos,transform.localPosition.y), 300*Time.deltaTime);

		if (f_HP < 0.8f && b_CorutineBool [0].Equals (false)) 
		{
			b_CorutineBool [0] = true;
			StopAllCoroutines ();
			StartCoroutine (BossPos ());
			StartCoroutine (BossAni_transformer ());
			StartCoroutine (Boss_Human_Drop ());
		}

		if (f_HP < 0.4f && b_CorutineBool [1].Equals (false)) 
		{
			b_CorutineBool [1] = true;
			StopAllCoroutines ();
			StartCoroutine (BossPos ());
			StartCoroutine (BossAni_transformer2 ());
			StartCoroutine (Boss_Human_Drop ());
		}

		if (f_HP <= 0 && b_CorutineBool [2].Equals (false)) 
		{
			b_CorutineBool [2] = true;
			b_BossStop = true;
			StopAllCoroutines ();
			StartCoroutine (BossDie ());
		}
	}

	IEnumerator BossDie()
	{
		Ani.loop = false;
		Ani.AnimationName = "Bossdead";
		GetComponent<BoxCollider2D> ().enabled = false;
		MissionPlayer.instance.b_OutsideTouch = true;
		MissionPlayer.instance.b_Invisible = true;
		g_Player.GetComponent<BoxCollider2D> ().enabled = false;
		yield return new WaitForSeconds (5f);
		StartCoroutine (MissionGM.instance.Clear ());
	}
	//----------------------------------------------------------------------------------------------단계별 공격 패턴
	IEnumerator BossPattern2()
	{
		StartCoroutine (Attack ());
		yield return new WaitForSeconds (5);
		StartCoroutine (Attack ());
		yield return new WaitForSeconds (5);
		StartCoroutine (Charge ());
		yield return new WaitForSeconds (5);
		StartCoroutine (BossPattern2 ());
	}

	IEnumerator BossPattern3()
	{
		StartCoroutine (Attack2 ());
		yield return new WaitForSeconds (3);
		StartCoroutine (Attack2 ());
		yield return new WaitForSeconds (3);
		StartCoroutine (Charge2 ());
		yield return new WaitForSeconds (3);
		StartCoroutine (BossPattern3 ());
	}
	//-------------------------------------------------------------------------------------------------보스 랜덤 좌표
	IEnumerator BossPos()
	{
		yield return new WaitForSeconds (2.5f);
		i_RandomPos = UnityEngine.Random.Range (-240, 240);
		StartCoroutine (BossPos ());
	}
	//--------------------------------------------------------------------------------------------------보스 단계
	IEnumerator BossAni_transformer()
	{
		Ani.loop = false;
		Ani.AnimationName = "Bosstransform";
		yield return new WaitForSeconds (1f);
		Ani.loop = true;
		Ani.AnimationName = "Bossidle";
		yield return new WaitForSeconds (5);
		StartCoroutine (BossPattern2 ());
	}

	IEnumerator BossAni_transformer2()
	{
		Ani.loop = true;
		Ani.AnimationName = "Bossidlehit";
		StartCoroutine (BossPos ());
		yield return new WaitForSeconds (5);
		StartCoroutine (BossPattern3 ());
	}

	//--------------------------------------------------------------------------------------------------총알 패턴
	IEnumerator Boss_Human_Drop()
	{
		yield return new WaitForSeconds (13);
		StartCoroutine (MissionGM.instance.Human_Drop ());

		StartCoroutine (Boss_Human_Drop ());
	}

	IEnumerator DefaultAttack()
	{
		GameObject Bullet = Instantiate (g_Bullet, g_BulletParent.transform);
		Bullet.transform.position = gs_BulletPos[11].transform.position;
		Bullet.transform.rotation = Quaternion.Euler (0, 0, 15);
		GameObject Bullet1 = Instantiate (g_Bullet, g_BulletParent.transform);
		Bullet1.transform.position = gs_BulletPos[11].transform.position;
		Bullet1.transform.rotation = Quaternion.Euler (0, 0, 0);
		GameObject Bullet2 = Instantiate (g_Bullet, g_BulletParent.transform);
		Bullet2.transform.position = gs_BulletPos[11].transform.position;
		Bullet2.transform.rotation = Quaternion.Euler (0, 0, -15);

		GameObject Bullet3 = Instantiate (g_Bullet, g_BulletParent.transform);
		Bullet3.transform.position = gs_BulletPos[12].transform.position;
		Bullet3.transform.rotation = Quaternion.Euler (0, 0, 15);
		GameObject Bullet4 = Instantiate (g_Bullet, g_BulletParent.transform);
		Bullet4.transform.position = gs_BulletPos[12].transform.position;
		Bullet4.transform.rotation = Quaternion.Euler (0, 0, 0);
		GameObject Bullet5 = Instantiate (g_Bullet, g_BulletParent.transform);
		Bullet5.transform.position = gs_BulletPos[12].transform.position;
		Bullet5.transform.rotation = Quaternion.Euler (0, 0, -15);

		yield return new WaitForSeconds (1);

		GameObject Bullet6 = Instantiate (g_Bullet, g_BulletParent.transform);
		Bullet6.transform.position = gs_BulletPos[11].transform.position;
		Bullet6.transform.rotation = Quaternion.Euler (0, 0, 15);
		GameObject Bullet7 = Instantiate (g_Bullet, g_BulletParent.transform);
		Bullet7.transform.position = gs_BulletPos[11].transform.position;
		Bullet7.transform.rotation = Quaternion.Euler (0, 0, 0);
		GameObject Bullet8 = Instantiate (g_Bullet, g_BulletParent.transform);
		Bullet8.transform.position = gs_BulletPos[11].transform.position;
		Bullet8.transform.rotation = Quaternion.Euler (0, 0, -15);

		GameObject Bullet9 = Instantiate (g_Bullet, g_BulletParent.transform);
		Bullet9.transform.position = gs_BulletPos[12].transform.position;
		Bullet9.transform.rotation = Quaternion.Euler (0, 0, 15);
		GameObject Bullet10 = Instantiate (g_Bullet, g_BulletParent.transform);
		Bullet10.transform.position = gs_BulletPos[12].transform.position;
		Bullet10.transform.rotation = Quaternion.Euler (0, 0, 0);
		GameObject Bullet11 = Instantiate (g_Bullet, g_BulletParent.transform);
		Bullet11.transform.position = gs_BulletPos[12].transform.position;
		Bullet11.transform.rotation = Quaternion.Euler (0, 0, -15);

	}
	IEnumerator AirPlane_Shoot()
	{
		yield return new WaitForSeconds (2f);
		GameObject Bullet = Instantiate (g_Bullet, g_BulletParent.transform);
		Bullet.transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y - 30);
		Bullet.transform.rotation = Quaternion.Euler (0, 0, 15);
		GameObject Bullet1 = Instantiate (g_Bullet, g_BulletParent.transform);
		Bullet1.transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y - 30);
		Bullet1.transform.rotation = Quaternion.Euler (0, 0, 0);
		GameObject Bullet2 = Instantiate (g_Bullet, g_BulletParent.transform);
		Bullet2.transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y - 30);
		Bullet2.transform.rotation = Quaternion.Euler (0, 0, -15);
		StartCoroutine (AirPlane_Shoot ());
	}
	IEnumerator Shoot_Left()
	{
		yield return new WaitForSeconds (0.8f);
		for (int i = 0; i < 11; i++) 
		{
			GameObject Bullet = Instantiate (g_Bullet2, g_BulletParent.transform);
			Bullet.transform.position = gs_BulletPos[i].transform.position;
			Bullet.transform.rotation = gs_BulletPos [i].transform.rotation;
			yield return new WaitForSeconds (0.1f);
		}
	}

	IEnumerator Shoot_Left2()
	{
		yield return new WaitForSeconds (0.5f);
		for (int i = 0; i < 11; i++) 
		{
			GameObject Bullet = Instantiate (g_Bullet2, g_BulletParent.transform);
			Bullet.transform.position = gs_BulletPos[i].transform.position;
			Bullet.transform.rotation = gs_BulletPos [i].transform.rotation;
			yield return new WaitForSeconds (0.07f);
		}
	}
		
	IEnumerator Shoot_right()
	{
		yield return new WaitForSeconds (0.8f);
		for (int i = 10; i > -1; i--) 
		{
			GameObject Bullet = Instantiate (g_Bullet2, g_BulletParent.transform);
			Bullet.transform.position = gs_BulletPos[i].transform.position;
			Bullet.transform.rotation = gs_BulletPos [i].transform.rotation;
			yield return new WaitForSeconds (0.1f);
		}
	}

	IEnumerator Shoot_right2()
	{
		yield return new WaitForSeconds (0.5f);
		for (int i = 10; i > -1; i--) 
		{
			GameObject Bullet = Instantiate (g_Bullet2, g_BulletParent.transform);
			Bullet.transform.position = gs_BulletPos[i].transform.position;
			Bullet.transform.rotation = gs_BulletPos [i].transform.rotation;
			yield return new WaitForSeconds (0.07f);
		}
	}

	IEnumerator Charge_Shoot()
	{
		yield return new WaitForSeconds (1.7f);
		GameObject Bullet = Instantiate (g_Bullet3, g_BulletParent.transform);
		Bullet.transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y - 20);
		Bullet.transform.rotation = transform.rotation;
		yield return new WaitForSeconds (0.3f);
		GameObject Bullet1 = Instantiate (g_Bullet3, g_BulletParent.transform);
		Bullet1.transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y - 20);
		Bullet1.transform.rotation = transform.rotation;
	}

	IEnumerator Charge_Shoot2()
	{
		yield return new WaitForSeconds (1.1f);
		GameObject Bullet = Instantiate (g_Bullet3, g_BulletParent.transform);
		Bullet.transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y - 20);
		Bullet.transform.rotation = transform.rotation;
		yield return new WaitForSeconds (0.3f);
		GameObject Bullet1 = Instantiate (g_Bullet3, g_BulletParent.transform);
		Bullet1.transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y - 20);
		Bullet1.transform.rotation = transform.rotation;
	}


	//----------------------------------------------------------------------------------------------------------공격 명령
	IEnumerator Attack()
	{
		int AttackRandom = UnityEngine.Random.Range (0, 2);
		yield return new WaitForSeconds (0);
		if (AttackRandom == 0) {
			b_BossStop = true;
			Ani.loop = false;
			Ani.AnimationName = "Bossattackleft";
			StartCoroutine (Shoot_Left ());
		} else {
			b_BossStop = true;
			Ani.loop = false;
			Ani.AnimationName = "Bossattackright";
			StartCoroutine (Shoot_right ());
		}
		yield return new WaitForSeconds (2.75f);
		Ani.loop = true;
		Ani.AnimationName = "Bossidle";
		b_BossStop = false;
		StartCoroutine (DefaultAttack ());
	}

	IEnumerator Attack2()
	{
		int AttackRandom = UnityEngine.Random.Range (0, 2);
		yield return new WaitForSeconds (0);
		if (AttackRandom == 0) {
			b_BossStop = true;
			Ani.loop = false;
			Ani.timeScale = 1.5f;
			StartCoroutine (Shoot_Left2 ());
			Ani.AnimationName = "Bossattacklefthit";
		} else {
			b_BossStop = true;
			Ani.loop = false;
			Ani.timeScale = 1.5f;
			Ani.AnimationName = "Bossattackrighthit";
			StartCoroutine (Shoot_right2 ());
		}
		yield return new WaitForSeconds (2f);
		Ani.loop = true;
		Ani.timeScale = 1.5f;
		Ani.AnimationName = "Bossidlehit";
		b_BossStop = false;
		StartCoroutine (DefaultAttack ());
	}


	IEnumerator Charge()
	{
		b_BossStop = true;
		Ani.loop = false;
		Ani.AnimationName = "Bosscharge";
		StartCoroutine (Charge_Shoot ());
		yield return new WaitForSeconds (3.8f);
		Ani.loop = true;
		Ani.AnimationName = "Bossidle";
		b_BossStop = false;
		StartCoroutine (DefaultAttack ());
	}

	IEnumerator Charge2()
	{
		b_BossStop = true;
		Ani.loop = false;
		Ani.timeScale = 1.5f;
		Ani.AnimationName = "Bosschargehit";
		StartCoroutine (Charge_Shoot2 ());
		yield return new WaitForSeconds (2.3f);
		Ani.loop = true;
		Ani.timeScale = 1.5f;
		Ani.AnimationName = "Bossidlehit";
		b_BossStop = false;
		StartCoroutine (DefaultAttack ());
	}

	void OnTriggerEnter2D(Collider2D Coll)
	{
		if (Coll.gameObject.GetComponent<Bullet> ()) 
		{
			f_HP-=0.003f/MissionGM.instance.f_BossHP;
			if(!Coll.GetComponent<Bullet>().b_Player_Bullet[1])
			Destroy (Coll.gameObject);
		}
		
	}
}
