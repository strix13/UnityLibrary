using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class PCUIOutFrame_MainMenu : CUIFrameBase, IButton_OnClickListener<PCUIOutFrame_MainMenu.EUIButton>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EUIButton
	{
		Button_MiniGame_Run,
		Button_MiniGame_Tower,
		Button_MiniGame_PickNum,

		Button_MiniGame_Select,
		Button_MiniGame_Cancel,

		Button_Tap,
		Button_Hide,
		Button_Notice,
		Button_Mail,

		Button_Tab,
		Button_Walk,
		Button_Achievement,
		Button_Character,
		Button_Run,
		Button_Mission,
		Button_Shop,
		Button_MiniGame
	}

	public enum EMiniGame
	{
		Nothing = -1,
		Run_2,
		Tower,
		PickNum
	}

	public EMiniGame _eMiniGame = EMiniGame.Nothing;

	public GameObject[] gs_MinigameTicketEffect;
	public GameObject[] gs_MInigameButtons;
	public GameObject[] gs_UIEffect;
	public GameObject g_TicketAni;
	public GameObject g_TouchEvent, g_TouchParent;//---------터치 이펙트, 터치이펙트 최상위 오브젝트
	public GameObject g_RouletteScene, g_Mode;//-룰렛씬,골드룰렛씬,모드씬 UI최상위 오브젝트

	public GameObject g_Tab_bar;
	public GameObject g_Tab;

	public GameObject g_MiniGame;
	public GameObject g_HideUI;


	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private GameObject _pButtonHide;

	float f_Tab_Num = 0;
	bool b_HideUI;
	bool b_Mode = false;//---------------------Walk모드인지 Tap모드인지
	int i_EventPercent = 0;//-------------------발동 확률


	//----------------------------------------------------------------------------------------------------------------------페도미터
	//	public UILabel                   acc;

	float loLim = 0.005f;
	float hiLim = 0.1f;
	float fHigh = 10.0f;
	float curAcc = 0f;
	float fLow = 0.1f;
	float avgAcc = 0f;

	int steps = 0;

	bool stateH = false;
	bool TimeLimit = false;

	//---------------------------------------------------------------------------------------------------------------------탭모드

	private List<PCCharacterAnimation> _listCharacter = new List<PCCharacterAnimation>();

	int i_TapCount = 0;


	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void IOnClick_Buttons( EUIButton eButtonName )
	{
		switch (eButtonName)
		{
			case EUIButton.Button_MiniGame_Run:
				ProcSelectMiniGame( EMiniGame.Run_2 );
				break;

			case EUIButton.Button_MiniGame_Tower:
				ProcSelectMiniGame( EMiniGame.Tower );
				break;

			case EUIButton.Button_MiniGame_PickNum:
				ProcSelectMiniGame( EMiniGame.PickNum );
				break;

			case EUIButton.Button_MiniGame_Select:
				MiniGameEnter();
				break;

			case EUIButton.Button_MiniGame_Cancel:
				MiniGame_Back();
				break;

			case EUIButton.Button_Tap:
				Tap();
				break;

			case EUIButton.Button_Hide:
				HideUI();
				break;

			case EUIButton.Button_Notice:
				break;

			case EUIButton.Button_Mail:
				break;

			case EUIButton.Button_Tab:
				Tap_Button();
				break;

			case EUIButton.Button_Walk:
				break;

			case EUIButton.Button_Achievement:
				break;

			case EUIButton.Button_Character:
				PCManagerUIOutGame.instance.DoChangeFrame_FadeInout( PCManagerUIOutGame.EUIFrame.PCUIOutFrame_MainMenu, PCManagerUIOutGame.EUIFrame.PCUIOutFrame_SelectCharacter, 1f );
				break;

			case EUIButton.Button_Run:
				break;

			case EUIButton.Button_Mission:
				Mission();
				break;

			case EUIButton.Button_Shop:
				break;

			case EUIButton.Button_MiniGame:
				MiniGames();
				break;
		}
	}

	public void DoShowHideUI_TabBar(bool bShow)
	{
		g_Tab_bar.SetActive(bShow);
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		Screen.orientation = ScreenOrientation.Portrait;
		avgAcc = Input.acceleration.magnitude;

		_pButtonHide = GetUIButton( EUIButton.Button_Hide ).gameObject;
		GetComponentsInChildren<PCCharacterAnimation>( _listCharacter );
		_listCharacter.Sort();
		b_HideUI = true;


		for (int i = 0; i < 3; i++)
		{
			gs_MInigameButtons[i].GetComponent<UIButton>().normalSprite = "unselect button";
		}
	}

	protected override void OnShow( int iSortOrder )
	{
		base.OnShow( iSortOrder );

		ProcShowCharacter_OnlyOne( PCManagerFramework.p_pInfoUser.eCharacterCurrent );
		PCUIOutFrame_MainTop pUITop = PCManagerUIOutGame.instance.GetUIFrame<PCUIOutFrame_MainTop>();
		pUITop.DoSetIsShowMainMenu( true );
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		
		g_Tab.GetComponent<UISlider>().value = f_Tab_Num;
		
		if (b_Mode)
		{
			if (Input.touchCount == 1)
			{
				Touch touch = Input.GetTouch( 0 );

				if (touch.phase == TouchPhase.Began)
				{
					GameObject TouchEvent = Instantiate( g_TouchEvent, g_TouchParent.transform ) as GameObject;
					TouchEvent.transform.position = Camera.main.ScreenToWorldPoint( touch.position );
					TouchEvent.transform.localScale = new Vector3( 62, 62, 62 );
					Destroy( TouchEvent.gameObject, 0.4f );
				}
			}
		}

		if (g_HideUI.transform.localPosition.x > 467)
		{
			g_HideUI.transform.localPosition = new Vector2( 467, 0 );
		}
		else if (g_HideUI.transform.localPosition.x < 56)
		{
			g_HideUI.transform.localPosition = new Vector2( 56, 0 );
		}

		if (b_HideUI)
		{
			if (g_HideUI.transform.localPosition.x < 467)
			{
				g_HideUI.transform.Translate( Vector2.right * 3 * Time.deltaTime );
			}
		}
		else
		{
			if (g_HideUI.transform.localPosition.x > 56)
			{
				g_HideUI.transform.Translate( Vector2.left * 3 * Time.deltaTime );
			}
		}

	}

	void RouletteScene()//------------------------------------룰렛 발동 확률
	{
		i_EventPercent = Random.Range( 0, 101 );
	}

	void FixedUpdate()
	{
		if (!b_Mode && steps <= 5)
		{
			RouletteScene();
		}

		if (b_Mode && i_TapCount <= 10)
		{
			RouletteScene();
		}

		if (steps >= 10)
		{
			steps = 0;
			//			i_StageCount++;
			//			if ((i_EventPercent <= i_pedoPercent) && !b_Mode) 
			//			{
			g_RouletteScene.SetActive( true );
			StartCoroutine( g_RouletteScene.GetComponent<CircleRoulette>().Anistart() );
			//			} 
			//		else if ((i_EventPercent <= i_pedoPercent + i_GoldPedo) && !b_Mode) {
			//				g_GoldScene.SetActive (true);
			//			}
			//			steps = 0;
		}

		if (f_Tab_Num >= 1)
		{
			f_Tab_Num = 0;
			//			if ((i_EventPercent <= i_tapPercent) && b_Mode) 
			//			{
			g_RouletteScene.SetActive( true );
			StartCoroutine( g_RouletteScene.GetComponent<CircleRoulette>().Anistart() );
			//			} else if ((i_EventPercent <= i_tapPercent + i_GoldTap) && b_Mode) {
			//				g_GoldScene.SetActive (true);
			//			}
			//			f_Tab_Num = 0;
		}

		if (!b_Mode)
		{//-------------------------------------------------------------------------------------페도미터
			curAcc = Mathf.Lerp( curAcc, Input.acceleration.magnitude, Time.deltaTime * fHigh );
			avgAcc = Mathf.Lerp( avgAcc, Input.acceleration.magnitude, Time.deltaTime * fLow );

			if (avgAcc < 0.9)
			{
				avgAcc = 0.9f;
			}

			if (!TimeLimit)
			{
				float delta = curAcc - avgAcc;
				if (!stateH)
				{
					if (delta > hiLim)
					{
						stateH = true;
						steps++;
						//						acc.text = "steps:" + steps;
						TimeLimit = true;
						StartCoroutine( "TimeLimit_Clear" );
					}
				}
				else
				{
					if (delta < loLim)
					{
						stateH = false;
					}
				}
			}
		}
	}


	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	IEnumerator TimeLimit_Clear()
	{//----------------------------------------카운트0.5초 쿨타임
		yield return new WaitForSeconds( 0.5f );
		TimeLimit = false;
	}

	IEnumerator MiniGame_Enter()
	{
		gs_MinigameTicketEffect[0].SetActive( true );

		yield return new WaitForSeconds( 1f );

		PCManagerFramework.p_pDataGame.iExe++;
		PCManagerFramework.DoLoadScene( _eMiniGame.ToString(), UnityEngine.SceneManagement.LoadSceneMode.Single );
	}

	public void MiniGameEnter()
	{
		if (_eMiniGame != EMiniGame.Nothing)
		{
			if (PCManagerFramework.p_pDataGame.iTicket >= 1)
			{
				PCManagerFramework.p_pDataGame.iTicket--;
				StartCoroutine( MiniGame_Enter() );
			}
		}
	}

	public void MiniGame_Back()
	{
		g_MiniGame.SetActive( false );
	}

	public void Tap_Button()
	{
		if (b_Mode)
		{
			f_Tab_Num += 0.02f;
			//			i_TapCount++;
			//			acc.text = "TapCount : " + i_TapCount.ToString();
		}
	}

	public void Tap()
	{
		g_Tab_bar.SetActive( true );
		if (!b_Mode)
		{
			b_Mode = true;
			steps = 0;
		}
	}

	public void MiniGames()
	{
		g_MiniGame.SetActive( g_MiniGame.activeInHierarchy.Equals( false ) );
	}

	public void Walk()
	{
		if (b_Mode)
		{
			TouchEffect.instance.StopAllCoroutines();
			b_Mode = false;
			g_Tab_bar.SetActive( false );
			f_Tab_Num = 0;
			//			acc.text = "Steps : " + steps.ToString();			
		}
	}

	public void MiniGame()
	{
		ProcSelectMiniGame( EMiniGame.Run_2 );
	}

	public void Tower()
	{
		ProcSelectMiniGame( EMiniGame.Tower );
	}

	public void NumberGame()
	{
		ProcSelectMiniGame( EMiniGame.PickNum );
	}

	private void ProcSelectMiniGame( EMiniGame eMiniGame )
	{
		_eMiniGame = eMiniGame;
		for (int i = 0; i < 3; i++)
			gs_MInigameButtons[i].GetComponent<UIButton>().normalSprite = "unselect button";

		gs_MInigameButtons[(int)eMiniGame].GetComponent<UIButton>().normalSprite = "select button";
	}

	public void Mission()
	{
		PCManagerFramework.p_pDataGame.iExe++;
		//PCManagerUIOutGame.instance.DoChangeFrame_FadeInout( PCManagerUIOutGame.EUIFrame.PCUIOutFrame_MainMenu, PCManagerUIOutGame.EUIFrame.PCUIOutFrame_SelectMission, 1f );
		PCManagerFramework.p_pManagerSound.DoStopAllSound();
		PCManagerFramework.DoLoadScene_FadeInOut(ESceneName.InGame_Mission, 1f, Color.black, OnFinishloadMissionGame );
	}

	public void HideUI()
	{
		if (b_HideUI)
			_pButtonHide.transform.localScale = new Vector3( -1, 1, 1 );
		else
			_pButtonHide.transform.localScale = new Vector3( 1, 1, 1 );

		b_HideUI = !b_HideUI;
	}


	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	private void ProcShowCharacter_OnlyOne( ECharacterName eCharacterName )
	{
		int iCharacterIndex = (int)eCharacterName;
		_listCharacter[iCharacterIndex].gameObject.SetActive( true );
		for (int i = 0; i < _listCharacter.Count; i++)
		{
			if (i != iCharacterIndex)
				_listCharacter[i].gameObject.SetActive( false );
		}
	}

	private void OnFinishloadMissionGame()
	{
		PCManagerFramework.DoLoadSceneAsync( ESceneName.InGameUI_Mission, UnityEngine.SceneManagement.LoadSceneMode.Additive, OnFinishloadMissionGame_UI );
	}

	private void OnFinishloadMissionGame_UI()
	{
		PCManagerInMission.instance.DoStartGame( 1 );
	}
}
