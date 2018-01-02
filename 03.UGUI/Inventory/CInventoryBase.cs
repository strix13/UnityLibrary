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
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

//public class CInventoryData_Test
//{
//	private IInventoryListener _pInventoryOwner; public IInventoryListener p_pInventoryOwner { get { return _pInventoryOwner; } }

//	private Sprite _pSprite; public Sprite p_pSprite { get { return _pSprite; } }

//	private int _iSlotID; public int p_iSlotID { get { return _iSlotID; } }
//	private int _iRealID; public int p_iRealID { get { return _iRealID; } }

//	public void DoInit(Sprite pSprite, IInventoryListener pInventoryListener)
//	{
//		DoInitSprite(pSprite);
//		DoInitOwnerListener(pInventoryListener);
//	}

//	public void DoInitSprite(Sprite pSprite) { this._pSprite = pSprite; }

//	public void DoSetSlotID(int iSlotID) { this._iSlotID = iSlotID; }

//	public void DoSetRealID(int iRealID) { this._iRealID = iRealID; }

//	public void DoInitOwnerListener(IInventoryListener pInventoryOwner) { _pInventoryOwner = pInventoryOwner; }
//}

//public interface IInventoryListener
//{
//	void OnRegisterSlot(CUGUIInventorySlot pSlot);

//	void OnPickItem(Vector3 v3CursorPos, int iPickSlotID);
//	void OnDragItem(Vector3 v3CursorPos, int iPickSlotID);
//	void OnDropItem(GameObject pGameObject, int iPickSlotID, int iHashCode);
//	void OnSelectSlot(CUGUIInventorySlot pSlot);

//	void OnReceiveData(int iReceiveSlotID, CInventoryData_Test IReceiveData);
//}

public interface IInventory
{
	void OnClickSlot(IInventorySlot IInventorySlot);
}

public interface IInventorySlot
{
	void DoInit(IInventory IInventory);

	void OnSetSprite(string strSpriteName);

	void OnEnableSlot(bool bEnable);
}

public interface IInventoryInfoData { }

