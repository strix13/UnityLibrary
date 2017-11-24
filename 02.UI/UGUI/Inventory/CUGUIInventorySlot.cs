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

public class CUGUIInventorySlot : CObjectBase,
	IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	#region Field

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private IInventoryListener _IInventoryListener; 

	private Image _pImageSlot;
	private Image _pImageItem;

	private Sprite _pSpriteTemp;

	private int _iSlotID; public int p_iSlotID { get { return _iSlotID; } }
	private int _iInvenHashCode; public int p_iInvenHashCode { get { return _iInvenHashCode; } }

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoSetInventoryListener(IInventoryListener IListener)
	{
		_IInventoryListener = IListener;
		_iInvenHashCode = IListener.GetHashCode();

		EventRegisterSlot();
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	public void EventSetData(CInventoryDataBase IData)
	{
		Sprite pSprite = IData.p_pSprite;

		EventSetImage(pSprite);
	}

	public void EventRemoveData()
	{
		EventRemoveImage();
	}

	public void OnPointerDown(PointerEventData pEventData)
	{
		_IInventoryListener.OnPickItem(pEventData.position, _iSlotID);
	}

	public void OnDrag(PointerEventData pEventData)
	{
		_IInventoryListener.OnDragItem(pEventData.position);
	}

    public void OnPointerUp(PointerEventData pEventData)
	{
		_IInventoryListener.OnDropItem(pEventData.pointerEnter, _iSlotID, _iInvenHashCode);
	}

	public void OnReceiveData(CInventoryDataBase IData)
	{
		_IInventoryListener.OnReceiveData(_iSlotID, IData);
	}

	#endregion Public

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	private void EventRegisterSlot()
	{
		_IInventoryListener.OnRegisterSlot(this);
	}

	private void EventSetImage(Sprite pSprite)
	{
		_pSpriteTemp = pSprite;
		_pImageItem.sprite = pSprite;
		_pImageItem.enabled = true;
	}

	private void EventRemoveImage()
	{
		_pImageItem.enabled = false;
	}

    /* protected - Override & Unity API         */

    protected override void OnAwake()
	{
		base.OnAwake();

		GetComponent(out _pImageSlot);

		_pImageItem = GetComponentInChildrenOnly<Image>(this);
		_pImageItem.raycastTarget = false;
		_pImageItem.enabled = false;

		_iSlotID = _pTransformCached.GetSiblingIndex();
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
