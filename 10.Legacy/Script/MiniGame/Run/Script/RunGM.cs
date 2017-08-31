using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System;

public class RunGM : MonoBehaviour {
	public static RunGM            instance;

	int                            i_Num_Score=0;//---------------------------점수
	int                            i_Tile_Count=0;//--------------------------타일번호
    int                            i_Item_percent=0;//------------------------특수노트 퍼센트
	int                            i_Combo=0;

	float                          f_Tile_dis;//----------------------------노트 거리

	bool                           b_Timebool;
	bool                           b_reverse;//-----------------------------특수효과(반대로)
	public bool                    b_SpeedUp;//-----------------------------특수효과(노트빠르게)
	public bool                    b_Boost;//-------------------------------특수효과(질주)
	public bool                    b_Stop;//--------------------------------특수효과(쉬는시간)


	public UILabel                 UIL_Score;//-----------------------------스코어 라벨
	public GameObject[]            Score;//---------------------------------스코어 애니메이션 프리팹

	public GameObject              g_Player,g_Enemy;
	public GameObject              g_Combo,g_ComboParent;
	public GameObject              g_Stop, g_RealStop;
	public GameObject              G_firstObject;//-------------------------가장 가까운 노트
	public GameObject              G_Time_bar;//----------------------------제한시간
	public GameObject              G_Tile_set;//----------------------------노트도착지점
	public GameObject              UIRoot;//--------------------------------생성노트의 상위 오브젝트

	public GameObject[]            G_Tile;//--------------------------------노트 프리팹
	public GameObject[]            Gs_Tile;//-------------------------------생성된 노트의 배열
	public GameObject[]            Background;//----------------------------배경

	Coroutine                      StopTime;//------------------------------Tile_Pattern코루틴 변수

	SkeletonAnimation              Player;
	SkeletonAnimation              Enemy;

	void Awake(){
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		Player = g_Player.GetComponent<SkeletonAnimation> ();
		Enemy = g_Enemy.GetComponent<SkeletonAnimation> ();
		b_SpeedUp = false;
		b_reverse = false;
		b_Stop = false;
		b_Boost = false;
		i_Item_percent = 0;
		i_Tile_Count = 0;
		i_Num_Score = 0;

		instance = this;
	}

	// Use this for initialization
	void Start () {
//		StopTime = StartCoroutine (Tile_Pattern ());
		StartCoroutine (Time_Value ());
		StartCoroutine (StartDelay ());
	}

	// Update is called once per frame
	void Update () {
		Gs_Tile = GameObject.FindGameObjectsWithTag ("Tile");

		UIL_Score.text = i_Num_Score.ToString ();

		if (G_Time_bar.GetComponent<UISlider> ().value >= 1) {
			G_Time_bar.GetComponent<UISlider> ().value = 1;
		} else if (G_Time_bar.GetComponent<UISlider> ().value <= 0 && !b_Timebool) 
		{
			G_Time_bar.GetComponent<UISlider> ().value = 0;
//			UIL_Score.text = "Score = " + i_Num_Score.ToString () + " Distance = " + Background[0].GetComponent<Offset>().i_count.ToString ();
			Time.timeScale = 0;
		}

		if (Gs_Tile.Length>0)//------------------------------------------------가장 가까운 오브젝트 변수화
		{
			Gs_Tile [0].GetComponent<Run_Tile> ().first = true;
		}
	}

