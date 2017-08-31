using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PCManagerUIOutGame : CManagerUIBase<PCManagerUIOutGame, PCManagerUIOutGame.EUIFrame, PCManagerUIOutGame.EUIPopup>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EUIFrame
	{
		PCUIOutFrame_MainTop,
		PCUIOutFrame_MainMenu,
		PCUIOutFrame_Background,
		PCUIOutFrame_SelectMission,
		PCUIOutFrame_SelectCharacter,
	}

	public enum EUIPopup
	{

	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	// ========================================================================== //

	/* public - [Do] Function
       외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
	   자식 객체가 호출(For Child class call)	*/

	private void OnFinishLoad_Database()
	{
		OnDefaultFrameShow();
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		if (PCManagerFramework.instance == null)
		{
			PCManagerFramework.DoLoadScene(ESceneName.Common, UnityEngine.SceneManagement.LoadSceneMode.Additive);
			PCManagerFramework.p_EVENT_OnDBLoadFinish += OnFinishLoad_Database;
		}

		DoPlayBGMLoop();
	}

	public void DoPlayBGMLoop()
	{
		if (PCManagerFramework.p_pManagerSound.DoCheckIsPlayingBGM( ESoundName.BGM_Title_LenskoCetus ) == false)
			PCManagerFramework.p_pManagerSound.DoPlayBGM(ESoundName.BGM_Title_LenskoCetus, DoPlayBGMLoop );
	}


	protected override void OnDefaultFrameShow()
	{
		PCManagerFramework.DoSetTimeScale(1f);

		if (PCManagerFramework.instance == null) return;

		DoShowHide_Frame(EUIFrame.PCUIOutFrame_Background, true);

		// TEST
		//PCManagerFramework.p_pDataGame.bTutorial = true;

		if (PCManagerFramework.p_pDataGame.bTutorial == false)
		{
			DoShowHide_Frame(EUIFrame.PCUIOutFrame_MainTop, true);
			DoShowHide_Frame(EUIFrame.PCUIOutFrame_MainMenu, true);
		}
		else
		{
			PCUISharedFrame_Dialogue pFrameShared_Dialogue = PCManagerUIShared.instance.GetUIFrame<PCUISharedFrame_Dialogue>();
			pFrameShared_Dialogue.DoInitUI(ECharacterName.guide, EDialogue.Tutorial_Main, PCManagerUIShared.instance);
			pFrameShared_Dialogue.DoSetIndexAndShowDialogue(0);
		}
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
	   찾기, 계산등 단순 로직(Simpe logic)        */
}