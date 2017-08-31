using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Date        : 2017-08-17 오후 4:17:25
   Description : 
   Edit Log    : 
   ============================================ */

public class PCUIScrollViewItem_Bonus : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EUILabel
	{
		Label_BonusAdd,
		Label_BonusName
	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoInitUI(EMissionBonus eBonus, int iCount)
	{
		string strLocValue = CManagerUILocalize.DoGetCurrentLocalizeValue(string.Format("{0}_{1}",
			EUILabel.Label_BonusName.ToString_GarbageSafe(), eBonus.ToString_GarbageSafe()));


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