	void Special_Note()
	{
		if (G_firstObject.GetComponent<Run_Tile> ().Red.Equals (true)) 
		{
			i_Item_percent = UnityEngine.Random.Range (1, 100 + 1);

			if (i_Item_percent <= 30) 
			{
				G_Time_bar.GetComponent<UISlider> ().value += 0.055f;
//				StartCoroutine (Boost ());//---------------------------임시로 텍스트에 시간을 부여 하기위해
			} else if (i_Item_percent <= 60) {
				b_Boost = true;
				StartCoroutine (Boost ());
				//질주
			} else if (i_Item_percent <= 80) {
				b_Stop = true;
				StopCoroutine (StopTime);
				StartCoroutine (Stop_note ());
				//쉬는시간
			} 
//			else if (i_Item_percent <= 90) {
//				b_reverse = true;
//				StartCoroutine (Reverse_Key ());
//				//반대로
//			} 
			else if (i_Item_percent <= 100) {
				b_SpeedUp = true;
				StartCoroutine (SpeedUp_Key ());
				//속도증가
			}
		}
	}
	public void A_Button()
	{
		if (!b_Boost && !b_Stop) 
		{
			if (G_firstObject != null)
			{
				f_Tile_dis = Vector2.Distance (G_Tile_set.transform.localPosition, G_firstObject.transform.localPosition);
			}

			if (!b_reverse) 
			{
				if (G_firstObject.GetComponent<Run_Tile> ().A.Equals (true))
				{
					if (f_Tile_dis > 60 && f_Tile_dis <= 90) {
						Special_Note ();
						i_Combo++;
						GameObject Font =Instantiate (g_Combo, g_ComboParent.transform)as GameObject;
						Font.GetComponent<UILabel> ().text = i_Combo.ToString();
						G_Tile_set.GetComponent<AudioSource> ().Play ();
						i_Num_Score += 1;
						Instantiate (Score [3], UIRoot.transform);
						Gs_Tile [1].GetComponent<Run_Tile> ().first = true;
						Destroy (Gs_Tile [0]);
					} else if (f_Tile_dis > 40 && f_Tile_dis <= 60) {
						Special_Note ();
						i_Combo++;
						GameObject Font =Instantiate (g_Combo, g_ComboParent.transform)as GameObject;
						Font.GetComponent<UILabel> ().text = i_Combo.ToString();
						G_Tile_set.GetComponent<AudioSource> ().Play ();
						i_Num_Score += 5;
						Instantiate (Score [2], UIRoot.transform);
						Gs_Tile [1].GetComponent<Run_Tile> ().first = true;
						Destroy (Gs_Tile [0]);
					} else if (f_Tile_dis > 20 && f_Tile_dis <= 40) {
						Special_Note ();
						i_Combo++;
						GameObject Font =Instantiate (g_Combo, g_ComboParent.transform)as GameObject;
						Font.GetComponent<UILabel> ().text = i_Combo.ToString();
						G_Tile_set.GetComponent<AudioSource> ().Play ();
						i_Num_Score += 7;
						Instantiate (Score [1], UIRoot.transform);
						Gs_Tile [1].GetComponent<Run_Tile> ().first = true;
						Destroy (Gs_Tile [0]);
					} else if (f_Tile_dis > 0 && f_Tile_dis <= 20) {
						Special_Note ();
						i_Combo++;
						GameObject Font =Instantiate (g_Combo, g_ComboParent.transform)as GameObject;
						Font.GetComponent<UILabel> ().text = i_Combo.ToString();
						G_Tile_set.GetComponent<AudioSource> ().Play ();
						i_Num_Score += 10;
						Instantiate (Score [0], UIRoot.transform);
						Gs_Tile [1].GetComponent<Run_Tile> ().first = true;
						Destroy (Gs_Tile [0]);
					} else if (f_Tile_dis > 90) {
						Instantiate (Score [4], UIRoot.transform);
						StartCoroutine (Stop_failed ());
						i_Combo = 0;
						b_Stop = true;
						StopCoroutine (StopTime);
					}
				} else {
					Instantiate (Score [4], UIRoot.transform);
					StartCoroutine (Stop_failed ());
					i_Combo = 0;
					b_Stop = true;
					StopCoroutine (StopTime);
				}
			} else {//-------------------------------------------------------------------반대로
				if (G_firstObject.GetComponent<Run_Tile> ().A.Equals (false)) {
				
					if (f_Tile_dis > 60 && f_Tile_dis <= 90) {
						G_Tile_set.GetComponent<AudioSource> ().Play ();
						i_Combo++;
						GameObject Font =Instantiate (g_Combo, g_ComboParent.transform)as GameObject;
						Font.GetComponent<UILabel> ().text = i_Combo.ToString();
						i_Num_Score += 1;
						Instantiate (Score [3], UIRoot.transform);
						Gs_Tile [1].GetComponent<Run_Tile> ().first = true;
						Destroy (Gs_Tile [0]);
					} else if (f_Tile_dis > 40 && f_Tile_dis <= 60) {
						G_Tile_set.GetComponent<AudioSource> ().Play ();
						i_Combo++;
						GameObject Font =Instantiate (g_Combo, g_ComboParent.transform)as GameObject;
						Font.GetComponent<UILabel> ().text = i_Combo.ToString();
						i_Num_Score += 5;
						Instantiate (Score [2], UIRoot.transform);
						Gs_Tile [1].GetComponent<Run_Tile> ().first = true;
						Destroy (Gs_Tile [0]);
					} else if (f_Tile_dis > 20 && f_Tile_dis <= 40) {
						G_Tile_set.GetComponent<AudioSource> ().Play ();
						i_Combo++;
						GameObject Font =Instantiate (g_Combo, g_ComboParent.transform)as GameObject;
						Font.GetComponent<UILabel> ().text = i_Combo.ToString();
						i_Num_Score += 7;
						Instantiate (Score [1], UIRoot.transform);
						Gs_Tile [1].GetComponent<Run_Tile> ().first = true;
						Destroy (Gs_Tile [0]);
					} else if (f_Tile_dis > 0 && f_Tile_dis <= 20) {
						G_Tile_set.GetComponent<AudioSource> ().Play ();
						i_Combo++;
						GameObject Font =Instantiate (g_Combo, g_ComboParent.transform)as GameObject;
						Font.GetComponent<UILabel> ().text = i_Combo.ToString();
						i_Num_Score += 10;
						Instantiate (Score [0], UIRoot.transform);
						Gs_Tile [1].GetComponent<Run_Tile> ().first = true;
						Destroy (Gs_Tile [0]);
					} else if (f_Tile_dis > 90) {
						Instantiate (Score [4], UIRoot.transform);
						StartCoroutine (Stop_failed ());
						b_Stop = true;
						i_Combo = 0;
						StopCoroutine (StopTime);
					}
				} else {
					Instantiate (Score [4], UIRoot.transform);
					StartCoroutine (Stop_failed ());
					b_Stop = true;
					i_Combo = 0;
					StopCoroutine (StopTime);
				}		
			}
		}
	}

