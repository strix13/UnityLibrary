using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class TowerGM : MonoBehaviour {
	public static TowerGM             instance;
	public GameObject                 g_Tile;
	public GameObject                 g_TilePrefab;
	public GameObject                 g_UIparent;
	public GameObject                 g_World;
	public GameObject                 g_Crane;
	public GameObject                 g_Cam;
	public GameObject                 g_fever;
	public GameObject                 g_Background;
	public GameObject                 g_feverEffect;
	public GameObject                 g_FeverText;
	public GameObject                 g_StopUI;
	public GameObject                 g_GameOver;
	public GameObject                 g_Start;
	public GameObject                 g_beforeTile;
	public bool                       b_CamMove=false;
	bool                              b_CraneMove=false;
	bool                              b_GameOver = false;
	public bool                       b_fever=false;

	public UILabel                    UIL_BestScore;
	public UILabel                    UIL_FinalScore;
	public UILabel                    UIL_Score;
	public UILabel                    UIL_Stage;

	public int                        i_Tile_Num;
	public int                        i_Score;
	public float                      f_fever;
	float                             f_Time;
	float                             f_CraneSpeed;
	float                             f_CamYPlus;
	float                             f_Background;

	bool                              b_Cranevector=false;

	bool                              b_GameStart;

	SkeletonAnimation                 Spine_CraneAni;

	void Awake(){
		instance = this;
		b_CraneMove = true;
		b_Cranevector = false;
		i_Tile_Num = 0;
		f_CraneSpeed = 0.5f;
		f_fever = 0.5f;
		f_Background = 640;
		Spine_CraneAni = g_Crane.GetComponent<SkeletonAnimation> ();
	}

	// Use this for initialization
	void Start () {
		StartCoroutine (GameStart ());
		StartCoroutine (GameTime ());
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey (KeyCode.Escape)) 
		{
			g_StopUI.SetActive (true);
		}

		if (UIL_FinalScore.isActiveAndEnabled.Equals (true)) 
		{
			UIL_FinalScore.text = i_Score.ToString ();
		}
		if (UIL_BestScore.isActiveAndEnabled.Equals (true)) 
		{
			UIL_BestScore.text = i_Score.ToString ();
		}
		g_Background.transform.localPosition = new Vector2 (g_Cam.transform.localPosition.x, g_Background.transform.localPosition.y);
		if (b_GameStart) {
			if(!b_GameOver)
			f_Time +=Time.deltaTime;
			f_CraneSpeed += 0.01f*Time.deltaTime;
			g_fever.GetComponent<UISlider> ().value = f_fever;
			UIL_Score.text = i_Score.ToString ();
			UIL_Stage.text = i_Tile_Num.ToString ();
			g_Cam.transform.localPosition = Vector2.MoveTowards (g_Cam.transform.localPosition, new Vector2 (g_Cam.transform.localPosition.x, f_CamYPlus), 770 * Time.deltaTime);
			g_Background.transform.localPosition = Vector2.MoveTowards (g_Background.transform.localPosition, new Vector2 (g_Background.transform.localPosition.x, f_Background), 770 * Time.deltaTime);
			if (g_Background.transform.localPosition.y <= -640) {
				f_Background = 640;
				g_Background.transform.localPosition = new Vector2 (transform.localPosition.x, 640);
			}
			if (f_fever >= 1) {
				f_fever = 1;
				StartCoroutine (feverTime ());
			}

			if (f_fever < 0) {
				f_fever = 0;
				StartCoroutine (GameOver ());
			}
			if (b_CraneMove) {
				if (!b_Cranevector) {
					g_Crane.transform.Translate (Vector2.left * f_CraneSpeed * Time.deltaTime);
				} else {
					g_Crane.transform.Translate (Vector2.right * f_CraneSpeed * Time.deltaTime);
				}

				if (g_Crane.transform.localPosition.x <= -173)
					b_Cranevector = true;
				else if (g_Crane.transform.localPosition.x >= 173)
					b_Cranevector = false;

				g_Tile.transform.localPosition = new Vector2 (g_Crane.transform.localPosition.x, g_Tile.transform.localPosition.y);
			}
		}
	}

	public void BackgroundButton()
	{
		if (b_GameStart) {
			if (b_CraneMove) {
				b_CraneMove = false;
				g_Tile.gameObject.transform.parent = g_World.transform;
				g_Tile.GetComponent<Rigidbody2D> ().simulated = true;
				if (g_beforeTile != null) 
				{
					if (Mathf.Abs (g_Tile.transform.localPosition.x - g_beforeTile.transform.localPosition.x) < 25) 
					{
						g_Tile.transform.localPosition = new Vector2 (g_beforeTile.transform.localPosition.x, g_Tile.transform.localPosition.y);
					}
				}
				Spine_CraneAni.loop = false;
				Spine_CraneAni.AnimationName = "close";
				StartCoroutine (CraneMove ());
			}
		}
	}

	public void Stop()
	{
		Time.timeScale = 0;
		g_StopUI.SetActive (true);
	}

	public void Continue()
	{
		Time.timeScale = 1;
		g_StopUI.SetActive (false);
	}

	public void Exit()
	{
		Time.timeScale = 1;
		PCManagerFramework.DoLoadScene_FadeInOut( ESceneName.OutGame, 1f, Color.black );
	}

	IEnumerator GameStart()
	{
		yield return new WaitForSeconds (3);
		g_Start.SetActive (false);
		b_GameStart = true;
	}

	IEnumerator GameOver()
	{
		yield return new WaitForSeconds (0f);
		b_GameOver = true;
		g_GameOver.SetActive (true);
	}

	IEnumerator CraneMove()
	{
		yield return new WaitForSeconds (0.2f);
		if (!b_CamMove)
		{
			b_CamMove = true;
			if (i_Tile_Num > 3) 
			{
				f_CamYPlus = g_Cam.transform.localPosition.y + 100;
				f_Background = g_Background.transform.localPosition.y - 100;
			}
		}

		Spine_CraneAni.loop = true;
		Spine_CraneAni.AnimationName = "run";
		yield return new WaitForSeconds (0.5f);
		g_beforeTile = g_Tile;
		GameObject Tile = Instantiate (g_TilePrefab, g_UIparent.transform)as GameObject;
			Tile.transform.localScale = new Vector2 (87, 87);
			Tile.transform.localPosition = new Vector2 (g_Crane.transform.localPosition.x, 185);
			Tile.GetComponent<MeshRenderer> ().sortingOrder = g_beforeTile.GetComponent<MeshRenderer> ().sortingOrder + 1;
		g_Tile = Tile;
		b_CraneMove = true;
	}
	IEnumerator feverText()
	{
		g_FeverText.SetActive (true);
		yield return new WaitForSeconds (1);
		g_FeverText.SetActive (false);
	}

	IEnumerator GameTime()
	{
		while (f_fever > 0) {
			f_fever -= 0.001f;
			yield return new WaitForSeconds (0.01f);
		}
	}

	IEnumerator feverTime()
	{
		StartCoroutine (feverText ());
		b_fever = true;
		g_feverEffect.SetActive (true);
		g_Background.GetComponent<UISprite>().spriteName = "fever background";
		yield return new WaitForSeconds (10f);
		b_fever = false;
		g_feverEffect.SetActive (false);
		g_Background.GetComponent<UISprite>().spriteName = "background00";
	}
}
