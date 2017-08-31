using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
public class MissionRoulette : MonoBehaviour
{
	public static MissionRoulette instance;

	public GameObject g_Background;
	public GameObject g_Coin;//----------------------------코인이펙트
	public GameObject g_Wolrd;
	public GameObject g_Black;
	public GameObject[] g_Pins;
	public GameObject[] gs_CoinPos;

	public bool b_Slide = false;//---------------------룰렛 활성화, 비활성화
	public bool b_Slide_Off = false;
	public bool b_Roulette = false;

	int i_Roulette_Result = 0;

	int i_Roulette1 = 0, i_Roulette2 = 0, i_Roulette3 = 0;

	SkeletonAnimation skeletonAnimation;//-----------------스파인
	SkeletonAnimation PinAni, PinAni2, PinAni3, PinAni4, PinAni5, PinAni6;//------------스파인

	// Use this for initialization
	void Start()
	{
		instance = this;
		skeletonAnimation = GetComponent<SkeletonAnimation>();
		PinAni = g_Pins[0].GetComponent<SkeletonAnimation>();
		PinAni2 = g_Pins[1].GetComponent<SkeletonAnimation>();
		PinAni3 = g_Pins[2].GetComponent<SkeletonAnimation>();
		PinAni4 = g_Pins[3].GetComponent<SkeletonAnimation>();
		PinAni5 = g_Pins[4].GetComponent<SkeletonAnimation>();
	}

