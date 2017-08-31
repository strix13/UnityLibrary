using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour {

	public static EnemyMove instance;

	public bool          b_Black_Hole,b_Worm_Hole,b_Satellite,b_Drone,b_Land_Mine,b_Meteo,b_Trash,b_SpaceChamber;
	public bool          b_Move;
	public bool          b_DeathBool;
	public bool          b_Stop=false;
	public bool          b_Drone_Color,b_Turret,b_Turret_Color,b_Human;
	public bool          b_Down;
	bool                 b_Meteo_Move;
	bool                 b_Death=false;//-------Death코루틴 호출할 때 예외처리용 

	public bool[]        b_Path;
	public Vector2[]     v2_Path_Point;

	int                  i_Path_Num=0;
	int                  i_TimeControl=0;
	float                f_hp=1;
	float                f_hp_Minus=0;

	public float         f_X;
	public float         f_Y;
	public float         f_Speed=0.5f;
	float                f_Path_Dis=0;

	public GameObject    g_HP;
	public GameObject    g_Bullet;
	public GameObject    g_Meteo_Effect;
	public GameObject    g_Boomb;
	public GameObject    g_Coin, g_Roulette, g_fuel;
	GameObject           g_Effect_Parent;
	GameObject           g_Item_Parent;
	LineRenderer         L_Line;
	public Coroutine     Stop;

	void Awake()
	{
		instance = this;
		g_Item_Parent = GameObject.FindGameObjectWithTag ("Respawn");
		g_Effect_Parent = GameObject.FindGameObjectWithTag ("MainCamera");
		if (b_Black_Hole) {
			transform.localScale = new Vector3 (100, 100, 100);
			f_Speed = 200;
			f_hp_Minus = 50;
		} else if (b_Worm_Hole) {
			transform.localScale = new Vector3 (150, 150, 150);
			f_Speed = 200;
			f_hp_Minus = 50;
		} else if (b_Drone) {
			transform.localScale = new Vector3 (20, 20, 20);
			f_Speed = 400;
			f_hp_Minus = 0.34f;
		} else if (b_Satellite) {
			transform.localScale = new Vector3 (80, 80, 80);
			f_Speed = 500;
			f_hp_Minus = 0.34f;
		} else if (b_Land_Mine) {
			transform.localScale = new Vector3 (20, 20, 20);
			f_Speed = 400;
			f_hp_Minus = 0.5f;
		} else if (b_Meteo) {
			transform.localScale = new Vector3 (150, 150, 150);
			L_Line = GetComponent<LineRenderer> ();
			f_hp_Minus = 5000;
		} else if (b_SpaceChamber) {
			transform.localScale = new Vector3 (77, 77, 77);
			f_hp_Minus = 0.1f;
		} else if (b_Drone_Color) {
			transform.localScale = new Vector3 (50, 50, 50);
			f_Speed = 100;
			f_hp_Minus = 0.2f;
			Stop = StartCoroutine (Shoot ());
		} else if (b_Turret) {
			transform.localScale = new Vector3 (50, 50, 50);
			f_Speed = 150;
			f_hp_Minus = 0.2f;
			Stop = StartCoroutine (Shoot ());
		} else if (b_Turret_Color) {
			transform.localScale = new Vector3 (50, 50, 50);
			f_Speed = 150;
			f_hp_Minus = 0.2f;
			Stop = StartCoroutine (Shoot ());
		} else if (b_Human) {
			transform.localScale = new Vector3 (50, 50, 50);
			f_Speed = 70;
			f_hp_Minus = 0.2f;
			Stop = StartCoroutine (Shoot ());
		}
	}

	// Use this for initialization
	void Start () {
		if (b_Meteo) 
		{
			L_Line.SetColors(Color.red, Color.red);
			L_Line.SetWidth(0.005f, 0.005f);
			L_Line.SetPosition (0, transform.position);
			L_Line.SetPosition (1, new Vector2(transform.position.x,-1000));
			StartCoroutine (Meteo_Drop ());
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (MissionRoulette.instance.b_Roulette) {
			b_Stop = true;
		} else {
			b_Stop = false;
		}

		if ((transform.localPosition.x > -f_X && transform.localPosition.x < f_X) && (transform.localPosition.y > -f_Y && transform.localPosition.y < f_Y)) {
			gameObject.tag = "Enemy";
		} else {
			gameObject.tag = "Untagged";
		}

		if(g_HP != null)
		g_HP.GetComponent<UISlider> ().value = f_hp;
		
		if (!b_Stop) {
			if (b_Move) {//----------------------------------------------waypoint
				if (i_Path_Num < b_Path.Length) {
					if (b_Path [i_Path_Num]) {
						f_Path_Dis = Vector2.Distance (transform.localPosition, v2_Path_Point [i_Path_Num]);
						transform.localPosition = Vector2.MoveTowards (transform.localPosition, v2_Path_Point [i_Path_Num], f_Speed * Time.deltaTime);
					}

					if (f_Path_Dis <= 10 && b_Path [i_Path_Num]) {
						b_Path [i_Path_Num] = false;
						i_Path_Num++;
						if (i_Path_Num < b_Path.Length)
							b_Path [i_Path_Num] = true;
					}
				}
			}

			if (b_SpaceChamber || b_Worm_Hole || b_Black_Hole) {
				transform.Translate (Vector2.down * 0.4f * Time.deltaTime);
			}

			if (b_Meteo_Move) {
				L_Line.SetPosition (0, transform.position);
				L_Line.SetPosition (1, new Vector2 (transform.position.x, -1000));
				transform.Translate (Vector2.down * 1.5f * Time.deltaTime);
			}

			if (f_hp <= 0 && !b_Death) {
				b_Death = true;
				if (!b_Trash) {
					if(MissionGM.instance.f_RealTime>0)
					StartCoroutine (ItemDrop ());
				}
				if(!b_DeathBool)
				StartCoroutine (Death ());
			}
		}
	}

	public IEnumerator Meteo_Drop()
	{
		yield return new WaitForSeconds (1.5f);
		GameObject Effect = Instantiate (g_Meteo_Effect, g_Effect_Parent.transform)as GameObject;
		Effect.transform.localScale = new Vector2 (150, 150);
		Effect.transform.localPosition = new Vector2 (transform.localPosition.x, 400);
		Destroy (Effect.gameObject, 0.5f);
		yield return new WaitForSeconds (1f);
		b_Meteo_Move = true;
	}

	IEnumerator ItemDrop()
	{
		yield return new WaitForSeconds (0f);
		int Drop_Random = Random.Range (1, 101);
		if (Drop_Random <= 73) {
			GameObject Item = Instantiate (g_Coin, g_Item_Parent.transform)as GameObject;
			Item.transform.localPosition = transform.localPosition;
		} else if (Drop_Random <= 75) {
			GameObject Item = Instantiate (g_Roulette, g_Item_Parent.transform)as GameObject;
			Item.transform.localPosition = transform.localPosition;
		} else if (Drop_Random <= 80) {
			GameObject Item = Instantiate (g_fuel, g_Item_Parent.transform)as GameObject;
			Item.transform.localPosition = transform.localPosition;
		}
	}

	IEnumerator Shoot()
	{
		float ShootTime=0;
		if (b_Drone_Color) {
			ShootTime = 1.5f;
		} else if (b_Turret) {
			ShootTime = 1.5f;
		} else if (b_Turret_Color) {
			ShootTime = 1.5f;
		} else if (b_Human) {
			ShootTime = 1f;
		}
		if (i_TimeControl == 0)
			yield return new WaitForSeconds (4f);
		i_TimeControl++;
		GameObject Bullet = Instantiate (g_Bullet, g_Item_Parent.transform)as GameObject;
		Bullet.transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y - 20);
		yield return new WaitForSeconds (ShootTime);
		Stop = StartCoroutine (Shoot ());
	}

	public IEnumerator Death()
	{
		yield return new WaitForSeconds (0f);
		MissionGM.instance.i_Score += 50;
		b_DeathBool = true;
		GameObject Boom = Instantiate (g_Boomb, g_Effect_Parent.transform)as GameObject;
		Boom.transform.localPosition = transform.localPosition;
		Boom.transform.localScale = new Vector2 (82, 82);
		Destroy (gameObject);
	}

	void OnTriggerEnter2D(Collider2D Coll)
	{
		Bullet ColGet = Coll.gameObject.GetComponent<Bullet> ();

		if (ColGet) 
		{
			if (b_Drone || b_Land_Mine || b_Satellite || b_SpaceChamber || b_Drone_Color || b_Turret || b_Turret_Color || b_Human ) {
				f_hp-=f_hp_Minus;
				if(!Coll.GetComponent<Bullet>().b_Player_Bullet[1])
				Destroy (Coll.gameObject);
			}
		}
	}


}
