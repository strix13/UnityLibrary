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
using System.Collections;
using System.Collections.Generic;

public abstract class CInventoryTabBase<ENUM_TAB, CLASS_DATA, CLASS_SLOT> : CInventoryBase<CLASS_DATA, CLASS_SLOT>
	where ENUM_TAB : System.IFormattable, System.IConvertible, System.IComparable
	where CLASS_DATA : IInventoryInfoData
	where CLASS_SLOT : IInventorySlot
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private ENUM_TAB _eCurrentTab; public ENUM_TAB p_eCurrentTab { get { return _eCurrentTab; } }

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoSetTab(ENUM_TAB eTab)
	{
		_eCurrentTab = eTab;

		DoInit_InventoryData();
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	protected abstract void OnSetInventoryTab(ENUM_TAB eTab, CLASS_DATA sInfoData, CLASS_SLOT pSlot);

	protected abstract List<CLASS_DATA> GetInventoryDataTab();

	protected abstract ENUM_TAB GetDefaultTab();

	protected abstract bool GetEquals_InventoryDataTab(ENUM_TAB eTab, CLASS_DATA sInfoData);


	protected virtual void OnSetInventoryTab(ENUM_TAB eTab) { }

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override List<CLASS_DATA> GetInventoryData()
	{
		List<CLASS_DATA> listInventoryDataTab = new List<CLASS_DATA>();
		List<CLASS_DATA> listInventoryData = GetInventoryDataTab();

		int iCount = listInventoryData.Count;
		for (int i = 0; i < iCount; i++)
		{
			CLASS_DATA sInfoData = listInventoryData[i];

			if (GetEquals_InventoryDataTab(_eCurrentTab, sInfoData))
				listInventoryDataTab.Add(sInfoData);
		}

		OnSetInventoryTab(_eCurrentTab);

		return listInventoryDataTab;
	}

	protected override void OnSetInventoryData(CLASS_DATA sInfoData, CLASS_SLOT pSlot)
	{
		base.OnSetInventoryData(sInfoData, pSlot);

		OnSetInventoryTab(_eCurrentTab, sInfoData, pSlot);
	}

	protected override void OnAwake()
	{
		base.OnAwake();

		_eCurrentTab = GetDefaultTab();
	}

	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
