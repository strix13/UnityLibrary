using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class MissionPlayer : MonoBehaviour {
	public static MissionPlayer                  instance;

	public Animator                              Ani;
	public GameObject                            g_Enemy=null;
	public GameObject                            g_Black;
	public GameObject                            g_BulletParent;
	public GameObject                            g_Shield;
	public GameObject                            g_Bullet;
	public GameObject                            g_Bullet_Serve;
	public GameObject[]                          gs_PlayerHP;
	public GameObject[]                          gs_PlayerPower;
	public GameObject[]                          gs_AttackPoint;

	public bool                                  b_Collider = false;
	public bool                                  b_EnemyStop=false;
	public bool                                  b_StrongTime = false;
	public bool                                  b_feverTime = false;
	public bool                                  b_Invisible = false;
	public bool                                  b_OutsideTouch = true;
	public bool                                  b_Touch=false;
	bool                                         b_Shoot=false;
	bool                                         b_BeganTouch=false;
	bool                                         b_ShootTime;

	public float                                 f_Speed = 5;
	public float                                 f_Shooting;
	public float                                 f_Shooted;
	float                                        f_half_Width;

	int                                          i_HP=3;
	public int                                   i_PowerLevel;

	Vector2                                      v2_PlayerPos;
	Vector2                                      v2_TouchStartPos;

	Coroutine                                    Stop;
	public Coroutine                             FuelStop;

	void Awake()
	{
		instance = this;
		f_half_Width = Screen.width * 5f;
	}
	// Use this for initialization
	void Start () {
		FuelStop = StartCoroutine (PowerTime ());
		i_HP = 3;
		i_PowerLevel = PCManagerFramework.p_pDataGame.iPowerLevel;
	}

	// Update is called once per frame
	void Update () {
		if (b_Invisible) {
			b_Collider = true;
		} else {
			b_Collider = false;
		}
			
		if (b_ShootTime) {
			f_Shooting += Time.deltaTime;
		} else {
			f_Shooting = 0;
		}

		if (f_Shooting >= f_Shooted) 
		{
			b_ShootTime = false;
		}

		if (Input.touchCount == 1) {
			if (!b_OutsideTouch) {
				if (!b_Touch) {
					Touch touch = Input.GetTouch (0);

					if (touch.phase == TouchPhase.Began) {	
						b_BeganTouch = true;
						v2_PlayerPos = transform.position;
						v2_TouchStartPos = Camera.main.ScreenToWorldPoint (touch.position);
						if (!b_Shoot) {
							b_Shoot = true;
							if(!b_ShootTime)
							Stop = StartCoroutine (Shoot ());
						}
					} else if (touch.phase == TouchPhase.Moved) {
						if (b_BeganTouch) {
							float TouchingX = Camera.main.ScreenToWorldPoint (touch.position).x - v2_TouchStartPos.x;
							float TouchingY = Camera.main.ScreenToWorldPoint (touch.position).y - v2_TouchStartPos.y;
							transform.position = new Vector2 (v2_PlayerPos.x + TouchingX, v2_PlayerPos.y + TouchingY);
						}
					} else if (touch.phase == TouchPhase.Ended) {
						if (b_Shoot)
							StopCoroutine (Stop);
						b_Shoot = false;
					}
				} else if (Input.touchCount > 1) {
					b_Touch = true;
				} else if (Input.touchCount == 0) {
					b_Touch = false;
					if (b_Shoot)
						StopCoroutine (Stop);
					b_Shoot = false;
				}
			} else {
				if (b_Shoot)
					StopCoroutine (Stop);
				b_Shoot = false;
			}
		}

		if (transform.localPosition.x <= -360) 
		{
			transform.localPosition = new Vector2(-360,transform.localPosition.y);
		}

		if (transform.localPosition.x >= 360) 
		{
			transform.localPosition = new Vector2(360,transform.localPosition.y);
		}

		if (transform.localPosition.y <= -640) 
		{
			transform.localPosition = new Vector2 (transform.localPosition.x, -640);
		}

		if (transform.localPosition.y >= 640) 
		{
			transform.localPosition = new Vector2 (transform.localPosition.x, 640);
		}
	}


	public IEnumerator PowerTime()
	{
		yield return new WaitForSeconds (7f);

		Color Clr=gs_PlayerPower [PCManagerFramework.p_pDataGame.iFuelNum - 1].GetComponent<UISprite> ().color;
		for (int i = 0; i < 3; i++) 
		{
			yield return new WaitForSeconds (0.25f);
			Clr.a = 0.5f;
			gs_PlayerPower [PCManagerFramework.p_pDataGame.iFuelNum - 1].GetComponent<UISprite> ().color = Clr;
			yield return new WaitForSeconds (0.25f);
			Clr.a = 1f;
			gs_PlayerPower [PCManagerFramework.p_pDataGame.iFuelNum - 1].GetComponent<UISprite> ().color = Clr;
			yield return new WaitForSeconds (0.25f);
			Clr.a = 0.5f;
			gs_PlayerPower [PCManagerFramework.p_pDataGame.iFuelNum - 1].GetComponent<UISprite> ().color = Clr;
			yield return new WaitForSeconds (0.25f);
			Clr.a = 1f;
			gs_PlayerPower [PCManagerFramework.p_pDataGame.iFuelNum - 1].GetComponent<UISprite> ().color = Clr;
		}
		gs_PlayerPower [PCManagerFramework.p_pDataGame.iFuelNum - 1].SetActive (false);
		PCManagerFramework.p_pDataGame.iFuelNum--;
		if (PCManagerFramework.p_pDataGame.iFuelNum > 0) 
		{
			FuelStop = StartCoroutine (PowerTime ());
		}
	}

	IEnumerator Player1_Serve(){
		for (int i = 0; i < 2; i++)
		{
			GameObject Bullet = Instantiate (g_Bullet_Serve, g_BulletParent.transform)as GameObject;
			Bullet.transform.localScale = new Vector3 (1, 1, 1);
			Bullet.transform.localPosition = new Vector2 (transform.localPosition.x - 100, transform.localPosition.y + 185f);
			GameObject Bullet1 = Instantiate (g_Bullet_Serve, g_BulletParent.transform)as GameObject;
			Bullet1.transform.localScale = new Vector3 (1, 1, 1);
			Bullet1.transform.localPosition = new Vector2 (transform.localPosition.x + 100, transform.localPosition.y + 185f);
			yield return new WaitForSeconds (0.05f);
		}
	}

	IEnumerator Player1_Serve2(){
		for (int i = 0; i < 2; i++)
		{
			GameObject Bullet = Instantiate (g_Bullet_Serve, g_BulletParent.transform)as GameObject;
			Bullet.transform.localScale = new Vector3 (1, 1, 1);
			Bullet.transform.localPosition = new Vector2 (transform.localPosition.x - 125, transform.localPosition.y + 135f);
			GameObject Bullet1 = Instantiate (g_Bullet_Serve, g_BulletParent.transform)as GameObject;
			Bullet1.transform.localScale = new Vector3 (1, 1, 1);
			Bullet1.transform.localPosition = new Vector2 (transform.localPosition.x + 125, transform.localPosition.y + 135f);
			yield return new WaitForSeconds (0.05f);
		}
	}

	IEnumerator Player2_Serve(){
		yield return new WaitForSeconds (0f);
		GameObject Bullet = Instantiate (g_Bullet_Serve, g_BulletParent.transform)as GameObject;
		Bullet.transform.localScale = new Vector3 (1, 1, 1);
		Bullet.transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y + 135f);
	}

	IEnumerator Player3_Serve(){
		yield return new WaitForSeconds (0f);
		GameObject Bullet = Instantiate (g_Bullet_Serve, g_BulletParent.transform)as GameObject;
		Bullet.transform.localScale = new Vector3 (1, 1, 1);
		Bullet.transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y + 135f);
	}

	IEnumerator Shoot()
	{
		b_ShootTime = true;
		if (PCManagerFramework.p_pDataGame.eCharacterCurrent == 0 || PCManagerFramework.p_pDataGame.eCharacterCurrent == ECharacterName.kkyung) {
			float ObjectScale = 0;
			if (i_PowerLevel == 0) {
				ObjectScale = 1;
			} else if (i_PowerLevel == 1) {
				ObjectScale = 1.5f;
			} else if (i_PowerLevel == 2) {
				ObjectScale = 1.5f;
				StartCoroutine (Player1_Serve ());
			} else if (i_PowerLevel == 3) {
				ObjectScale = 1.5f;
				StartCoroutine (Player1_Serve ());
				StartCoroutine (Player1_Serve2 ());
			}

			GameObject Bullet = Instantiate (g_Bullet, g_BulletParent.transform)as GameObject;
			Bullet.transform.localScale = new Vector3 (ObjectScale, ObjectScale, ObjectScale);
			Bullet.transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y + 135f);
			yield return new WaitForSeconds (0.1f);
			Stop = StartCoroutine (Shoot ());
		} else if (PCManagerFramework.p_pDataGame.eCharacterCurrent == ECharacterName.milia) {
			float ObjectScale = 0;
			string ObjectName = null;
			if (i_PowerLevel == 0) {
				ObjectScale = 1;
				ObjectName = "4Player_1";
			} else if (i_PowerLevel == 1) {
				ObjectScale = 1.5f;
				ObjectName = "4Player_1";
			} else if (i_PowerLevel == 2) {
				ObjectScale = 2;
				ObjectName = "4Player_2";
			}  else if (i_PowerLevel == 3) {
				ObjectScale = 2;
				ObjectName = "4Player_2";
				StartCoroutine (Player2_Serve ());
			}
			GameObject Bullet = Instantiate (g_Bullet, g_BulletParent.transform)as GameObject;	
			Bullet.transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y + 135f);
			Bullet.GetComponent<Bullet> ().g_ChildImage.GetComponent<UISprite> ().name = ObjectName;
			Bullet.transform.localScale = new Vector3 (ObjectScale, ObjectScale, ObjectScale);
			yield return new WaitForSeconds (0.3f);
			Stop = StartCoroutine (Shoot ());
		} else if (PCManagerFramework.p_pDataGame.eCharacterCurrent == ECharacterName.Yuna) {
			int Bullets = 0;
			if (i_PowerLevel == 0) {
				Bullets = 3;
			} else if (i_PowerLevel == 1) {
				Bullets = 5;
			} else if (i_PowerLevel == 2) {
				Bullets = 8;
			} else if (i_PowerLevel == 3) {
				Bullets = 8;
				StartCoroutine (Player3_Serve ());
			}

			for (int i = 0; i < Bullets; i++) 
			{
				GameObject Bullet = Instantiate (g_Bullet, g_BulletParent.transform)as GameObject;
				Bullet.transform.localScale = new Vector3 (0.4f, 0.4f, 0.4f);
				Bullet.transform.position = gs_AttackPoint [i].transform.position;
				Bullet.transform.rotation = gs_AttackPoint [i].transform.rotation;
				Bullet.GetComponent<Bullet> ().f_ShootTime = i * 0.05f;
				Bullet.GetComponent<Bullet> ().g_BulletPos = gs_AttackPoint[i];
			}
			yield return new WaitForSeconds (0.4f);
			Stop = StartCoroutine (Shoot ());
		} else if (PCManagerFramework.p_pDataGame.eCharacterCurrent == ECharacterName.zion) {
			int Bullets = 0;
			float ObjectScale = 0;
			float ObjectSpeed = 0;
			GameObject ObjectName = null;
			if (i_PowerLevel == 0) {
				Bullets = 3;
				ObjectScale = 1;
				ObjectName = g_Bullet;
				ObjectSpeed = 0.2f;
			} else if (i_PowerLevel == 1) {
				Bullets = 5;
				ObjectScale = 1;
				ObjectName = g_Bullet;
				ObjectSpeed = 0.2f;
			} else if (i_PowerLevel == 2) {
				Bullets = 5;
				ObjectScale = 1.5f;
				ObjectName = g_Bullet_Serve;
				ObjectSpeed = 0.2f;
			} else if (i_PowerLevel == 3) {
				Bullets = 5;
				ObjectScale = 1.5f;
				ObjectName = g_Bullet_Serve;
				ObjectSpeed = 0.1f;
			}
			for (int i = 0; i < Bullets; i++) 
			{
				GameObject Bullet = Instantiate (ObjectName, g_BulletParent.transform)as GameObject;
				Bullet.transform.localScale = new Vector3 (ObjectScale, ObjectScale, ObjectScale);
				Bullet.transform.position = gs_AttackPoint [i].transform.position;
				Bullet.transform.rotation = gs_AttackPoint [i].transform.rotation;
				Bullet.GetComponent<Bullet> ().f_ShootTime = i * 0.1f;
				Bullet.GetComponent<Bullet> ().g_BulletPos = gs_AttackPoint[i];
			}
			yield return new WaitForSeconds (ObjectSpeed);
			Stop = StartCoroutine (Shoot ());
		}
	}

	void OnTriggerEnter2D(Collider2D Col)
	{
		if (!b_feverTime) {
			if (!b_StrongTime) {	
				if (!b_Collider) {
					if (Col.gameObject.CompareTag ("Enemy") || Col.gameObject.CompareTag ("EnemyBullet")) {
						g_Shield.SetActive (true);
						b_StrongTime = true;	
						StartCoroutine (MissionGM.instance.Behit ());
						Handheld.Vibrate ();
						i_HP--;
						if (i_HP == 2) {
							gs_PlayerHP [2].SetActive (false);
						} else if (i_HP == 1) {
							gs_PlayerHP [1].SetActive (false);
						} else if (i_HP == 0) {
							gs_PlayerHP [0].SetActive (false);
						} else if (i_HP == -1) {
							MissionGM.instance.Over ();
						}
					}
				} 

				if (Col.gameObject.GetComponent<EnemyMove> ()) {
					EnemyMove ColGet = Col.gameObject.GetComponent<EnemyMove> ();
					if (ColGet.b_Black_Hole || ColGet.b_Worm_Hole) {
						Time.timeScale = 1;
						PCManagerFramework.DoLoadScene_FadeInOut( ESceneName.OutGame, 1f, Color.black );
					}
				}
			}
		} else {
			if (Col.gameObject.CompareTag ("Enemy")) {
				if(!Col.gameObject.GetComponent<EnemyMove>().b_DeathBool)
				StartCoroutine (Col.gameObject.GetComponent<EnemyMove> ().Death ());
			} else if (Col.gameObject.CompareTag ("EnemyBullet")) {
				Destroy (Col.gameObject);
			}
		}

		if (Col.gameObject.GetComponent<Mission_Item> ())
		{
			if (Col.gameObject.GetComponent<Mission_Item> ().b_Coin.Equals (true)) {
				PCManagerFramework.p_pDataGame.iGold+=10;
				MissionGM.instance.i_Score += 100;

				PCManagerFramework.p_pManagerSound.DoPlaySoundEffect( ESoundName.CoinSound );

				Destroy (Col.gameObject);
			} else if (Col.gameObject.GetComponent<Mission_Item> ().b_Roulette.Equals (true)) {
				MissionGM.instance.StopAllCoroutines ();
				MissionGM.instance.i_Score += 100;
				MissionGM.instance.StartCoroutine (MissionGM.instance.MonObject_Stop ());
				MissionRoulette.instance.b_Slide = true;
				Destroy (Col.gameObject);
			} else if (Col.gameObject.GetComponent<Mission_Item> ().b_fuel.Equals (true)) {
				StopCoroutine (FuelStop);	
				MissionGM.instance.i_Score += 100;
				Color Alp = MissionPlayer.instance.gs_PlayerPower [PCManagerFramework.p_pDataGame.iFuelNum - 1].GetComponent<UISprite> ().color;
				Alp.a = 1;
				MissionPlayer.instance.gs_PlayerPower [PCManagerFramework.p_pDataGame.iFuelNum - 1].GetComponent<UISprite> ().color = Alp;
				if (PCManagerFramework.p_pDataGame.iFuelNum < 8)
				{
					MissionPlayer.instance.gs_PlayerPower [PCManagerFramework.p_pDataGame.iFuelNum].SetActive (true);
					PCManagerFramework.p_pDataGame.iFuelNum++;
				}
				FuelStop = StartCoroutine (PowerTime ());
				Destroy (Col.gameObject);
			}
		}
	}
}
