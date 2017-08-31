using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using System;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class PCUIOutFrame_MainTop : CUIFrameBase, IButton_OnClickListener<PCUIOutFrame_MainTop.EUIButton>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EUIButton
	{
		Button_CashShop,
		Button_KeyShop,
		Button_TicketShop,
		Button_GoldShop,
		Button_Mail,
		Button_Option
	}

	public enum EUILabel
	{
		Label_GoldNum,
		Label_CashNum,
		Label_TicketNum,
		Label_Nick,
	}

	public enum EUISprite
	{
		Sprite_CharacterFace,
	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	[SerializeField]
	private GameObject _pObjectOnMainMenu = null;

	private UILabel _UILabel_Gold;
	private UILabel _UILabel_Ticket;
	private UILabel _UILabel_CharacterName;

	private UISprite _UISprite_CharacterFace;
	
	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoSetIsShowMainMenu(bool bIsShowMainMenu)
	{
		_pObjectOnMainMenu.SetActive( bIsShowMainMenu );

		if (bIsShowMainMenu)
		{
			_UISprite_CharacterFace.spriteName = PCManagerFramework.p_pInfoUser.eCharacterCurrent.ToString();

			string strCharacterName = PCManagerFramework.p_pInfoUser.eCharacterCurrent.ToString_GarbageSafe();
			string strLocValue = CManagerUILocalize.DoGetCurrentLocalizeValue(strCharacterName);
			_UILabel_CharacterName.text = strLocValue;
		}
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

		_UILabel_Gold = GetUILabel( EUILabel.Label_GoldNum );
		_UILabel_Ticket = GetUILabel( EUILabel.Label_TicketNum );
		_UILabel_CharacterName = GetUILabel( EUILabel.Label_Nick );

		_UISprite_CharacterFace = GetUISprite( EUISprite.Sprite_CharacterFace );
	}

	protected override void OnShow( int iSortOrder )
	{
		base.OnShow( iSortOrder );

		_UILabel_Gold.text = PCManagerFramework.p_pInfoUser.iGold.ToString();
		_UILabel_Ticket.text = PCManagerFramework.p_pInfoUser.iTicket.ToString();
	}

	public void IOnClick_Buttons( EUIButton eButtonName )
	{
		switch (eButtonName)
		{
			case EUIButton.Button_Option:
				break;
		}
		
		Debug.Log( eButtonName );
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