	public void B_Button()
	{	
		if (!b_Boost && !b_Stop) {
			if (G_firstObject != null) {
				f_Tile_dis = Vector2.Distance (G_Tile_set.transform.localPosition, G_firstObject.transform.localPosition);
			}

			if (!b_reverse) {
				if (G_firstObject.GetComponent<Run_Tile> ().A.Equals (false)) 
				{					
					if (f_Tile_dis > 60 && f_Tile_dis <= 90) {
						Special_Note ();
						i_Combo++;
						GameObject Font =Instantiate (g_Combo, g_ComboParent.transform)as GameObject;
						Font.GetComponent<UILabel> ().text = i_Combo.ToString();
						G_Tile_set.GetComponent<AudioSource> ().Play ();
						i_Num_Score += 1;
						Instantiate (Score [3], UIRoot.transform);
						Gs_Tile [1].GetComponent<Run_Tile> ().first = true;
						Destroy (Gs_Tile [0]);
					} else if (f_Tile_dis > 40 && f_Tile_dis <= 60) {
						Special_Note ();
						i_Combo++;
						GameObject Font =Instantiate (g_Combo, g_ComboParent.transform)as GameObject;
						Font.GetComponent<UILabel> ().text = i_Combo.ToString();
						G_Tile_set.GetComponent<AudioSource> ().Play ();
						i_Num_Score += 5;
						Instantiate (Score [2], UIRoot.transform);
						Gs_Tile [1].GetComponent<Run_Tile> ().first = true;
						Destroy (Gs_Tile [0]);
					} else if (f_Tile_dis > 20 && f_Tile_dis <= 40) {
						Special_Note ();
						i_Combo++;
						GameObject Font =Instantiate (g_Combo, g_ComboParent.transform)as GameObject;
						Font.GetComponent<UILabel> ().text = i_Combo.ToString();
						G_Tile_set.GetComponent<AudioSource> ().Play ();
						i_Num_Score += 7;
						Instantiate (Score [1], UIRoot.transform);
						Gs_Tile [1].GetComponent<Run_Tile> ().first = true;
						Destroy (Gs_Tile [0]);
					} else if (f_Tile_dis > 0 && f_Tile_dis <= 20) {
						Special_Note ();
						i_Combo++;
						GameObject Font =Instantiate (g_Combo, g_ComboParent.transform)as GameObject;
						Font.GetComponent<UILabel> ().text = i_Combo.ToString();
						G_Tile_set.GetComponent<AudioSource> ().Play ();
						i_Num_Score += 10;
						Instantiate (Score [0], UIRoot.transform);
						Gs_Tile [1].GetComponent<Run_Tile> ().first = true;
						Destroy (Gs_Tile [0]);
					} else if (f_Tile_dis > 90) {
						Instantiate (Score [4], UIRoot.transform);
						StartCoroutine (Stop_failed ());
						b_Stop = true;
						i_Combo = 0;
						StopCoroutine (StopTime);
					}
				} else {
					Instantiate (Score [4], UIRoot.transform);
					StartCoroutine (Stop_failed ());
					b_Stop = true;
					i_Combo = 0;
					StopCoroutine (StopTime);
				}
			} else {//----------------------------------------------------------------반대로
				if (G_firstObject.GetComponent<Run_Tile> ().A.Equals (true)) {
				
					if (f_Tile_dis > 60 && f_Tile_dis <= 90) {
						G_Tile_set.GetComponent<AudioSource> ().Play ();
						i_Combo++;
						GameObject Font =Instantiate (g_Combo, g_ComboParent.transform)as GameObject;
						Font.GetComponent<UILabel> ().text = i_Combo.ToString();
						i_Num_Score += 1;
						Instantiate (Score [3], UIRoot.transform);
						Gs_Tile [1].GetComponent<Run_Tile> ().first = true;
						Destroy (Gs_Tile [0]);
					} else if (f_Tile_dis > 40 && f_Tile_dis <= 60) {
						G_Tile_set.GetComponent<AudioSource> ().Play ();
						i_Combo++;
						GameObject Font =Instantiate (g_Combo, g_ComboParent.transform)as GameObject;
						Font.GetComponent<UILabel> ().text = i_Combo.ToString();
						i_Num_Score += 5;
						Instantiate (Score [2], UIRoot.transform);
						Gs_Tile [1].GetComponent<Run_Tile> ().first = true;
						Destroy (Gs_Tile [0]);
					} else if (f_Tile_dis > 20 && f_Tile_dis <= 40) {
						G_Tile_set.GetComponent<AudioSource> ().Play ();
						i_Combo++;
						GameObject Font =Instantiate (g_Combo, g_ComboParent.transform)as GameObject;
						Font.GetComponent<UILabel> ().text = i_Combo.ToString();
						i_Num_Score += 7;
						Instantiate (Score [1], UIRoot.transform);
						Gs_Tile [1].GetComponent<Run_Tile> ().first = true;
						Destroy (Gs_Tile [0]);
					} else if (f_Tile_dis > 0 && f_Tile_dis <= 20) {
						G_Tile_set.GetComponent<AudioSource> ().Play ();
						i_Combo++;
						GameObject Font =Instantiate (g_Combo, g_ComboParent.transform)as GameObject;
						Font.GetComponent<UILabel> ().text = i_Combo.ToString();
						i_Num_Score += 10;
						Instantiate (Score [0], UIRoot.transform);
						Gs_Tile [1].GetComponent<Run_Tile> ().first = true;
						Destroy (Gs_Tile [0]);
					} else if (f_Tile_dis > 90) {
						Instantiate (Score [4], UIRoot.transform);
						StartCoroutine (Stop_failed ());
						b_Stop = true;
						i_Combo = 0;
						StopCoroutine (StopTime);
					}
				} else {
					Instantiate (Score [4], UIRoot.transform);
					StartCoroutine (Stop_failed ());
					b_Stop = true;
					i_Combo = 0;
					StopCoroutine (StopTime);
				}
			}
		}
	}

