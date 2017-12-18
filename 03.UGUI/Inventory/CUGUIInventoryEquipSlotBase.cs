#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : KJH
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public interface IInventoryEquipSlot<CLASS_DATA>
{
	bool CheckSlotType(CLASS_DATA pData);
}

public abstract class CUGUIInventoryEquipSlotBase<ENUM_TYPE, CLASS_DATA> : CUGUIInventorySlotBase, IInventoryEquipSlot<CLASS_DATA>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	#region Field

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	[SerializeField]
	private ENUM_TYPE _eSlotType; public ENUM_TYPE p_eSlotType { get { return _eSlotType; } }

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	public bool CheckSlotType(CLASS_DATA pData)
	{
		return OnCheckSlotType(pData);
	}

	#endregion Public

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	protected abstract bool OnCheckSlotType(CLASS_DATA pData);

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