	// Update is called once per frame
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Alpha1))
			StartCoroutine(Down());

		if (Input.GetKeyDown(KeyCode.Alpha2))
			StartCoroutine(Up());

		if (Input.GetKeyDown(KeyCode.Alpha3))
			StartCoroutine(Invisible());

		if (Input.GetKeyDown(KeyCode.Alpha4))
			StartCoroutine(Rotate());

		if (Input.GetKeyDown(KeyCode.Alpha5))
			StartCoroutine(Rotate());

		if (Input.GetKeyDown(KeyCode.Alpha6))
			StartCoroutine(Fail());

		if (Input.GetKeyDown(KeyCode.Alpha7))
			StartCoroutine(Stop_PowerUp());


		if (b_Slide)
		{
			b_Slide = false;
			StartCoroutine( Down() );
		}

		if (b_Slide_Off)
		{
			b_Slide_Off = false;
			StartCoroutine( Up() );
		}
	}

	void Spine_Result( SkeletonAnimation Pin, bool Loop, float Speed, string Ani )//------------------------------------------------애니메이션 함수
	{
		Pin.loop = Loop;
		Pin.timeScale = Speed;
		Pin.AnimationName = Ani;
	}

	IEnumerator Down()
	{
		b_Roulette = true;
		//MissionPlayer.instance.StopCoroutine( MissionPlayer.instance.FuelStop );
		//MissionGM.instance.b_SkySpeed = true;
		//g_Background.GetComponent<Public_Offset>().f_Speed = 0;
		//MissionGM.instance.b_RouletteTime = true;
		//MissionPlayer.instance.b_Invisible = true;
		//MissionPlayer.instance.b_OutsideTouch = true;
		//GameObject[] Enemys = GameObject.FindGameObjectsWithTag( "Enemy" );
		//for (int i = 0; i < Enemys.Length; i++)
		//{
		//	if (!Enemys[i].GetComponent<EnemyMove>().b_Meteo)
		//		Enemys[i].GetComponent<EnemyMove>().StopAllCoroutines();
		//}
		//g_Black.SetActive( true );
		//yield return new WaitForSeconds( 0 );
		b_Slide_Off = false;
		Spine_Result( skeletonAnimation, false, 1, "down" );
		Spine_Result( PinAni5, false, 1, "pindown" );
		Spine_Result( PinAni4, false, 1, "backgrounddown" );
		Spine_Result( PinAni, false, 1, "slotdown" );
		Spine_Result( PinAni2, false, 1, "slotdown" );
		Spine_Result( PinAni3, false, 1, "slotdown" );
		StartCoroutine( Rotate() );

		yield return null;
	}

	IEnumerator Up()//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	{
		yield return new WaitForSeconds( 2 );
		MissionPlayer.instance.b_OutsideTouch = false;
		MissionGM.instance.b_SkySpeed = false;
		g_Background.GetComponent<Public_Offset>().f_Speed = 1;
		b_Slide = false;
		MissionGM.instance.b_RouletteTime = false;
		if (i_Roulette_Result == 1)
		{
			for (int i = 0; i < gs_CoinPos.Length; i++)
			{
				GameObject Coins = Instantiate( g_Coin, g_Wolrd.transform );
				Coins.transform.localPosition = gs_CoinPos[i].transform.localPosition;
				Coins.transform.localScale = new Vector2( 94, 94 );
				yield return new WaitForSeconds( 0.1f );
			}
			PCManagerFramework.p_pDataGame.iGold += 1000;
			yield return new WaitForSeconds( 2f );
		}
		Spine_Result( skeletonAnimation, false, 1, "up" );
		Spine_Result( PinAni4, false, 1, "backgroundup" );

		if (i_Roulette1 == 1)
		{
			Spine_Result( PinAni, false, 1, "blackhole1up" );
		}
		else if (i_Roulette1 == 2)
		{
			Spine_Result( PinAni, false, 1, "fever1up" );
		}
		else if (i_Roulette1 == 3)
		{
			Spine_Result( PinAni, false, 1, "fuel1up" );
		}
		else if (i_Roulette1 == 4)
		{
			Spine_Result( PinAni, false, 1, "gold1up" );
		}
		else if (i_Roulette1 == 5)
		{
			Spine_Result( PinAni, false, 1, "power1up" );
		}
		else if (i_Roulette1 == 6)
		{
			Spine_Result( PinAni, false, 1, "universe1up" );
		}
		else if (i_Roulette1 == 7)
		{
			Spine_Result( PinAni, false, 1, "ticket1up" );
		}

		if (i_Roulette2 == 1)
		{
			Spine_Result( PinAni2, false, 1, "blackhole2up" );
		}
		else if (i_Roulette2 == 2)
		{
			Spine_Result( PinAni2, false, 1, "fever2up" );
		}
		else if (i_Roulette2 == 3)
		{
			Spine_Result( PinAni2, false, 1, "fuel2up" );
		}
		else if (i_Roulette2 == 4)
		{
			Spine_Result( PinAni2, false, 1, "gold2up" );
		}
		else if (i_Roulette2 == 5)
		{
			Spine_Result( PinAni2, false, 1, "power2up" );
		}
		else if (i_Roulette2 == 6)
		{
			Spine_Result( PinAni2, false, 1, "universe2up" );
		}
		else if (i_Roulette2 == 7)
		{
			Spine_Result( PinAni2, false, 1, "ticket2up" );
		}

		if (i_Roulette3 == 1)
		{
			Spine_Result( PinAni3, false, 1, "blackhole3up" );
		}
		else if (i_Roulette3 == 2)
		{
			Spine_Result( PinAni3, false, 1, "fever3up" );
		}
		else if (i_Roulette3 == 3)
		{
			Spine_Result( PinAni3, false, 1, "fuel3up" );
		}
		else if (i_Roulette3 == 4)
		{
			Spine_Result( PinAni3, false, 1, "gold3up" );
		}
		else if (i_Roulette3 == 5)
		{
			Spine_Result( PinAni3, false, 1, "power3up" );
		}
		else if (i_Roulette3 == 6)
		{
			Spine_Result( PinAni3, false, 1, "universe3up" );
		}
		else if (i_Roulette3 == 7)
		{
			Spine_Result( PinAni3, false, 1, "ticket3up" );
		}
		Spine_Result( PinAni5, false, 1, "pinup" );
		g_Black.SetActive( false );
		MissionGM.instance.StartCoroutine( MissionGM.instance.MonObject_Move() );
		if (PCManagerFramework.p_pDataGame.eLevel == ELevel.Easy)
			MissionGM.instance.StartCoroutine( MissionGM.instance.Monster_Zen_Easy() );
		else if (PCManagerFramework.p_pDataGame.eLevel == ELevel.Normal)
			MissionGM.instance.StartCoroutine( MissionGM.instance.Monster_Zen_Normal() );
		else if (PCManagerFramework.p_pDataGame.eLevel == ELevel.Hard)
			MissionGM.instance.StartCoroutine( MissionGM.instance.Monster_Zen_Hard() );

		if (i_Roulette_Result == 2)
		{
			Color Alp = MissionPlayer.instance.gs_PlayerPower[PCManagerFramework.p_pDataGame.iFuelNum - 1].GetComponent<UISprite>().color;
			Alp.a = 1;
			MissionPlayer.instance.gs_PlayerPower[PCManagerFramework.p_pDataGame.iFuelNum - 1].GetComponent<UISprite>().color = Alp;
			if (PCManagerFramework.p_pDataGame.iFuelNum < 8)
			{
				MissionPlayer.instance.gs_PlayerPower[PCManagerFramework.p_pDataGame.iFuelNum].SetActive( true );
				PCManagerFramework.p_pDataGame.iFuelNum++;
			}
		}
		else if (i_Roulette_Result == 3)
		{
			PCManagerFramework.p_pDataGame.iTicket++;
		}
		else if (i_Roulette_Result == 4)
		{
			if (!PCManagerFramework.p_pDataGame.bTutorial)
				PCManagerFramework.DoLoadScene_FadeInOut( ESceneName.Mission, 1f, Color.black );
		}
		else if (i_Roulette_Result == 5)
		{
			if (MissionPlayer.instance.i_PowerLevel < 3)
				MissionPlayer.instance.i_PowerLevel++;
		}
		else if (i_Roulette_Result == 6)
		{
			StartCoroutine( MissionGM.instance.FeverTime() );
			StartCoroutine( MissionGM.instance.FeverTime_Zen() );
		}
		b_Roulette = false;
		StartCoroutine( Invisible() );
		yield return new WaitForSeconds( 2f );
		MissionPlayer.instance.FuelStop = MissionPlayer.instance.StartCoroutine( MissionPlayer.instance.PowerTime() );
	}

	IEnumerator Invisible()
	{
		yield return new WaitForSeconds( 3f );
		MissionPlayer.instance.b_Invisible = false;
	}

	IEnumerator Rotate()
	{
		yield return new WaitForSeconds( 1.5f );
		Spine_Result( skeletonAnimation, true, 1, "bodyrun" );
		Spine_Result( PinAni, true, 1, "slotrun" );
		Spine_Result( PinAni2, true, 1, "slotrun" );
		Spine_Result( PinAni3, true, 1, "slotrun" );
		Spine_Result( PinAni5, true, 1, "pinrun" );

		int Random_Stop = Random.Range( 0, 7 );

		if (Random_Stop == 0)
		{
			StartCoroutine( Stop_Ticket() );
			Debug.Log( "Ticket" );
		}
		else if (Random_Stop == 1)
		{
			StartCoroutine( Stop_Gold() );
			Debug.Log( "Gold" );
		}
		else if (Random_Stop == 2)
		{
			StartCoroutine( Stop_Oil() );
			Debug.Log( "Oil" );
		}
		else if (Random_Stop == 3)
		{
			StartCoroutine( Stop_PowerUp() );
			Debug.Log( "PowerUp" );
		}
		else if (Random_Stop == 4)
		{
			StartCoroutine( Stop_BlackHole() );
			Debug.Log( "BlackHole" );
		}
		else if (Random_Stop == 5)
		{
			StartCoroutine( Stop_Fever() );
			Debug.Log( "Fever" );
		}
		else if (Random_Stop == 6)
		{
			StartCoroutine( Fail() );
			Debug.Log( "꽝" );
		}
	}

	IEnumerator Fail()
	{
		int Random_Ani = Random.Range( 0, 4 );
		if (Random_Ani == 0)
		{
			Spine_Result( PinAni, false, 1, "fever1" );
			i_Roulette1 = 2;
		}
		else if (Random_Ani == 1)
		{
			Spine_Result( PinAni, false, 1, "power1" );
			i_Roulette1 = 5;
		}
		else if (Random_Ani == 2)
		{
			Spine_Result( PinAni, false, 1, "fuel1" );
			i_Roulette1 = 3;
		}
		else if (Random_Ani == 3)
		{
			Spine_Result( PinAni, false, 1, "gold1" );
			i_Roulette1 = 4;
		}

		yield return new WaitForSeconds( 1.5f );
		Random_Ani = Random.Range( 0, 4 );
		if (Random_Ani == 0)
		{
			Spine_Result( PinAni2, false, 1, "fever2" );
			i_Roulette2 = 2;
		}
		else if (Random_Ani == 1)
		{
			Spine_Result( PinAni2, false, 1, "ticket2" );
			i_Roulette2 = 7;
		}
		else if (Random_Ani == 2)
		{
			Spine_Result( PinAni2, false, 1, "fuel2" );
			i_Roulette2 = 3;
		}
		else if (Random_Ani == 3)
		{
			Spine_Result( PinAni2, false, 1, "gold2" );
			i_Roulette2 = 4;
		}

		yield return new WaitForSeconds( 1.5f );
		Random_Ani = Random.Range( 0, 2 );
		if (Random_Ani == 0)
		{
			Spine_Result( PinAni3, false, 1, "power3" );
			Spine_Result( PinAni5, false, 1, "pinpick" );
			i_Roulette3 = 5;
		}
		else if (Random_Ani == 1)
		{
			Spine_Result( PinAni3, false, 1, "ticket3" );
			Spine_Result( PinAni5, false, 1, "pinpick" );
			i_Roulette3 = 7;
		}
		StartCoroutine( Up() );
	}

	IEnumerator Stop_PowerUp()
	{
		Spine_Result( PinAni, false, 1, "power1" );
		i_Roulette1 = 5;
		yield return new WaitForSeconds( 1.5f );
		Spine_Result( PinAni2, false, 1, "power2" );
		i_Roulette2 = 5;
		yield return new WaitForSeconds( 1.5f );
		int Random_Ani = Random.Range( 0, 2 );
		if (Random_Ani == 0)
		{
			Spine_Result( PinAni3, false, 1.3f, "universe3" );
			Spine_Result( PinAni5, false, 1.3f, "pinpick" );
		}
		else if (Random_Ani == 1)
		{
			Spine_Result( PinAni3, false, 1, "power3" );
			Spine_Result( PinAni5, false, 1, "pinpick" );
		}
		i_Roulette3 = 5;
		StartCoroutine( Up() );
		i_Roulette_Result = 5;
	}

	IEnumerator Stop_BlackHole()
	{
		Spine_Result( PinAni, false, 1, "blackhole1" );
		i_Roulette1 = 1;
		yield return new WaitForSeconds( 1.5f );
		Spine_Result( PinAni2, false, 1, "blackhole2" );
		i_Roulette2 = 1;
		yield return new WaitForSeconds( 1.5f );
		int Random_Ani = Random.Range( 0, 2 );
		if (Random_Ani == 0)
		{
			Spine_Result( PinAni3, false, 1.3f, "universe3" );
			Spine_Result( PinAni5, false, 1.3f, "pinpick" );
			yield return new WaitForSeconds( 0.7f );
		}
		else if (Random_Ani == 1)
		{
			Spine_Result( PinAni3, false, 1, "blackhole3" );
			Spine_Result( PinAni5, false, 1, "pinpick" );
		}
		i_Roulette3 = 1;
		StartCoroutine( Up() );
		i_Roulette_Result = 4;
	}

	IEnumerator Stop_Fever()
	{
		Spine_Result( PinAni, false, 1, "fever1" );
		i_Roulette1 = 2;
		yield return new WaitForSeconds( 1.5f );
		Spine_Result( PinAni2, false, 1, "fever2" );
		i_Roulette2 = 2;
		yield return new WaitForSeconds( 1.5f );
		int Random_Ani = Random.Range( 0, 2 );
		if (Random_Ani == 0)
		{
			Spine_Result( PinAni3, false, 1.3f, "universe3" );
			Spine_Result( PinAni5, false, 1.3f, "pinpick" );
			yield return new WaitForSeconds( 1f );
		}
		else if (Random_Ani == 1)
		{
			Spine_Result( PinAni3, false, 1, "fever3" );
			Spine_Result( PinAni5, false, 1, "pinpick" );
		}
		i_Roulette3 = 2;
		StartCoroutine( Up() );
		i_Roulette_Result = 6;
	}

	IEnumerator Stop_Ticket()
	{
		Spine_Result( PinAni, false, 1, "ticket1" );
		i_Roulette1 = 7;
		yield return new WaitForSeconds( 1.5f );
		Spine_Result( PinAni2, false, 1, "ticket2" );
		i_Roulette2 = 7;
		yield return new WaitForSeconds( 1.5f );
		int Random_Ani = Random.Range( 0, 2 );
		if (Random_Ani == 0)
		{
			Spine_Result( PinAni3, false, 1.3f, "universe3" );
			Spine_Result( PinAni5, false, 1.3f, "pinpick" );
			yield return new WaitForSeconds( 0.7f );
		}
		else if (Random_Ani == 1)
		{
			Spine_Result( PinAni3, false, 1.3f, "ticket3" );
			Spine_Result( PinAni5, false, 1.3f, "pinpick" );
		}
		i_Roulette3 = 7;
		StartCoroutine( Up() );
		i_Roulette_Result = 3;
	}

	IEnumerator Stop_Gold()
	{
		Spine_Result( PinAni, false, 1, "gold1" );
		i_Roulette1 = 4;
		yield return new WaitForSeconds( 1.5f );
		Spine_Result( PinAni2, false, 1, "gold2" );
		i_Roulette2 = 4;
		yield return new WaitForSeconds( 1.5f );
		int Random_Ani = Random.Range( 0, 2 );
		if (Random_Ani == 0)
		{
			Spine_Result( PinAni3, false, 1.3f, "universe3" );
			Spine_Result( PinAni5, false, 1.3f, "pinpick" );
		}
		else if (Random_Ani == 1)
		{
			Spine_Result( PinAni3, false, 1, "gold3" );
			Spine_Result( PinAni5, false, 1, "pinpick" );
		}
		i_Roulette3 = 4;
		StartCoroutine( Up() );
		i_Roulette_Result = 1;
	}

	IEnumerator Stop_Oil()
	{
		Spine_Result( PinAni, false, 1, "fuel1" );
		i_Roulette1 = 3;
		yield return new WaitForSeconds( 1.5f );
		Spine_Result( PinAni2, false, 1, "fuel2" );
		i_Roulette2 = 3;
		yield return new WaitForSeconds( 1.5f );
		int Random_Ani = Random.Range( 0, 2 );
		if (Random_Ani == 0)
		{
			Spine_Result( PinAni3, false, 1.3f, "universe3" );
			Spine_Result( PinAni5, false, 1.3f, "pinpick" );
			yield return new WaitForSeconds( 0.7f );
		}
		else if (Random_Ani == 1)
		{
			Spine_Result( PinAni3, false, 1, "fuel3" );
			Spine_Result( PinAni5, false, 1, "pinpick" );
		}
		i_Roulette3 = 3;
		i_Roulette_Result = 2;
		StartCoroutine( Up() );
	}
}
