using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* ============================================ 
   Editor      : KJH
   Description : 
   Edit Log    : 
   ============================================ */

public class PCUIOutFrame_SelectMission : CUIFrameBase, IButton_OnClickListener<PCUIOutFrame_SelectMission.EUIButton>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EUIButton
	{
		Button_Left,
		Button_Right,

		Button_Stage_1,
		Button_Stage_2,
		Button_Stage_3,
		Button_Stage_4,
		Button_Stage_5,
		Button_Stage_6,
		Button_Stage_7,
		Button_Stage_8,
		Button_Stage_9,
		Button_Stage_10,

		Button_PowerUp,
		Button_FuelUp,
		Button_LifeUp,
		Button_GoldBonus,
		Button_PlayGame_Mission,
		Button_SelectTeam_Back,

		Button_Check,
		Button_SelectStage_Back,

		Button_Ship_Left,
		Button_Ship_Right
	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	[SerializeField]
	private GameObject _pObjectSelectTeam = null;

	private int _iSelectStage = 0;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void IOnClick_Buttons( EUIButton eButtonName )
	{
		switch (eButtonName)
		{
			case EUIButton.Button_Left:
				break;
			case EUIButton.Button_Right:
				break;

			case EUIButton.Button_Stage_1:
				_iSelectStage = 1;
				break;
			case EUIButton.Button_Stage_2:
				_iSelectStage = 2;
				break;
			case EUIButton.Button_Stage_3:
				_iSelectStage = 3;
				break;
			case EUIButton.Button_Stage_4:
				_iSelectStage = 4;
				break;
			case EUIButton.Button_Stage_5:
				_iSelectStage = 5;
				break;
			case EUIButton.Button_Stage_6:
				_iSelectStage = 6;
				break;
			case EUIButton.Button_Stage_7:
				_iSelectStage = 7;
				break;
			case EUIButton.Button_Stage_8:
				_iSelectStage = 8;
				break;
			case EUIButton.Button_Stage_9:
				_iSelectStage = 9;
				break;
			case EUIButton.Button_Stage_10:
				_iSelectStage = 10;
				break;

			case EUIButton.Button_PowerUp:
				break;
			case EUIButton.Button_FuelUp:
				break;
			case EUIButton.Button_LifeUp:
				break;
			case EUIButton.Button_GoldBonus:
				break;

			case EUIButton.Button_PlayGame_Mission:
				//PCManagerFramework.DoLoadScene_FadeInOut( ESceneName.Mission, 1f, Color.black, OnFinishloadMissionGame );
				break;

			case EUIButton.Button_SelectTeam_Back:
				_pObjectSelectTeam.SetActive( false );
				break;

			case EUIButton.Button_Check:
				PCManagerFramework.DoLoadScene_FadeInOut(ESceneName.InGame_Mission, 1f, Color.black, OnFinishloadMissionGame);
				//_pObjectSelectTeam.SetActive( true );
				break;

			case EUIButton.Button_SelectStage_Back:
				PCManagerUIOutGame.instance.DoChangeFrame_FadeInout( PCManagerUIOutGame.EUIFrame.PCUIOutFrame_SelectMission, PCManagerUIOutGame.EUIFrame.PCUIOutFrame_MainMenu, 1f );


				break;
			case EUIButton.Button_Ship_Left:
				break;
			case EUIButton.Button_Ship_Right:
				break;
		}
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	protected override void OnShow( int iSortOrder )
	{
		base.OnShow( iSortOrder );

		_pObjectSelectTeam.SetActive( false );
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private void OnFinishloadMissionGame()
	{
		PCManagerFramework.DoLoadSceneAsync( ESceneName.InGameUI_Mission, UnityEngine.SceneManagement.LoadSceneMode.Additive, OnFinishloadMissionGame_UI );
	}

	private void OnFinishloadMissionGame_UI()
	{
		PCManagerInMission.instance.DoStartGame(1);
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
