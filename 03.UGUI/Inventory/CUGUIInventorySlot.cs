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

public abstract class CUGUIInventorySlot : CUGUIObjectBase, IInventorySlot, IPointerClickHandler
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	#region Field

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private IInventory _IInventory;

	private Image _pImage_Icon;
	private GameObject _pGoImage_Icon;

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoInit(IInventory IInventory)
	{
		EventOnAwake();

		_IInventory = IInventory;
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	public void OnSetSprite(string strSpriteName)
	{
		EventSetImage(GetSprite(strSpriteName));
	}

	public void OnEnableSlot(bool bEnable)
	{
		EventEnableImage(bEnable);
	}

	public void OnPointerClick(PointerEventData pEventData)
	{
		_IInventory.OnClickSlot(this);
	}

	#endregion Public

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	protected abstract string GetImageName();

	protected abstract Sprite GetSprite(string strSpriteName);

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	private void EventSetImage(Sprite pSprite)
	{
		_pImage_Icon.sprite = pSprite;
		_pImage_Icon.SetNativeSize();
	}

	private void EventEnableImage(bool bEnable)
	{
		_pGoImage_Icon.SetActive(bEnable);
	}
	
	protected override void OnAwake()
	{
		base.OnAwake();

		string strImageName = GetImageName();

		_pImage_Icon = GetImage(strImageName);
		_pImage_Icon.raycastTarget = false;

		_pGoImage_Icon = _pImage_Icon.gameObject;
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