public abstract class CInventoryBase<CLASS_DATA, CLASS_SLOT> : CObjectBase, IInventory
	where CLASS_DATA : IInventoryInfoData
	where CLASS_SLOT : IInventorySlot
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EInventoryType
	{
		Scroll,
		Page,
		Page_Auto
	}

	#region Field

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private Dictionary<int, CLASS_DATA> _mapInventoryData = new Dictionary<int, CLASS_DATA>();
	private Dictionary<int, IInventorySlot> _mapInventorySlot = new Dictionary<int, IInventorySlot>();

	private int _iMaxPage;
	private int _iCurPage;

	private int _iMaxSlotCount;
	private int _iTotalDataCount;

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoInit_InventoryData()
	{
		EventInit_InventoryType();
	}

	public void DoPrevPage()
	{
		EventPrevPage();
	}

	public void DoNextPage()
	{
		EventNextPage();
	}

	public void DoAddMaxPage(int iAddMaxPage)
	{
		EventAddMaxPage(iAddMaxPage);
	}

	public void DoSetPage(int iPage)
	{
		EventSetPage(iPage);
	}

	//public CLASS_DATA GetInventoryData(int iRealID)
	//{
	//	return _mapInventoryData[iRealID];
	//}

	//public CUGUIInventorySlot GetInventorySlot(int iSlotID)
	//{
	//	return _mapInventorySlot[iSlotID];
	//}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	//public void OnRegisterSlot(CUGUIInventorySlot pSlot)
	//{
	//	int iSlotID = pSlot.p_iSlotID;
	//	if (_mapInventorySlot.ContainsKey(iSlotID))
	//	{
	//		Debug.Log( "이미 슬롯 키 값이 들어가있습니다." );
	//		return;
	//	}

	//	_mapInventorySlot.Add(iSlotID, pSlot);
	//}

	//public void OnPickItem(Vector3 v3CursorPos, int iPickSlotID)
	//{
	//	if (_bEnableDragItem == false) return;

	//	_bIsSlotContainItem = _mapInventoryData.ContainsKey(iPickSlotID);
	//}

	//public void OnDragItem(Vector3 v3CursorPos, int iPickSlotID)
	//{
	//	if (_bEnableDragItem == false) return;

	//	if (_bIsSlotContainItem == false) return;

	//	CUGUIInventorySlot pSlot = _mapInventorySlot[iPickSlotID];

	//	_pInventoryCursor.DoSetImage(pSlot.p_pImage.sprite);
	//	_pInventoryCursor.DoUpdatePosition(v3CursorPos);
	//}

	//public void OnDropItem(GameObject pGameObject, int iOwnerSlotID, int iOwnerSlotHashCode)
	//{
	//	if (_bEnableDragItem == false) return;

	//	if (_bIsSlotContainItem == false) return;

	//	_pInventoryCursor.SetActive(false);

	//	if (pGameObject == null) return;

	//	// 일단 슬롯이 있는지 체크
	//	CUGUIInventorySlot pTargetSlot = pGameObject.GetComponent<CUGUIInventorySlot>();
	//	if (pTargetSlot == null) return;

	//	CLASS_DATA pOwnerData_Backup = _mapInventoryData[iOwnerSlotID];

	//	IInventoryEquipSlot<CLASS_DATA> ITargetEquipSlot = pTargetSlot.GetComponent<IInventoryEquipSlot<CLASS_DATA>>();
	//	if (ITargetEquipSlot != null)
	//	{
	//		CUGUIInventorySlot pOwnerSlot = GetInventorySlot(iOwnerSlotID);
	//		if (ITargetEquipSlot.CheckSlotType(pOwnerData_Backup) == false) return;
	//	}

	//	if (iOwnerSlotHashCode == pTargetSlot.p_iInvenHashCode)
	//	{
	//		// 같은 슬롯일 경우 데이터만 스왑해준다.
	//		if (_bEnableSwapItemInSameInventory)
	//			EventSwapData(iOwnerSlotID, pTargetSlot.p_iSlotID);
	//	}
	//	else
	//	{
	//		// 다른 슬롯일 경우 데이터 발신
	//		if (_bRemoveItem)
	//			EventRemoveData(iOwnerSlotID);

	//		pTargetSlot.OnReceiveData(pOwnerData_Backup);
	//	}
	//}

	//public void OnSelectSlot(CUGUIInventorySlot pSlot)
	//{
	//	if (_bEnableSelectSlot == false) return;

	//	EventSelectSlot(pSlot);
	//}

	//protected void EventSelectSlot(CUGUIInventorySlot pSlot)
	//{
	//	int iSlotID = pSlot.p_iSlotID;
	//	if (_mapInventoryData.ContainsKey(iSlotID) == false) return;

	//	if (_pSelectedSlot != null)
	//	{
	//		_pSelectedSlot.DoEnableImageSelected(false);

	//		if (_pSelectedSlot.p_iSlotID == pSlot.p_iSlotID)
	//		{
	//			EventUnSelectSlot();
	//			OnUnSelectedSlot();
	//			return;
	//		}
	//	}

	//	pSlot.DoEnableImageSelected(true);
	//	OnSelectedSlot(pSlot, _mapInventoryData[pSlot.p_iSlotID]);

	//	_pSelectedSlot = pSlot;
	//}

	//protected void EventUnSelectSlot()
	//{
	//	if (_pSelectedSlot != null)
	//	{
	//		_pSelectedSlot.DoEnableImageSelected(false);
	//		_pSelectedSlot = null;
	//	}
	//}

	//public void OnReceiveData(int iReceiveSlotID, CInventoryData_Test IReceiveData)
	//{
	//	CLASS_DATA pData = (CLASS_DATA)IReceiveData;

	//	if (_mapInventoryData.ContainsKey(iReceiveSlotID))
	//	{
	//		// 일단 옮기기 전에 들어있던 데이터 백업
	//		CLASS_DATA pDataDroppedPos_Backup = _mapInventoryData[iReceiveSlotID];
	//		IInventoryListener pInventoryListner_Backup = pData.p_pInventoryOwner;

	//		int iSlot_Backup = pData.p_iSlotID;

	//		// 현재 드래그된 아이템을 현재 인벤토리에 일단 옮긴다..
	//		EventSetSlotData(iReceiveSlotID, pData);
	//		pInventoryListner_Backup.OnReceiveData(iSlot_Backup, pDataDroppedPos_Backup);
	//	}
	//	else
	//	{
	//		EventSetSlotData(iReceiveSlotID, pData);
	//	}
	//}

	public void OnClickSlot(IInventorySlot IInventorySlot)
	{

	}

	#endregion Public

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	protected virtual void OnSetInventoryData(CLASS_DATA sInfoData, CLASS_SLOT pSlot) { }

	protected virtual void OnSetPage(int iCurPage, int iMaxPage) { }

	protected abstract List<CLASS_DATA> GetInventoryData();

	protected abstract EInventoryType GetInventoryType();

	protected abstract string GetSpriteName(CLASS_DATA sInfoData);

	//protected virtual void OnSelectedSlot(CUGUIInventorySlot pSlot, CLASS_DATA pData) { }

	//protected virtual void OnUnSelectedSlot() { }

	//protected virtual void OnSetItem(CUGUIInventorySlot pSlot, CLASS_DATA pData) { }

	//protected virtual void OnRemoveItem(int iRealID) { }

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	//private void EventSwapData(int iOwnerSlotID, int iTargetSlotID)
	//{
	//	if (iOwnerSlotID == iTargetSlotID) return;

	//	CLASS_DATA pOwnerDataTemp = _mapInventoryData[iOwnerSlotID];

	//	if (_mapInventoryData.ContainsKey(iTargetSlotID))
	//	{
	//		EventSetSlotData(iOwnerSlotID, _mapInventoryData[iTargetSlotID]);
	//		EventSetSlotData(iTargetSlotID, pOwnerDataTemp);
	//	}
	//	else
	//	{
	//		EventRemoveData(iOwnerSlotID);
	//		EventSetSlotData(iTargetSlotID, pOwnerDataTemp);
	//	}
	//}

	//private void EventSetSlotData(int iSlotID, CLASS_DATA pData)
	//{
	//	pData.DoInitOwnerListener(this);

	//	CUGUIInventorySlot pSlot = _mapInventorySlot[iSlotID];
	//	pSlot.EventSetData(pData);

	//	_mapInventoryData[iSlotID] = pData;

	//	OnSetItem(pSlot, pData);
	//}

	//private void EventRemoveData(int iSlotID)
	//{
	//	if (_mapInventoryData.ContainsKey(iSlotID) == false) return;

	//	int iRealID_Backup = _mapInventoryData[iSlotID].p_iRealID;

	//	_mapInventorySlot[iSlotID].DoEnableImage(false);
	//	_mapInventoryData.Remove(iSlotID);

	//	OnRemoveItem(iRealID_Backup);
	//}

	protected void EventInit_InventoryType()
	{
		_iTotalDataCount = GetInventoryData().Count;

		EInventoryType eInventoryType = GetInventoryType();
		switch (eInventoryType)
		{
			case EInventoryType.Scroll:
				ProcInit_InventoryData(GetInventoryData());
				break;

			case EInventoryType.Page:
			case EInventoryType.Page_Auto:
					ProcInit_InventoryData_Page(1, eInventoryType == EInventoryType.Page_Auto);
				break;
		}
	}

	protected void EventSetMaxPage(int iMaxPage)
	{
		_iMaxPage = iMaxPage;
	}

	protected void EventAddMaxPage(int iAddMaxPage)
	{
		_iMaxPage += iAddMaxPage;
	}

	protected void EventPrevPage()
	{
		_iCurPage--;

		EventSetPage(GetPageClamped(_iCurPage));
	}

	protected void EventNextPage()
	{
		_iCurPage++;

		EventSetPage(GetPageClamped(_iCurPage));
	}

	protected void EventSetPage(int iPage)
	{
		ProcInit_InventoryData_Page(iPage);
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		ProcInit_InventorySlot();

		//if (_bEnableDragItem && _pInventoryCursor == null)
		//{
		//	Debug.Log("InventoryCursor를 인스펙터에 등록해주세요.");
		//	return;
		//}

		//List<CUGUIInventorySlot> listInventorySlot = new List<CUGUIInventorySlot>();
		//GetComponentsInChildren(listInventorySlot);

		//int iCount = listInventorySlot.Count;
		//for (int i = 0; i < iCount; i++)
		//{
		//	CUGUIInventorySlot pSlot = listInventorySlot[i];
		//	pSlot.EventOnAwake();
		//	pSlot.DoSetInventoryListener(this);
		//}

		//_iMaxSlot = iCount;
	}

	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private void ProcInit_Page_Auto()
	{
		EventSetMaxPage(1);

		int i = 1;
		while ((_iTotalDataCount - 1) >= i)
		{
			int iPercent = i % _iMaxSlotCount;
			if (iPercent == 0)
			{
				EventAddMaxPage(1);
			}

			i++;
		}
	}

	private void ProcInit_InventorySlot()
	{
		IInventorySlot[] arrInventoryslot = GetComponentsInChildren<IInventorySlot>(true);

		int iLen = arrInventoryslot.Length;
		for (int i = 0; i < iLen; i++)
		{
			IInventorySlot pSlot = arrInventoryslot[i];

			if (_mapInventorySlot.ContainsKey(i))
			{
				Debug.LogWarning("이미 _mapInventorySlot 에 " + pSlot + " 가 있습니다...");
			}
			else
			{
				_mapInventorySlot.Add(i, pSlot);

				pSlot.DoInit(this);
				pSlot.OnEnableSlot(false);
			}
		}

		_iMaxSlotCount = iLen;
	}

	private void ProcInit_InventoryData(List<CLASS_DATA> listInventoryData)
	{
		_mapInventoryData.Clear();

		int iCount_InventoryData = listInventoryData.Count;
		int iCount = _mapInventorySlot.Count;
		for (int i = 0; i < iCount; i++)
		{
			IInventorySlot pSlot = _mapInventorySlot[i];

			bool bContains_InventoryData = i < iCount_InventoryData;
			if (bContains_InventoryData)
			{
				CLASS_DATA sInfoData = listInventoryData[i];
				string strSpriteName = GetSpriteName(sInfoData);

				pSlot.OnSetSprite(strSpriteName);
				OnSetInventoryData(sInfoData, (CLASS_SLOT)pSlot);

				_mapInventoryData.Add(i, sInfoData);
			}

			pSlot.OnEnableSlot(bContains_InventoryData);
		}
	}

	private void ProcInit_InventoryData_Page(int iPage, bool bPageAuto = false)
	{
		if (bPageAuto)
			ProcInit_Page_Auto();

		int iPrev_PageSlotCount = Mathf.Max(0, iPage - 1) * _iMaxSlotCount;
		int iNext_PageSlotCount = iPage * _iMaxSlotCount;

		List<CLASS_DATA> listInventoryData = GetInventoryData();
		List<CLASS_DATA> listInventoryData_Page = new List<CLASS_DATA>();

		for (int i = iPrev_PageSlotCount; i <= iNext_PageSlotCount; i++)
		{
			if (i < _iTotalDataCount)
			{
				CLASS_DATA sInfoData = listInventoryData[i];
				listInventoryData_Page.Add(sInfoData);
			}
			else
				break;
		}

		ProcInit_InventoryData(listInventoryData_Page);
		OnSetPage(iPage, _iMaxPage);

		_iCurPage = iPage;
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	private int GetPageClamped(int iPage)
	{
		return Mathf.Clamp(iPage, 1, _iMaxPage);
	}

	#endregion Private
}
