using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 

   여기선 TiemScale이 0이기때문에 
   코루틴에서 WaitForSecondsRealtime을 써야한다.

   Version	   :
   ============================================ */

public class PCUIInPopup_MissionRoulette : CUIPopupBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EMissionRouletteAnimationName
	{
		up,
		down,

		pinup,
		pindown,

		backgroundup,
		backgrounddown,

		slotup1frame,
		slotdown,

		bodyrun,
		slotrun,
		pinrun,

		fuel1,
		fuel2,
		fuel3,

		blackhole1up,
		blackhole2up,
		blackhole3up,

		fever1up,
		fever2up,
		fever3up,

		fuel1up,
		fuel2up,
		fuel3up,

		gold1up,
		gold2up,
		gold3up,

		power1up,
		power2up,
		power3up,

		universe1up,
		universe2up,
		universe3up,

		ticket1,
		ticket1up,
		ticket2up,
		ticket3up,

		pinpick,

	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private PCRoulette _pRoulette;
	private TweenPosition _pTweenPosRoulette;

	private System.Action _OnFinishAnimation_Show;
	private System.Action _OnFinishAnimation_Roullette;

	private bool _bIsRouletteRunning;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/
	 
	public void DoRunRoulette( System.Action OnFinishRoullette )
	{
		_OnFinishAnimation_Roullette = OnFinishRoullette;

		if (_bIsRouletteRunning == false)
			ProcStartRouletteDummy();
		else
			_OnFinishAnimation_Show = ProcStartRouletteDummy;
	}

	public void DoHideRoulette()
	{
		if (_bIsRouletteRunning == false)
			ProcHideRoulette();
		else
			_OnFinishAnimation_Show = ProcHideRoulette;
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

		GetComponentInChildren( out _pRoulette );
		GetComponentInChildren( out _pTweenPosRoulette );
	}

	protected override void OnShow( int iSortOrder )
	{
		base.OnShow( iSortOrder );

		PCManagerFramework.DoSetTimeScale( 0f );
		StartCoroutine( CoOnRoulette_Show() );
	}

	protected override void OnHide()
	{
		base.OnHide();

		PCManagerFramework.DoSetTimeScale( 1f );
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private void ProcStartRouletteDummy()
	{
		StartCoroutine( CoRoulletteDummy() );
	}

	private void ProcHideRoulette()
	{
		StartCoroutine( CoOnRoulette_Hide() );
	}

	private IEnumerator CoOnRoulette_Show()
	{
		_bIsRouletteRunning = true;
		//_pTweenPosRoulette.ResetToBeginning();
		//_pTweenPosRoulette.PlayForward();

		yield return new WaitForSecondsRealtime( 0.1f );

		_pRoulette.DoPlayAnimation( PCRoulette.EComponentName.RouletteBoddy, EMissionRouletteAnimationName.down );
		_pRoulette.DoPlayAnimation( PCRoulette.EComponentName.Background, EMissionRouletteAnimationName.backgrounddown );
		_pRoulette.DoPlayAnimation( PCRoulette.EComponentName.Pin, EMissionRouletteAnimationName.pindown );

		_pRoulette.DoPlayAnimation( PCRoulette.EComponentName.Reel_1, EMissionRouletteAnimationName.slotdown );
		_pRoulette.DoPlayAnimation( PCRoulette.EComponentName.Reel_2, EMissionRouletteAnimationName.slotdown );
		_pRoulette.DoPlayAnimation( PCRoulette.EComponentName.Reel_3, EMissionRouletteAnimationName.slotdown );

		yield return new WaitForSecondsRealtime( 2f );

		_bIsRouletteRunning = false;

		if(_OnFinishAnimation_Show != null)
		{
			System.Action OnFinishAnimation_Temp = _OnFinishAnimation_Show;
			_OnFinishAnimation_Show = null;
			OnFinishAnimation_Temp();
		}
	}

	private IEnumerator CoOnRoulette_Hide()
	{
		_pRoulette.DoPlayAnimation( PCRoulette.EComponentName.RouletteBoddy, EMissionRouletteAnimationName.up );
		_pRoulette.DoPlayAnimation( PCRoulette.EComponentName.Background, EMissionRouletteAnimationName.backgroundup );
		_pRoulette.DoPlayAnimation( PCRoulette.EComponentName.Pin, EMissionRouletteAnimationName.pinup );

		_pRoulette.DoPlayAnimation( PCRoulette.EComponentName.Reel_1, EMissionRouletteAnimationName.slotup1frame );
		_pRoulette.DoPlayAnimation( PCRoulette.EComponentName.Reel_2, EMissionRouletteAnimationName.slotup1frame );
		_pRoulette.DoPlayAnimation( PCRoulette.EComponentName.Reel_3, EMissionRouletteAnimationName.slotup1frame );

		yield return new WaitForSecondsRealtime( 2f );

		DoHide();
	}

	private IEnumerator CoRoulletteDummy()
	{
		_bIsRouletteRunning = true;

		_pRoulette.DoPlayAnimation_Loop( PCRoulette.EComponentName.RouletteBoddy, EMissionRouletteAnimationName.bodyrun );
		_pRoulette.DoPlayAnimation_Loop( PCRoulette.EComponentName.Pin, EMissionRouletteAnimationName.pinrun );

		_pRoulette.DoPlayAnimation_Loop( PCRoulette.EComponentName.Reel_1, EMissionRouletteAnimationName.slotrun );
		_pRoulette.DoPlayAnimation_Loop( PCRoulette.EComponentName.Reel_2, EMissionRouletteAnimationName.slotrun );
		_pRoulette.DoPlayAnimation_Loop( PCRoulette.EComponentName.Reel_3, EMissionRouletteAnimationName.slotrun );

		yield return new WaitForSecondsRealtime( 2f );

		// 일단 더미로..
		// 차후 애니메이션이 아니라 프로그래밍에서 제어

		_pRoulette.DoPlayAnimation( PCRoulette.EComponentName.Reel_1, EMissionRouletteAnimationName.ticket1 );
		yield return new WaitForSecondsRealtime( 0.2f );
		_pRoulette.DoPlayAnimation( PCRoulette.EComponentName.Reel_2, EMissionRouletteAnimationName.ticket1 );
		yield return new WaitForSecondsRealtime( 0.2f );
		_pRoulette.DoPlayAnimation( PCRoulette.EComponentName.Reel_3, EMissionRouletteAnimationName.ticket1 );

		_pRoulette.DoPlayAnimation( PCRoulette.EComponentName.RouletteBoddy, "universe3" );
		_pRoulette.DoPlayAnimation( PCRoulette.EComponentName.Pin, EMissionRouletteAnimationName.pinpick );

		yield return new WaitForSecondsRealtime( 2f );

		_bIsRouletteRunning = false;

		if(_OnFinishAnimation_Roullette != null)
		{
			System.Action OnFinishAnimation_Temp = _OnFinishAnimation_Roullette;
			_OnFinishAnimation_Roullette = null;
			OnFinishAnimation_Temp();
		}
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
