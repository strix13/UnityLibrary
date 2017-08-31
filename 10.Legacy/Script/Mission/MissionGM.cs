using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MissionGM : MonoBehaviour
{

	public static MissionGM instance;

	bool b_Stop = false;//---------------일시정지 상태인지 아닌지 구분하기위한 bool값
	bool b_Startbool = false;
	bool b_victory = false;
	float exe = 0.3f;
	int i_MonZen = 0;

	public int i_Coin = 0;
	public int i_Ticket = 0;
	public int i_Score;
	public bool b_SkySpeed = false;

	public GameObject g_EXE_bar;
	public GameObject g_BossHP_Bar;
	public GameObject g_Start;//--------------------씬이 시작할때 등장하는 스타트 텍스트
	public GameObject g_Stop, g_RealStop;//----------스톱씬 
	public GameObject g_BackGround, g_Earth;//-------하늘 , 연출용 지구
	public GameObject g_Player, g_Player1, g_Player2, g_Player3;//-------------------플레이어
	public GameObject g_Drone, g_LandMine;//---------Enemy 기체
	public GameObject g_Black_Hole, g_Worm_Hole, g_Satellite;
	public GameObject g_Meteo_1, g_Meteo_2, g_Meteo_3;
	public GameObject g_SpaceChamber, g_SpaceChamber2;
	public GameObject g_ObjectParent;//-------------Enemy생성할때 최상위 오브젝트
	public GameObject g_MissionScene, g_ClearScene, g_OverScene;
	public GameObject g_Damage;//-------------------BeHit 이펙트
	public GameObject g_Boss;
	public GameObject g_Warning;
	public GameObject[] gs_Human;
	public GameObject[] gs_Drone;
	public GameObject[] gs_Turret_Color;
	public GameObject[] gs_Turret;

	public UILabel Lb_Ticket;
	public UILabel Lb_Coin;
	public UILabel Lb_Score;
	public UILabel Lb_HighScore;
	public UILabel Lb_M;

	public float f_RealTime;
	public bool b_Timebool = false;
	public bool b_RouletteTime = false;
	public float f_BossHP;

	bool b_10Luna = false;
	bool b_BossDrop = false;
	bool b_CoroutineStop = false;
	bool b_Exception = false;
	bool b_FuelZero = false;
	void Awake()
	{
		instance = this;
		Screen.orientation = ScreenOrientation.Portrait;
		StartCoroutine( StartAni() );
		b_BossDrop = false;
	}

	// Use this for initialization
	void Start()
	{
		if (PCManagerFramework.p_pDataGame.eCharacterCurrent == ECharacterName.reina)
			g_Player.SetActive( true );
		else if (PCManagerFramework.p_pDataGame.eCharacterCurrent == ECharacterName.milia)
			g_Player1.SetActive( true );
		else if (PCManagerFramework.p_pDataGame.eCharacterCurrent == ECharacterName.Yuna)
			g_Player2.SetActive( true );
		else if (PCManagerFramework.p_pDataGame.eCharacterCurrent == ECharacterName.zion)
			g_Player3.SetActive( true );
		else if (PCManagerFramework.p_pDataGame.eCharacterCurrent == ECharacterName.kkyung)
			g_Player.SetActive( true );
	}

	// Update is called once per frame
	void Update()
	{
		Lb_HighScore.text = i_Score.ToString();
		Lb_Score.text = i_Score.ToString();
		g_EXE_bar.GetComponent<UISlider>().value = exe;

		if (!b_10Luna)
		{
			if (f_RealTime <= 10)
			{
				b_10Luna = true;
				StopAllCoroutines();
				GameObject[] Enemys = GameObject.FindGameObjectsWithTag( "Enemy" );
				for (int i = 0; i < Enemys.Length; i++)
				{
					if (Enemys[i].GetComponent<EnemyMove>())
					{
						if (!Enemys[i].GetComponent<EnemyMove>().b_DeathBool)
							StartCoroutine( Enemys[i].GetComponent<EnemyMove>().Death() );
					}
				}

				GameObject[] Items = GameObject.FindGameObjectsWithTag( "Item" );
				for (int i = 0; i < Items.Length; i++)
				{
					if (Items[i].GetComponent<Mission_Item>())
					{
						StartCoroutine( Items[i].GetComponent<Mission_Item>().Death() );
					}
				}
			}
		}

		if (!b_FuelZero)
		{
			if (PCManagerFramework.p_pDataGame.iFuelNum <= 0)
			{
				b_FuelZero = true;
				Over();
			}
		}
		if (!b_RouletteTime)
		{
			if (!b_Exception)
			{
				if (f_RealTime <= 0)
				{
					b_Exception = true;
					f_RealTime = 0;
					b_CoroutineStop = true;
					b_Timebool = true;
				}
			}

			if (!b_Timebool)
			{
				f_RealTime -= 3.3f * Time.deltaTime;
			}

			Lb_M.text = f_RealTime.ToString( "N0" );
		}



		if (b_CoroutineStop)
		{
			b_CoroutineStop = false;
			StopAllCoroutines();
			GameObject[] Enemys = GameObject.FindGameObjectsWithTag( "Enemy" );
			for (int i = 0; i < Enemys.Length; i++)
			{
				if (Enemys[i].GetComponent<EnemyMove>())
				{
					if (!Enemys[i].GetComponent<EnemyMove>().b_DeathBool)
						StartCoroutine( Enemys[i].GetComponent<EnemyMove>().Death() );
				}
			}
			StartCoroutine( Boss_Drop() );
		}
		if (b_victory)
		{
			if (PCManagerFramework.p_pDataGame.eCharacterCurrent == ECharacterName.reina)
			{
				g_Player.transform.Translate( Vector2.up * 2 * Time.deltaTime );
			}
			else if (PCManagerFramework.p_pDataGame.eCharacterCurrent == ECharacterName.milia)
			{
				g_Player1.transform.Translate( Vector2.up * 2 * Time.deltaTime );
			}
			else if (PCManagerFramework.p_pDataGame.eCharacterCurrent == ECharacterName.Yuna)
			{
				g_Player2.transform.Translate( Vector2.up * 2 * Time.deltaTime );
			}
			else if (PCManagerFramework.p_pDataGame.eCharacterCurrent == ECharacterName.zion)
			{
				g_Player3.transform.Translate( Vector2.up * 2 * Time.deltaTime );
			}
			else if (PCManagerFramework.p_pDataGame.eCharacterCurrent == ECharacterName.kkyung)
			{
				g_Player3.transform.Translate( Vector2.up * 2 * Time.deltaTime );
			}
		}

		if (!b_Startbool)
		{
			if (g_Player.transform.localPosition.y < -497)
			{
				g_Player.transform.Translate( Vector2.up * 0.5f * Time.deltaTime );
				g_Player1.transform.Translate( Vector2.up * 0.5f * Time.deltaTime );
				g_Player2.transform.Translate( Vector2.up * 0.5f * Time.deltaTime );
				g_Player3.transform.Translate( Vector2.up * 0.5f * Time.deltaTime );
			}
			else if (g_Player.transform.localPosition.y >= -497)
			{
				b_Startbool = true;
				if (!b_SkySpeed)
				{
					g_BackGround.GetComponent<Public_Offset>().f_Speed = 1;
				}

				if (g_Earth != null)
					g_Earth.GetComponent<Public_Offset>().f_Speed = 1;
			}
		}
	}

	public IEnumerator FeverTime()
	{
		MissionPlayer.instance.g_Shield.SetActive( true );
		Time.timeScale = 2;
		MissionPlayer.instance.b_feverTime = true;
		yield return new WaitForSeconds( 8 );
		Time.timeScale = 1;
		MissionPlayer.instance.b_feverTime = false;
		MissionPlayer.instance.g_Shield.SetActive( false );
	}

	public IEnumerator Behit()
	{
		yield return new WaitForSeconds( 0 );
		g_Damage.SetActive( true );
		yield return new WaitForSeconds( 2f );
		g_Damage.SetActive( false );
		MissionPlayer.instance.b_StrongTime = false;
		if (MissionPlayer.instance.g_Shield.activeInHierarchy.Equals( true ))
			MissionPlayer.instance.g_Shield.SetActive( false );
	}

	public IEnumerator Monster_Zen_Hard()
	{
		if (PCManagerFramework.p_pDataGame.iStage == 1)
		{

			f_BossHP = 1;
			StartCoroutine( LandMine_Left() );
			yield return new WaitForSeconds( 2 );
			StartCoroutine( LandMine_Right() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Drone_Color_Drop() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Human_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 2)
		{

			f_BossHP = 1;
			StartCoroutine( LandMine_Left() );
			yield return new WaitForSeconds( 2 );
			StartCoroutine( LandMine_Right() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Drone_Color_Drop() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Human_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 3)
		{

			f_BossHP = 2;
			StartCoroutine( LandMine_Left() );
			yield return new WaitForSeconds( 2 );
			StartCoroutine( LandMine_Right() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Drone_Color_Drop() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Human_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 4)
		{

			f_BossHP = 2;
			StartCoroutine( LandMine_Left() );
			yield return new WaitForSeconds( 2 );
			StartCoroutine( LandMine_Right() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Drone_Color_Drop() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Human_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 5)
		{

			f_BossHP = 2;
			StartCoroutine( LandMine_Left() );
			yield return new WaitForSeconds( 2 );
			StartCoroutine( LandMine_Right() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Drone_Color_Drop() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Human_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 6)
		{

			f_BossHP = 3;
			StartCoroutine( LandMine_Left() );
			yield return new WaitForSeconds( 2 );
			StartCoroutine( LandMine_Right() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Drone_Color_Drop() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Human_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 7)
		{

			f_BossHP = 3;
			StartCoroutine( LandMine_Left() );
			yield return new WaitForSeconds( 2 );
			StartCoroutine( LandMine_Right() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Drone_Color_Drop() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Human_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 8)
		{

			f_BossHP = 3;
			StartCoroutine( LandMine_Left() );
			yield return new WaitForSeconds( 2 );
			StartCoroutine( LandMine_Right() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Drone_Color_Drop() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Human_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 9)
		{

			f_BossHP = 3;
			StartCoroutine( LandMine_Left() );
			yield return new WaitForSeconds( 2 );
			StartCoroutine( LandMine_Right() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Drone_Color_Drop() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Human_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 10)
		{

			f_BossHP = 3;
			StartCoroutine( LandMine_Left() );
			yield return new WaitForSeconds( 2 );
			StartCoroutine( LandMine_Right() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Drone_Color_Drop() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Human_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}

		if (f_RealTime > 0)
		{
			StartCoroutine( Monster_Zen_Hard() );
		}
	}

	public IEnumerator Monster_Zen_Normal()
	{
		if (PCManagerFramework.p_pDataGame.iStage == 1)
		{

			b_BossDrop = true;
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Drone_Color_Drop() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 2)
		{

			b_BossDrop = true;
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Drone_Color_Drop() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 3)
		{

			f_BossHP = 1.5f;
			StartCoroutine( LandMine_Left() );
			yield return new WaitForSeconds( 2 );
			StartCoroutine( LandMine_Right() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Human_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 4)
		{

			f_BossHP = 1.5f;
			StartCoroutine( LandMine_Left() );
			yield return new WaitForSeconds( 2 );
			StartCoroutine( LandMine_Right() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Human_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 5)
		{

			f_BossHP = 1.5f;
			StartCoroutine( LandMine_Left() );
			yield return new WaitForSeconds( 2 );
			StartCoroutine( LandMine_Right() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Human_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 6)
		{

			f_BossHP = 2;
			StartCoroutine( LandMine_Left() );
			yield return new WaitForSeconds( 2 );
			StartCoroutine( LandMine_Right() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Human_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 7)
		{

			f_BossHP = 2;
			StartCoroutine( LandMine_Left() );
			yield return new WaitForSeconds( 2 );
			StartCoroutine( LandMine_Right() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Human_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 8)
		{

			f_BossHP = 2;
			StartCoroutine( LandMine_Left() );
			yield return new WaitForSeconds( 2 );
			StartCoroutine( LandMine_Right() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Human_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 9)
		{

			f_BossHP = 2;
			StartCoroutine( LandMine_Left() );
			yield return new WaitForSeconds( 2 );
			StartCoroutine( LandMine_Right() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Human_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 10)
		{

			f_BossHP = 2;
			StartCoroutine( LandMine_Left() );
			yield return new WaitForSeconds( 2 );
			StartCoroutine( LandMine_Right() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Drone_Color_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Human_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}

		if (f_RealTime > 0)
		{
			StartCoroutine( Monster_Zen_Normal() );
		}
	}

	public IEnumerator Monster_Zen_Easy()
	{
		if (PCManagerFramework.p_pDataGame.iStage == 1)
		{

			b_BossDrop = true;
			yield return new WaitForSeconds( 2f );
			StartCoroutine( LandMine_Left() );
			StartCoroutine( Drone_Left() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( LandMine_Right() );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Satellite_Left() );
			yield return new WaitForSeconds( 1f );
			StartCoroutine( Satellite_Right() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 2)
		{

			b_BossDrop = true;
			yield return new WaitForSeconds( 2f );
			StartCoroutine( LandMine_Left() );
			StartCoroutine( Drone_Left() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( LandMine_Right() );
			StartCoroutine( Drone_Right() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Satellite_Left() );
			yield return new WaitForSeconds( 1f );
			StartCoroutine( Satellite_Right() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 3)
		{

			f_BossHP = 1;
			yield return new WaitForSeconds( 2f );
			StartCoroutine( LandMine_Left() );
			StartCoroutine( Drone_Right() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( LandMine_Right() );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Satellite_Left() );
			yield return new WaitForSeconds( 1f );
			StartCoroutine( Satellite_Right() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Drone_Color_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Human_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 4)
		{

			f_BossHP = 1;
			yield return new WaitForSeconds( 2f );
			StartCoroutine( LandMine_Left() );
			StartCoroutine( Drone_Right() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( LandMine_Right() );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Satellite_Left() );
			yield return new WaitForSeconds( 1f );
			StartCoroutine( Satellite_Right() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Drone_Color_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Human_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 5)
		{

			f_BossHP = 1;
			yield return new WaitForSeconds( 2f );
			StartCoroutine( LandMine_Left() );
			StartCoroutine( Drone_Right() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( LandMine_Right() );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Satellite_Left() );
			yield return new WaitForSeconds( 1f );
			StartCoroutine( Satellite_Right() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Drone_Color_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Human_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 6)
		{

			f_BossHP = 1;
			yield return new WaitForSeconds( 2f );
			StartCoroutine( LandMine_Left() );
			StartCoroutine( Drone_Right() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( LandMine_Right() );
			StartCoroutine( Drone_Left() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Satellite_Left() );
			yield return new WaitForSeconds( 1f );
			StartCoroutine( Satellite_Right() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Drone_Color_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Human_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 7)
		{

			f_BossHP = 1;
			yield return new WaitForSeconds( 2f );
			StartCoroutine( LandMine_Left() );
			StartCoroutine( Drone_Right() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( LandMine_Right() );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Satellite_Left() );
			yield return new WaitForSeconds( 1f );
			StartCoroutine( Satellite_Right() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Drone_Color_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Human_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 8)
		{

			f_BossHP = 1;
			yield return new WaitForSeconds( 2f );
			StartCoroutine( LandMine_Left() );
			StartCoroutine( Drone_Right() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( LandMine_Right() );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Satellite_Left() );
			yield return new WaitForSeconds( 1f );
			StartCoroutine( Satellite_Right() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Drone_Color_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Human_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 9)
		{

			f_BossHP = 1;
			yield return new WaitForSeconds( 2f );
			StartCoroutine( LandMine_Left() );
			StartCoroutine( Drone_Right() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( LandMine_Right() );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Satellite_Left() );
			yield return new WaitForSeconds( 1 );
			StartCoroutine( Satellite_Right() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Drone_Color_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Human_Drop() );

		}
		else if (PCManagerFramework.p_pDataGame.iStage == 10)
		{

			f_BossHP = 1;
			yield return new WaitForSeconds( 2f );
			StartCoroutine( LandMine_Left() );
			StartCoroutine( Drone_Right() );
			yield return new WaitForSeconds( 3f );
			StartCoroutine( LandMine_Right() );
			StartCoroutine( Drone_Left() );
			StartCoroutine( Meteo_Drop() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Satellite_Left() );
			yield return new WaitForSeconds( 1f );
			StartCoroutine( Satellite_Right() );
			yield return new WaitForSeconds( 2f );
			StartCoroutine( Drone_Color_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Meteo_Drop() );
			StartCoroutine( Human_Drop() );

		}

		if (f_RealTime > 0)
		{
			StartCoroutine( Monster_Zen_Easy() );
		}
	}

	public IEnumerator FeverTime_Zen()
	{
		yield return new WaitForSeconds( 0 );
		StartCoroutine( LandMine_Left() );
		StartCoroutine( LandMine_Right() );
	}

	IEnumerator LandMine_Left()
	{
		for (int i = 0; i < 5; i++)
		{
			GameObject Land = Instantiate( g_LandMine, g_ObjectParent.transform ) as GameObject;
			Land.transform.localPosition = new Vector2( -426, 704 );
			Array.Resize( ref Land.GetComponent<EnemyMove>().b_Path, 4 );
			Array.Resize( ref Land.GetComponent<EnemyMove>().v2_Path_Point, 4 );
			Land.GetComponent<EnemyMove>().v2_Path_Point[0] = new Vector2( -256, 545 );
			Land.GetComponent<EnemyMove>().v2_Path_Point[1] = new Vector2( 304, 189 );
			Land.GetComponent<EnemyMove>().v2_Path_Point[2] = new Vector2( -276, -203 );
			Land.GetComponent<EnemyMove>().v2_Path_Point[3] = new Vector2( 379, -763 );
			Land.GetComponent<EnemyMove>().b_Path[0] = true;
			Land.GetComponent<EnemyMove>().b_Move = true;
			yield return new WaitForSeconds( 0.5f );
		}
	}

	IEnumerator LandMine_Right()
	{
		for (int i = 0; i < 5; i++)
		{
			GameObject Land = Instantiate( g_LandMine, g_ObjectParent.transform ) as GameObject;
			Land.transform.localPosition = new Vector2( 426, 704 );
			Array.Resize( ref Land.GetComponent<EnemyMove>().b_Path, 4 );
			Array.Resize( ref Land.GetComponent<EnemyMove>().v2_Path_Point, 4 );
			Land.GetComponent<EnemyMove>().v2_Path_Point[0] = new Vector2( 256, 545 );
			Land.GetComponent<EnemyMove>().v2_Path_Point[1] = new Vector2( -304, 189 );
			Land.GetComponent<EnemyMove>().v2_Path_Point[2] = new Vector2( 276, -203 );
			Land.GetComponent<EnemyMove>().v2_Path_Point[3] = new Vector2( -379, -763 );
			Land.GetComponent<EnemyMove>().b_Path[0] = true;
			Land.GetComponent<EnemyMove>().b_Move = true;
			yield return new WaitForSeconds( 0.5f );
		}
	}

	IEnumerator Drone_Left()
	{
		for (int i = 0; i < 3; i++)
		{
			GameObject Land = Instantiate( g_Drone, g_ObjectParent.transform ) as GameObject;
			Land.transform.localPosition = new Vector2( -606, 505 );
			Array.Resize( ref Land.GetComponent<EnemyMove>().b_Path, 2 );
			Array.Resize( ref Land.GetComponent<EnemyMove>().v2_Path_Point, 2 );
			Land.GetComponent<EnemyMove>().v2_Path_Point[0] = new Vector2( 0, 58 );
			Land.GetComponent<EnemyMove>().v2_Path_Point[1] = new Vector2( 0, -836 );
			Land.GetComponent<EnemyMove>().b_Path[0] = true;
			Land.GetComponent<EnemyMove>().b_Move = true;
			yield return new WaitForSeconds( 1f );
		}
	}

	IEnumerator Drone_Right()
	{
		for (int i = 0; i < 3; i++)
		{
			GameObject Land = Instantiate( g_Drone, g_ObjectParent.transform ) as GameObject;
			Land.transform.localPosition = new Vector2( 606, 505 );
			Array.Resize( ref Land.GetComponent<EnemyMove>().b_Path, 2 );
			Array.Resize( ref Land.GetComponent<EnemyMove>().v2_Path_Point, 2 );
			Land.GetComponent<EnemyMove>().v2_Path_Point[0] = new Vector2( 0, 58 );
			Land.GetComponent<EnemyMove>().v2_Path_Point[1] = new Vector2( 0, -836 );
			Land.GetComponent<EnemyMove>().b_Path[0] = true;
			Land.GetComponent<EnemyMove>().b_Move = true;
			yield return new WaitForSeconds( 1f );
		}
	}

	IEnumerator Satellite_Left()
	{
		for (int i = 0; i < 3; i++)
		{
			GameObject Land = Instantiate( g_Satellite, g_ObjectParent.transform ) as GameObject;
			Land.transform.localPosition = new Vector2( 0, 787 );
			Array.Resize( ref Land.GetComponent<EnemyMove>().b_Path, 2 );
			Array.Resize( ref Land.GetComponent<EnemyMove>().v2_Path_Point, 2 );
			Land.GetComponent<EnemyMove>().v2_Path_Point[0] = new Vector2( 0, -110 );
			Land.GetComponent<EnemyMove>().v2_Path_Point[1] = new Vector2( -564, -917 );
			Land.GetComponent<EnemyMove>().b_Path[0] = true;
			Land.GetComponent<EnemyMove>().b_Move = true;
			yield return new WaitForSeconds( 1f );
		}
	}

	IEnumerator Satellite_Right()
	{
		for (int i = 0; i < 3; i++)
		{
			GameObject Land = Instantiate( g_Satellite, g_ObjectParent.transform ) as GameObject;
			Land.transform.localPosition = new Vector2( 0, 787 );
			Array.Resize( ref Land.GetComponent<EnemyMove>().b_Path, 2 );
			Array.Resize( ref Land.GetComponent<EnemyMove>().v2_Path_Point, 2 );
			Land.GetComponent<EnemyMove>().v2_Path_Point[0] = new Vector2( 0, -110 );
			Land.GetComponent<EnemyMove>().v2_Path_Point[1] = new Vector2( 564, -917 );
			Land.GetComponent<EnemyMove>().b_Path[0] = true;
			Land.GetComponent<EnemyMove>().b_Move = true;
			yield return new WaitForSeconds( 1f );
		}
	}

	IEnumerator Meteo_Drop()
	{
		yield return new WaitForSeconds( 0 );
		int i_Meteo_Random = UnityEngine.Random.Range( 0, 3 );
		int i_MeteoPos_Random = UnityEngine.Random.Range( -290, 291 );
		if (i_Meteo_Random == 0)
		{
			GameObject Meteo = Instantiate( g_Meteo_1, g_ObjectParent.transform ) as GameObject;
			Meteo.transform.localPosition = new Vector2( i_MeteoPos_Random, 790 );
		}
		else if (i_Meteo_Random == 1)
		{
			GameObject Meteo = Instantiate( g_Meteo_2, g_ObjectParent.transform ) as GameObject;
			Meteo.transform.localPosition = new Vector2( i_MeteoPos_Random, 790 );
		}
		else if (i_Meteo_Random == 2)
		{
			GameObject Meteo = Instantiate( g_Meteo_3, g_ObjectParent.transform ) as GameObject;
			Meteo.transform.localPosition = new Vector2( i_MeteoPos_Random, 790 );
		}
	}

	IEnumerator Black_Worm()
	{
		yield return new WaitForSeconds( 0 );
		int i_Hole_Random = UnityEngine.Random.Range( 0, 2 );
		int i_HolePos_Random = UnityEngine.Random.Range( -290, 291 );
		if (i_Hole_Random == 0)
		{
			GameObject Hole = Instantiate( g_Black_Hole, g_ObjectParent.transform ) as GameObject;
			Hole.transform.localPosition = new Vector2( i_HolePos_Random, 790 );
		}
		else
		{
			GameObject Hole = Instantiate( g_Worm_Hole, g_ObjectParent.transform ) as GameObject;
			Hole.transform.localPosition = new Vector2( i_HolePos_Random, 790 );
		}
	}

	IEnumerator SpaceChamber_Drop()
	{
		yield return new WaitForSeconds( 0f );
		int i_SpaceChamber_Random = UnityEngine.Random.Range( 0, 2 );
		int i_SpaceChamberPos_Random = UnityEngine.Random.Range( -250, 250 );
		if (i_SpaceChamber_Random == 0)
		{
			GameObject Hole = Instantiate( g_SpaceChamber, g_ObjectParent.transform ) as GameObject;
			Hole.transform.localPosition = new Vector2( i_SpaceChamberPos_Random, 790 );
		}
		else
		{
			GameObject Hole = Instantiate( g_SpaceChamber2, g_ObjectParent.transform ) as GameObject;
			Hole.transform.localPosition = new Vector2( i_SpaceChamberPos_Random, 790 );
		}
	}

	IEnumerator Drone_Color_Drop()
	{
		int PosRandom = UnityEngine.Random.Range( 0, 2 );

		if (PosRandom == 0)
		{
			int ColorRandom = UnityEngine.Random.Range( 0, 3 );
			for (int i = 0; i < 4; i++)
			{
				GameObject gs_Drones = Instantiate( gs_Drone[ColorRandom], g_ObjectParent.transform ) as GameObject;
				gs_Drones.transform.localPosition = new Vector2( -470, 120 );
				Array.Resize( ref gs_Drones.GetComponent<EnemyMove>().b_Path, 2 );
				Array.Resize( ref gs_Drones.GetComponent<EnemyMove>().v2_Path_Point, 2 );
				gs_Drones.GetComponent<EnemyMove>().v2_Path_Point[0] = new Vector2( 470, 120 );
				gs_Drones.GetComponent<EnemyMove>().v2_Path_Point[1] = new Vector2( 470, -917 );
				gs_Drones.GetComponent<EnemyMove>().b_Path[0] = true;
				gs_Drones.GetComponent<EnemyMove>().b_Move = true;
				yield return new WaitForSeconds( 1.5f );
			}
		}
		else
		{
			int ColorRandom = UnityEngine.Random.Range( 0, 3 );
			for (int i = 0; i < 4; i++)
			{
				GameObject gs_Drones = Instantiate( gs_Drone[ColorRandom], g_ObjectParent.transform ) as GameObject;
				gs_Drones.transform.localPosition = new Vector2( 470, 120 );
				Array.Resize( ref gs_Drones.GetComponent<EnemyMove>().b_Path, 2 );
				Array.Resize( ref gs_Drones.GetComponent<EnemyMove>().v2_Path_Point, 2 );
				gs_Drones.GetComponent<EnemyMove>().v2_Path_Point[0] = new Vector2( -470, 120 );
				gs_Drones.GetComponent<EnemyMove>().v2_Path_Point[1] = new Vector2( -470, -917 );
				gs_Drones.GetComponent<EnemyMove>().b_Path[0] = true;
				gs_Drones.GetComponent<EnemyMove>().b_Move = true;
				yield return new WaitForSeconds( 1.5f );
			}
		}
		GameObject gs_Turret_Colors = Instantiate( gs_Turret_Color[0], g_ObjectParent.transform ) as GameObject;
		gs_Turret_Colors.transform.localPosition = new Vector2( -170, 740 );
		Array.Resize( ref gs_Turret_Colors.GetComponent<EnemyMove>().b_Path, 1 );
		Array.Resize( ref gs_Turret_Colors.GetComponent<EnemyMove>().v2_Path_Point, 1 );
		gs_Turret_Colors.GetComponent<EnemyMove>().v2_Path_Point[0] = new Vector2( 170, -917 );
		gs_Turret_Colors.GetComponent<EnemyMove>().b_Path[0] = true;
		gs_Turret_Colors.GetComponent<EnemyMove>().b_Move = true;
		GameObject gs_Turret_Colors2 = Instantiate( gs_Turret_Color[1], g_ObjectParent.transform ) as GameObject;
		gs_Turret_Colors2.transform.localPosition = new Vector2( 170, 740 );
		Array.Resize( ref gs_Turret_Colors2.GetComponent<EnemyMove>().b_Path, 1 );
		Array.Resize( ref gs_Turret_Colors2.GetComponent<EnemyMove>().v2_Path_Point, 1 );
		gs_Turret_Colors2.GetComponent<EnemyMove>().v2_Path_Point[0] = new Vector2( -170, -917 );
		gs_Turret_Colors2.GetComponent<EnemyMove>().b_Path[0] = true;
		gs_Turret_Colors2.GetComponent<EnemyMove>().b_Move = true;
	}

	IEnumerator Turret_Drop()
	{
		for (int i = 0; i < 2; i++)
		{
			GameObject gs_Turrets = Instantiate( gs_Turret[0], g_ObjectParent.transform ) as GameObject;
			gs_Turrets.transform.localPosition = new Vector2( -280, 740 );
			Array.Resize( ref gs_Turrets.GetComponent<EnemyMove>().b_Path, 3 );
			Array.Resize( ref gs_Turrets.GetComponent<EnemyMove>().v2_Path_Point, 3 );
			gs_Turrets.GetComponent<EnemyMove>().v2_Path_Point[0] = new Vector2( -50, 0 );
			gs_Turrets.GetComponent<EnemyMove>().v2_Path_Point[1] = new Vector2( 490, -430 );
			gs_Turrets.GetComponent<EnemyMove>().v2_Path_Point[2] = new Vector2( 490, -917 );
			gs_Turrets.GetComponent<EnemyMove>().b_Path[0] = true;
			gs_Turrets.GetComponent<EnemyMove>().b_Move = true;
			yield return new WaitForSeconds( 3 );
		}
	}

	IEnumerator Turret_Drop2()
	{
		for (int i = 0; i < 2; i++)
		{
			GameObject gs_Turrets = Instantiate( gs_Turret[1], g_ObjectParent.transform ) as GameObject;
			gs_Turrets.transform.localPosition = new Vector2( 280, 740 );
			Array.Resize( ref gs_Turrets.GetComponent<EnemyMove>().b_Path, 3 );
			Array.Resize( ref gs_Turrets.GetComponent<EnemyMove>().v2_Path_Point, 3 );
			gs_Turrets.GetComponent<EnemyMove>().v2_Path_Point[0] = new Vector2( 50, 0 );
			gs_Turrets.GetComponent<EnemyMove>().v2_Path_Point[1] = new Vector2( -490, -430 );
			gs_Turrets.GetComponent<EnemyMove>().v2_Path_Point[2] = new Vector2( -490, -917 );
			gs_Turrets.GetComponent<EnemyMove>().b_Path[0] = true;
			gs_Turrets.GetComponent<EnemyMove>().b_Move = true;
			yield return new WaitForSeconds( 3 );
		}
	}

	public IEnumerator Human_Drop()
	{
		yield return new WaitForSeconds( 0 );
		int HumanRandom = UnityEngine.Random.Range( 0, 6 );
		GameObject Human = Instantiate( gs_Human[HumanRandom], g_ObjectParent.transform ) as GameObject;
		Human.transform.localPosition = new Vector2( -250, 740 );
		Array.Resize( ref Human.GetComponent<EnemyMove>().b_Path, 4 );
		Array.Resize( ref Human.GetComponent<EnemyMove>().v2_Path_Point, 4 );
		Human.GetComponent<EnemyMove>().v2_Path_Point[0] = new Vector2( -170, 170 );
		Human.GetComponent<EnemyMove>().v2_Path_Point[1] = new Vector2( 300, 0 );
		Human.GetComponent<EnemyMove>().v2_Path_Point[2] = new Vector2( -490, -170 );
		Human.GetComponent<EnemyMove>().v2_Path_Point[3] = new Vector2( -490, -917 );
		Human.GetComponent<EnemyMove>().b_Path[0] = true;
		Human.GetComponent<EnemyMove>().b_Move = true;

		HumanRandom = UnityEngine.Random.Range( 0, 6 );
		GameObject Human2 = Instantiate( gs_Human[HumanRandom], g_ObjectParent.transform ) as GameObject;
		Human2.transform.localPosition = new Vector2( 250, 740 );
		Array.Resize( ref Human2.GetComponent<EnemyMove>().b_Path, 4 );
		Array.Resize( ref Human2.GetComponent<EnemyMove>().v2_Path_Point, 4 );
		Human2.GetComponent<EnemyMove>().v2_Path_Point[0] = new Vector2( 170, 170 );
		Human2.GetComponent<EnemyMove>().v2_Path_Point[1] = new Vector2( -300, 0 );
		Human2.GetComponent<EnemyMove>().v2_Path_Point[2] = new Vector2( 490, -170 );
		Human2.GetComponent<EnemyMove>().v2_Path_Point[3] = new Vector2( 490, -917 );
		Human2.GetComponent<EnemyMove>().b_Path[0] = true;
		Human2.GetComponent<EnemyMove>().b_Move = true;
	}

	IEnumerator Boss_Drop()
	{
		if (!b_BossDrop)
		{
			MissionPlayer.instance.StopCoroutine( MissionPlayer.instance.FuelStop );
			yield return new WaitForSeconds( 3 );
			MissionPlayer.instance.b_OutsideTouch = true;
			g_Warning.SetActive( true );
			yield return new WaitForSeconds( 4 );
			MissionPlayer.instance.b_OutsideTouch = false;
			g_Warning.SetActive( false );
			GameObject Boss = Instantiate( g_Boss, g_ObjectParent.transform ) as GameObject;
			Boss.transform.localPosition = new Vector2( 0, 740 );
			g_BossHP_Bar.SetActive( true );
		}
		else
		{
			StartCoroutine( Clear() );
		}
	}

	IEnumerator StartAni()//-------------------------------------------스타트 텍스트 출력
	{
		yield return new WaitForSeconds( 1f );
		g_Start.SetActive( true );

		yield return new WaitForSeconds( 3f );
		MissionPlayer.instance.b_OutsideTouch = false;
		if (PCManagerFramework.p_pDataGame.eLevel == ELevel.Easy)
		{
			Debug.Log( "Easy" );
			StartCoroutine( Monster_Zen_Easy() );
		}
		else if (PCManagerFramework.p_pDataGame.eLevel == ELevel.Normal)
		{
			Debug.Log( "Normal" );
			StartCoroutine( Monster_Zen_Normal() );
		}
		else if (PCManagerFramework.p_pDataGame.eLevel == ELevel.Hard)
		{
			Debug.Log( "Hard" );
			StartCoroutine( Monster_Zen_Hard() );
		}
	}

	public IEnumerator Clear()
	{
		if (PCManagerFramework.p_pDataGame.iStage == PCManagerFramework.p_pDataGame.iOpenStage)
		{
			PCManagerFramework.p_pDataGame.iOpenStage++;
		}
		MissionPlayer.instance.b_OutsideTouch = true;
		MissionPlayer.instance.b_Invisible = true;
		b_victory = true;
		yield return new WaitForSeconds( 2f );
		g_ClearScene.SetActive( true );
		g_MissionScene.SetActive( false );
		if (PCManagerFramework.p_pDataGame.bTutorial)
			PCManagerUIShared.instance.DoShowHide_Frame(PCManagerUIShared.EUIFrame.PCUISharedFrame_Dialogue, true);

		while (exe < 0.65f)
		{
			exe += 0.01f;
			yield return new WaitForSeconds( 0.01f );
		}
	}

	public IEnumerator MonObject_Stop()
	{
		yield return new WaitForSeconds( 0f );
		GameObject[] Monsters = GameObject.FindGameObjectsWithTag( "Enemy" );
		GameObject[] Items = GameObject.FindGameObjectsWithTag( "Item" );
		for (int i = 0; i < Monsters.Length; i++)
		{
			if (Monsters[i].GetComponent<EnemyMove>())
				Monsters[i].GetComponent<EnemyMove>().b_Stop = true;
		}

		for (int i = 0; i < Items.Length; i++)
		{
			Items[i].GetComponent<Mission_Item>().b_Move = true;
		}
	}

	public IEnumerator MonObject_Move()//-------------------------------------------------------asd;fljsda;lfshagoqnhd;g;klmag;kasngdho;wqherof;r
	{
		yield return new WaitForSeconds( 0f );
		GameObject[] Monsters = GameObject.FindGameObjectsWithTag( "Enemy" );
		GameObject[] Items = GameObject.FindGameObjectsWithTag( "Item" );
		for (int i = 0; i < Monsters.Length; i++)
		{
			if (Monsters[i].GetComponent<EnemyMove>())
				Monsters[i].GetComponent<EnemyMove>().b_Stop = false;
		}

		for (int i = 0; i < Items.Length; i++)
		{
			Items[i].GetComponent<Mission_Item>().b_Down = true;
		}
	}

	public void Stop_Button()//----------------------------------------일시정지
	{
		if (!b_Stop)
		{
			g_Stop.SetActive( true );
			b_Stop = true;
			Time.timeScale = 0;
		}
		else
		{
			g_Stop.SetActive( false );
			b_Stop = false;
			Time.timeScale = 1;
		}
	}

	public void Stop_RePlay()//-----------------------------------------다시게임진행
	{
		b_Stop = true;
		Time.timeScale = 1;
		g_Stop.SetActive( false );
	}

	public void Stop_Main()//------------------------------------------메인씬으로
	{
		Time.timeScale = 1;
		PCManagerFramework.DoLoadScene_FadeInOut( ESceneName.OutGame, 1f, Color.black );
	}

	public void Stop_Exit()//-------------------------------------------스톱취소
	{
		g_Stop.SetActive( false );
		g_RealStop.SetActive( true );
	}

	public void Stop_Back()//-------------------------------------------나가기에서 일시정지씬으로
	{
		g_Stop.SetActive( true );
		g_RealStop.SetActive( false );
	}

	public void AppQuit()//----------------------------------------------앱종료
	{
		Application.Quit();
	}

	public void Over()
	{
		g_OverScene.SetActive( true );
		g_MissionScene.SetActive( false );
		if (PCManagerFramework.p_pDataGame.bTutorial)
			PCManagerUIShared.instance.DoShowHide_Frame(PCManagerUIShared.EUIFrame.PCUISharedFrame_Dialogue, true);
	}

	void OnTriggerEnter2D( Collider2D Coll )
	{
		if (Coll.gameObject.CompareTag( "Enemy" ))
		{
			Destroy( Coll.gameObject );
		}

		if (Coll.gameObject.GetComponent<Mission_Item>())
		{
			Destroy( Coll.gameObject );
		}
	}
}