	IEnumerator Tile_Pattern()
	{
		//--------------------------------현재 ABABABBBABABABA		
		for (int i = 0; i < 3; i++) {
			yield return new WaitForSeconds (0.5f);
			if (i_Tile_Count % 10 == 0 && i_Tile_Count != 0) {
				Instantiate (G_Tile [2], UIRoot.transform);
			} else {
				Instantiate (G_Tile [0], UIRoot.transform);
			}
			i_Tile_Count++;
			yield return new WaitForSeconds (0.5f);
			if (i_Tile_Count % 10 == 0 && i_Tile_Count != 0) {
				Instantiate (G_Tile [3], UIRoot.transform);
			} else {
				Instantiate (G_Tile [1], UIRoot.transform);
			}
			i_Tile_Count++;
		}

		yield return new WaitForSeconds (0.7f);
		if (i_Tile_Count % 10 == 0 && i_Tile_Count != 0) {
			Instantiate (G_Tile [3], UIRoot.transform);
		} else {
			Instantiate (G_Tile [1], UIRoot.transform);
		}
		i_Tile_Count++;

		for (int i = 0; i < 4; i++) {
			yield return new WaitForSeconds (0.4f);
			if (i_Tile_Count % 10 == 0 && i_Tile_Count != 0) {
				Instantiate (G_Tile [3], UIRoot.transform);
			} else {
				Instantiate (G_Tile [1], UIRoot.transform);
			}
			i_Tile_Count++;
			yield return new WaitForSeconds (0.4f);
			if (i_Tile_Count % 10 == 0 && i_Tile_Count != 0) {
				Instantiate (G_Tile [2], UIRoot.transform);
			} else {
				Instantiate (G_Tile [0], UIRoot.transform);
			}
			i_Tile_Count++;
		}
		StopTime = StartCoroutine (Tile_Pattern ());
	}

