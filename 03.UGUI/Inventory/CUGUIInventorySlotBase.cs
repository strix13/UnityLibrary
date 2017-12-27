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

public class CUGUIInventorySlotBase : CObjectBase,
	IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerClickHandler
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	private enum EImage
	{
		Image_Item,
		Image_Selected,
		Image_Equip
	}

	#region Field

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	[SerializeField] private Sprite _pSprite_SlotEmpty;
	[SerializeField] private Sprite _pSprite_SlotNotEmpty;

	private IInventoryListener _IInventoryListener;

	private Image _pImageSlot;
	private Image _pImage_Item; public Image p_pImage { get { return _pImage_Item; } }
	private Image _pImage_Selected;
	private Image _pImage_Equip;

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

	public void DoEnableImage_Equip(bool bEnable)
	{
		EventEnableImage_Equip(bEnable);
	}

	public void EventSetData(CInventorySlotDataBase IData)
	{
		Sprite pSprite = IData.p_pSprite;

		EventSetImage(pSprite);
	}

	public void DoEnableImage(bool bEnable)
	{
		EventSetNotEmptySlot(bEnable);
	}

	public void DoEnableImageSelected(bool bEnable)
	{
		_pImage_Selected.enabled = bEnable;
	}

	public void DoSetEmptySlot()
	{
		EventSetEmptySlot();
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	public void OnPointerDown(PointerEventData pEventData)
	{
		_IInventoryListener.OnPickItem(pEventData.position, _iSlotID);
	}

	public void OnDrag(PointerEventData pEventData)
	{
		_IInventoryListener.OnDragItem(pEventData.position, _iSlotID);
	}

    public void OnPointerUp(PointerEventData pEventData)
	{
		_IInventoryListener.OnDropItem(pEventData.pointerEnter, _iSlotID, _iInvenHashCode);
	}

	public void OnPointerClick(PointerEventData pEventData)
	{
		_IInventoryListener.OnSelectSlot(this);
	}


	public void OnReceiveData(CInventorySlotDataBase pData)
	{
		_IInventoryListener.OnReceiveData(_iSlotID, pData);
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
		_pImageSlot.sprite = _pSprite_SlotNotEmpty;

		_pImage_Item.sprite = pSprite;
		_pImage_Item.SetNativeSize();

		EventSetNotEmptySlot(true);
	}

	private void EventSetNotEmptySlot(bool bEnable)
	{
		_pImage_Item.enabled = bEnable;
	}

	private void EventSetEmptySlot()
	{
		_pImageSlot.sprite = _pSprite_SlotEmpty;

		EventEnableImage_Equip(false);
		EventSetNotEmptySlot(false);
	}

	private void EventEnableImage_Equip(bool bEnable)
	{
		_pImage_Equip.enabled = bEnable;
	}

    /* protected - Override & Unity API         */

    protected override void OnAwake()
	{
		base.OnAwake();

		GetComponent(out _pImageSlot);

		_pImage_Item = GetGameObject(EImage.Image_Item).GetComponent<Image>();
		_pImage_Item.raycastTarget = false;
		_pImage_Item.enabled = false;

		_pImage_Equip = GetGameObject(EImage.Image_Equip).GetComponent<Image>();
		_pImage_Equip.raycastTarget = false;
		_pImage_Equip.enabled = false;

		GameObject pGoImage_Selected = GetGameObject(EImage.Image_Selected, false);
		if (pGoImage_Selected != null)
		{
			_pImage_Selected = pGoImage_Selected.GetComponent<Image>();
			_pImage_Selected.raycastTarget = false;
			_pImage_Selected.enabled = false;
		}

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
