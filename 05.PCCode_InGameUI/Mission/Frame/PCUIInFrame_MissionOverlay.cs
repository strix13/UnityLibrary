using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Edit Log    : 
   ============================================ */
   
public class PCUIInFrame_MissionOverlay : CUIFrameBase, IButton_OnClickListener<PCUIInFrame_MissionOverlay.EUIButton>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EUIButton
	{
		Button_Pause,
	}

	public enum EUISprite
	{
		Sprite_OnStart,
		Sprite_OnHit,

		Sprite_HP3,
		Sprite_HP2,
		Sprite_HP1,
	}

	public enum EUILabel
	{
		// Gain Info
		Label_TicketNum,
		Label_GoldNum,

		// Score Info
		Label_ScoreNum,
		Label_RunaNum,

		// TEST
		Label_RouletteNum,

		Label_FeverThumb,
	}

	private enum EProgressBar
	{
		ProgressBar_Fever,
		ProgressBar_Fuel
	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private CUIWidgetProgressExtend _pProgressBar_Fever;
	private CUIWidgetProgressExtend _pProgressBar_Fuel;

	private CNGUILabelIndicator _pLabelIndicator_Gold;
	private CNGUILabelIndicator _pLabelIndicator_Fever;
	private CNGUILabelIndicator _pLabelIndicator_Score;
	private CNGUILabelIndicator _pLabelIndicator_RouletteTicket;
	private CNGUILabelIndicator _pLabelIndicator_Fuel;

	private float _fMaxFever;
	private int _iMaxHealth;

	private float _fCurFever;
	private float _fCurHealth;
	private float _fLastFever;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoInitUI(int iMaxHealth)
	{
		// TEST CODE
		_fMaxFever = iMaxHealth;
		_fCurFever = iMaxHealth;
		//

		_iMaxHealth = iMaxHealth;
		_fCurHealth = iMaxHealth;
	}

	public void DoShowSprite_AutoDisable( EUISprite eUISprite, float fAutoDisableSec)
	{
		GameObject pObjectUISprite = GetUISprite( eUISprite ).gameObject;
		pObjectUISprite.SetActive( true );

		StartCoroutine( CoDelayAutoDisable( pObjectUISprite, fAutoDisableSec ));
	}

	public void DoDelayDisable( EUISprite eUISprite, float fAutoDisableSec )
	{
		GameObject pObjectUISprite = GetUISprite( eUISprite ).gameObject;

		StartCoroutine( CoDelayAutoDisable( pObjectUISprite, fAutoDisableSec ) );
	}

	public void DoSetTotal_Score( int iScoreTotal, int iScoreAdd )
	{
		DoEditLabel( EUILabel.Label_ScoreNum, iScoreTotal.ToString().CommaString() );
		//_pLabelIndicator_Score.DoStartTween_Indicator( iScoreAdd.ToString().CommaString(), "+{0}" );
	}

	public void DoSetTotal_Fever(float fFeverTotal, float fFeverAdd)
	{
		_pProgressBar_Fever.DoStartTween_Percent(_fLastFever, fFeverTotal, _fMaxFever, 2, "%");
		_pLabelIndicator_Fever.DoStartTween_Indicator( fFeverAdd.ToString(), "+{0}%" );
		_fLastFever = fFeverTotal;
	}

	public void DoSetTotal_Runa( int iRunaTotal )
	{
		DoEditLabel( EUILabel.Label_RunaNum, iRunaTotal.ToString().CommaString() );
	}

	public void DoSetTotal_Ticket(int iTicketTotal)
	{
		DoEditLabel(EUILabel.Label_TicketNum, iTicketTotal.ToString().CommaString());
	}

	public void DoSetTotal_Gold(int iGoldTotal, int iGoldAdd = 0)
	{
		DoEditLabel(EUILabel.Label_GoldNum, iGoldTotal.ToString().CommaString());

		if (iGoldAdd > 0)
			_pLabelIndicator_Gold.DoStartTween_Indicator(iGoldAdd.ToString().CommaString(), "+{0}");
	}

	public void EventOnFever()
	{
		print("피버 발동 UI");

		_pProgressBar_Fever.DoStartTween_Percent(_fLastFever, 0, 100f, 2, "%");
		_fLastFever = 0f;
	}

	public  void DoSetTotal_Roulette(int iRouletteTotal, int iAddAmount )
	{
		DoEditLabel(EUILabel.Label_RouletteNum, iRouletteTotal.ToString().CommaString());
		if (iAddAmount > 0)
			_pLabelIndicator_RouletteTicket.DoStartTween_Indicator( Color.green, iAddAmount.ToString() );
		else if(iAddAmount < 0)
			_pLabelIndicator_RouletteTicket.DoStartTween_Indicator( Color.red, iAddAmount.ToString() );
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	public void IOnClick_Buttons(EUIButton eButton)
	{
		switch (eButton)
		{
			case EUIButton.Button_Pause:
				PCManagerUIShared.instance.DoShowHide_Popup(PCManagerUIShared.EUIPopup.PCUISharedPopup_GamePause, true);
				break;
		}
	}

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	private void EventOnPlayerTakeDamage(float fDecreaseHP)
	{
		float fLastHealth = _fCurHealth;

		_fCurHealth -= fDecreaseHP;

		_pProgressBar_Fuel.DoStartTween(fLastHealth, _fCurHealth, _iMaxHealth);
		if(fDecreaseHP >= 1f)
			_pLabelIndicator_Fuel.DoStartTween_Indicator( fDecreaseHP.ToString() );
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pProgressBar_Fever = GetGameObject(EProgressBar.ProgressBar_Fever).transform.GetComponent<CUIWidgetProgressExtend>();
		_pProgressBar_Fuel = GetGameObject(EProgressBar.ProgressBar_Fuel).transform.GetComponent<CUIWidgetProgressExtend>();

		_pLabelIndicator_Gold = GetUILabel(EUILabel.Label_GoldNum).GetComponent<CNGUILabelIndicator>();
		_pLabelIndicator_Score = GetUILabel( EUILabel.Label_ScoreNum ).GetComponent<CNGUILabelIndicator>();
		_pLabelIndicator_Fever = GetUILabel( EUILabel.Label_FeverThumb ).GetComponent<CNGUILabelIndicator>();
		_pLabelIndicator_RouletteTicket = GetUILabel( EUILabel.Label_RouletteNum ).GetComponent<CNGUILabelIndicator>();

		_pLabelIndicator_Fuel = GetGameObject( "ThumbFuel").GetComponent<CNGUILabelIndicator>();
	}

	protected override void OnShow(int iSortOrder)
	{
		base.OnShow(iSortOrder);

		// Gain Info
		DoEditLabel(EUILabel.Label_TicketNum, "0");
		DoEditLabel(EUILabel.Label_GoldNum, "0");

		// Score Info
		DoEditLabel(EUILabel.Label_ScoreNum, "0");
		DoEditLabel(EUILabel.Label_RunaNum, "0");

		DoInitUI(100);

		_pProgressBar_Fever.DoStartTween_Percent(0, 0, _fMaxFever, 2, "%");
		_pProgressBar_Fuel.DoStartTween_Percent(0, _fCurHealth, _iMaxHealth, 0);

		PCManagerInMission.instance.p_EVENT_OnPlayerTakeDamage += EventOnPlayerTakeDamage;
	}

	protected override void OnHide()
	{
		base.OnHide();

		PCManagerInMission.instance.p_EVENT_OnPlayerTakeDamage -= EventOnPlayerTakeDamage;
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

	private IEnumerator CoDelayAutoDisable( GameObject pObject, float fDelaySec )
	{
		yield return new WaitForSeconds( fDelaySec );

		pObject.SetActive( false );
	}
}