	IEnumerator Time_Value()//--------------------------------------------------시간초
	{
		yield return new WaitForSeconds (1f);
		G_Time_bar.GetComponent<UISlider> ().value-= 0.0055f;
		StartCoroutine (Time_Value ());
	}

	IEnumerator Boost()//-------------------------------------------------------질주
	{
		for (int i = 0; i < Gs_Tile.Length; i++) 
		{
			if (Gs_Tile [i].GetComponent<UISprite> ().spriteName == "knot")
			{
				Gs_Tile [i].GetComponent<UISprite> ().spriteName = "GoldNote";
			}else if(Gs_Tile [i].GetComponent<UISprite> ().spriteName == "knot2")
			{
				Gs_Tile [i].GetComponent<UISprite> ().spriteName = "GoldNote";
			}
		}
		yield return new WaitForSeconds (3);
		b_Boost = false;
		for (int i = 0; i < Gs_Tile.Length; i++) 
		{
			if (Gs_Tile [i].GetComponent<UISprite> ().spriteName == "GoldNote")
			{
				Gs_Tile [i].GetComponent<UISprite> ().spriteName = Gs_Tile [i].GetComponent<Run_Tile> ().StartImage;
			}else if(Gs_Tile [i].GetComponent<UISprite> ().spriteName == "GoldNote")
			{
				Gs_Tile [i].GetComponent<UISprite> ().spriteName = Gs_Tile [i].GetComponent<Run_Tile> ().StartImage;
			}
		}
	}

