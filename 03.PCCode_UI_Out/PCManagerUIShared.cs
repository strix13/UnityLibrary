using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Description : 
   Edit Log    : 
   ============================================ */

public class PCManagerUIShared : CManagerUIBase<PCManagerUIShared, PCManagerUIShared.EUIFrame, PCManagerUIShared.EUIPopup>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EUIFrame
	{
		PCUISharedFrame_Dialogue
	}

	public enum EUIPopup
	{
		PCUISharedPopup_EffectGuide,
		PCUISharedPopup_GamePause,
		PCUISharedPopup_GamePauseX2,
	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private PCUISharedFrame_Dialogue _pUISharedFrame_Dialogue;
	private PCUISharedPopup_EffectGuide _pUISharedPopup_EffectGuide;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoShowHidePopup_EffectGuide(bool bShow, Transform pParent = null, float fScale = 0f)
	{
		if (bShow)
			_pUISharedPopup_EffectGuide.DoInitUI(pParent, fScale);

		_pUISharedPopup_EffectGuide.DoShowHide(bShow);
	}

	public void DoDisableTutorial()
	{
		PCManagerFramework.DoNetworkDB_UpdateAdd<SInfoUser>( "iLogInCount", 1, null );
		PCManagerFramework.p_pDataGame.bTutorial = false;
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pUISharedPopup_EffectGuide = GetUIPopup<PCUISharedPopup_EffectGuide>();
		_pUISharedFrame_Dialogue = GetUIFrame<PCUISharedFrame_Dialogue>();
	}

	protected override void OnDefaultFrameShow()
	{

	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

	/* Event */

	private void OnTutorial_GotoSelectCharacter()
	{
		_pUISharedFrame_Dialogue.DoHide();

		DoShowHide_FrameDelayed(EUIFrame.PCUISharedFrame_Dialogue, true, 1.5f);

		PCManagerUIOutGame.instance.DoShowFrame_FadeIn(PCManagerUIOutGame.EUIFrame.PCUIOutFrame_SelectCharacter);
		EventDelegate.Add(PCManagerUIOutGame.instance.GetUIFrame<PCUIOutFrame_SelectCharacter>().p_EVENT_OnSelectCharacter,
			_pUISharedFrame_Dialogue.DoShow, true);
	}

	private void OnTutorial_SelectCharacter()
	{
		_pUISharedFrame_Dialogue.DoHide();
	}

	private void OnTutorial_GotoLobby()
	{
		_pUISharedFrame_Dialogue.DoHide();

		PCManagerUIOutGame.instance.DoChangeFrame_FadeInout(PCManagerUIOutGame.EUIFrame.PCUIOutFrame_SelectCharacter, PCManagerUIOutGame.EUIFrame.PCUIOutFrame_MainMenu);
		PCManagerUIOutGame.instance.DoShowHide_FrameDelayed(PCManagerUIOutGame.EUIFrame.PCUIOutFrame_MainTop, true, 0.5f);

		DoShowHide_FrameDelayed(EUIFrame.PCUISharedFrame_Dialogue, true, 1.5f);
	}

	private void OnTutorial_HideUI()
	{
		_pUISharedFrame_Dialogue.DoHide();

		PCUIOutFrame_MainMenu pUIOutFrame_MainMenu = PCManagerUIOutGame.instance.GetUIFrame<PCUIOutFrame_MainMenu>();
		pUIOutFrame_MainMenu.DoEnableButton_OnlyOne(PCUIOutFrame_MainMenu.EUIButton.Button_Hide);

		UIButton pButton = pUIOutFrame_MainMenu.GetUIButton(PCUIOutFrame_MainMenu.EUIButton.Button_Hide);
		_pUISharedPopup_EffectGuide.DoInitUI(pButton.transform);

		EventDelegate.Add(pButton.onClick, OnTutorial_HideUI2);
	}

	private void OnTutorial_HideUI2()
	{
		_pUISharedPopup_EffectGuide.DoHide();
		_pUISharedFrame_Dialogue.DoShow();
	}

	private void OnTutorial_TouchTap()
	{
		_pUISharedFrame_Dialogue.DoHide();

		PCUIOutFrame_MainMenu pUIOutFrame_MainMenu = PCManagerUIOutGame.instance.GetUIFrame<PCUIOutFrame_MainMenu>();
		pUIOutFrame_MainMenu.DoEnableButton_OnlyOne(PCUIOutFrame_MainMenu.EUIButton.Button_Tap);

		UIButton pButton = pUIOutFrame_MainMenu.GetUIButton(PCUIOutFrame_MainMenu.EUIButton.Button_Tap);
		_pUISharedPopup_EffectGuide.DoInitUI(pButton.transform);

		EventDelegate.Add(pButton.onClick, OnTutorial_TouchTap2);
	}

	private void OnTutorial_TouchTap2()
	{
		_pUISharedFrame_Dialogue.DoShow();
		PCManagerUIOutGame.instance.GetUIFrame<PCUIOutFrame_MainMenu>().HideUI();
	}

	private void OnTutorial_HideTap()
	{
		PCManagerUIOutGame.instance.GetUIFrame<PCUIOutFrame_MainMenu>().DoShowHideUI_TabBar(false);
	}

	private void OnTutorial_GotoMission()
	{
		_pUISharedFrame_Dialogue.DoHide();

		PCUIOutFrame_MainMenu pUIOutFrame_MainMenu = PCManagerUIOutGame.instance.GetUIFrame<PCUIOutFrame_MainMenu>();
		pUIOutFrame_MainMenu.DoEnableButton_OnlyOne(PCUIOutFrame_MainMenu.EUIButton.Button_Mission);

		UIButton pButton = pUIOutFrame_MainMenu.GetUIButton(PCUIOutFrame_MainMenu.EUIButton.Button_Mission);
		_pUISharedPopup_EffectGuide.DoInitUI(pButton.transform);

		PCManagerFramework.p_pDataGame.bTutorial = false;
	}

	//// ========================================================================== //

	///* Skip */

	private void OnSkipTutorial_GotoSelectCharacter()
	{
		OnTutorial_GotoSelectCharacter();
	}

	private void OnSkipTutorial_GotoLobby()
	{
		OnTutorial_GotoLobby();
	}

	private void OnSkipTutorial_GotoMission()
	{
		_pUISharedFrame_Dialogue.DoHide();

		PCUIOutFrame_MainMenu pUIOutFrame_MainMenu = PCManagerUIOutGame.instance.GetUIFrame<PCUIOutFrame_MainMenu>();
		pUIOutFrame_MainMenu.Mission();

		PCManagerFramework.p_pDataGame.bTutorial = false;
	}

	// ========================================================================== //

}
