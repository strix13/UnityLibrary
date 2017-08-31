using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* ============================================ 
   Editor      : KJH
   Description : 
   Edit Log    : 
   ============================================ */

public class PCUIInFrame_MissionFuel : CUIFrameBase, IButton_OnClickListener<PCUIInFrame_MissionFuel.EUIButton>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EUIButton
	{
		Button_fuel_4,
		Button_fuel_5,
		Button_fuel_6,
		Button_fuel_7,
		Button_fuel_8,

		Button_fuel_Start,
		Stop,

		Button_RealExit,
		Button_RealNo,
		Button_RealYes,
		
		Exit,
		Exit2,
		Main,
		Replay,
	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void IOnClick_Buttons( EUIButton eButtonName )
	{
		Debug.Log( eButtonName );
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