	IEnumerator Stop_failed()//-------------------------------------------------------스턴
	{
		for (int i = 0; i < Gs_Tile.Length; i++) 
		{
			if(Gs_Tile[i]!=null)
			Gs_Tile [i].GetComponent<UISprite> ().spriteName = "knot5";
		}
		Player.loop = true;
		Player.AnimationName = "run 03";
		yield return new WaitForSeconds (2);
		Player.loop = true;
		Player.AnimationName = "run";
		for (int i = 0; i < Gs_Tile.Length; i++) 
		{
			Gs_Tile [i].GetComponent<UISprite> ().spriteName = Gs_Tile [i].GetComponent<Run_Tile> ().StartImage;
		}
		b_Stop = false;
		StopTime = StartCoroutine (Tile_Pattern ());
	}

	IEnumerator Stop_note()//--------------------------------------------------------일시정지
	{
		yield return new WaitForSeconds (5);
		b_Stop = false;
		b_Boost = true;
		StopTime = StartCoroutine (Tile_Pattern ());
		StartCoroutine (Boost ());
	}

	IEnumerator Reverse_Key()//--------------------------------------------------반대로
	{
		yield return new WaitForSeconds (5f);
		b_reverse = false;
	}

	IEnumerator SpeedUp_Key()//-------------------------------------------------노트속도증가
	{
		yield return new WaitForSeconds (5f);
		b_SpeedUp = false;
	}

	IEnumerator StartDelay()
	{
		yield return new WaitForSeconds (3f);
		Player.loop = true;
		Player.AnimationName = "run";
		Enemy.loop = true;
		Enemy.AnimationName = "run normal";
		Background [0].GetComponent<Offset> ().b_Startbool = true;
		Background [1].GetComponent<Offset> ().b_Startbool = true;
		StopTime = StartCoroutine (Tile_Pattern ());
	}

	public void Stop_Button()
	{
		if (!b_Stop) {
			g_Stop.SetActive (true);
			b_Stop = true;
			Time.timeScale = 0;
		} else {
			g_Stop.SetActive (false);
			b_Stop = false;
			Time.timeScale = 1;
		}
	}

	public void Stop_RePlay()
	{
		b_Stop = false;
		Time.timeScale = 1;
		g_Stop.SetActive (false);
	}

	public void Stop_Main()
	{
		b_Timebool = true;
		Time.timeScale = 1;
		PCManagerFramework.DoLoadScene_FadeInOut( ESceneName.OutGame, 1f, Color.black );
	}

	public void Stop_Exit()
	{
		g_Stop.SetActive (false);
		g_RealStop.SetActive (true);
	}

	public void Stop_Back()
	{
		g_Stop.SetActive (true);
		g_RealStop.SetActive (false);
	}

	public void AppQuit()
	{
		Application.Quit ();
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (b_Boost)
		{
			if (coll.gameObject)
			{
				i_Combo++;
				GameObject Font =Instantiate (g_Combo, g_ComboParent.transform)as GameObject;
				Font.GetComponent<UILabel> ().text = i_Combo.ToString();
				G_Tile_set.GetComponent<AudioSource> ().Play ();
				i_Num_Score += 10;
				Instantiate (Score [0], UIRoot.transform);
				Gs_Tile [1].GetComponent<Run_Tile> ().first = true; 
				Destroy (Gs_Tile[0]);			
			}
		}
	}
}
