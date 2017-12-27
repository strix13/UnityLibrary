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

public class CInventorySlotDataBase
{
	private IInventoryListener _pInventoryOwner; public IInventoryListener p_pInventoryOwner { get { return _pInventoryOwner; } }

	private Sprite _pSprite; public Sprite p_pSprite { get { return _pSprite; } }

	private int _iSlotID; public int p_iSlotID { get { return _iSlotID; } }
	private int _iRealID; public int p_iRealID { get { return _iRealID; } }

	public void DoInit(Sprite pSprite, IInventoryListener pInventoryListener)
	{
		DoInitSprite(pSprite);
		DoInitOwnerListener(pInventoryListener);
	}

	public void DoInitOwnerListener(IInventoryListener pInventoryOwner ) { _pInventoryOwner = pInventoryOwner; }

	public void DoInitSprite(Sprite pSprite) { this._pSprite = pSprite; }

	public void DoSetSlotID(int iSlotID) { this._iSlotID = iSlotID; }

	public void DoSetRealID(int iRealID) { this._iRealID = iRealID; }
}

public interface IInventoryListener
{
	void OnRegisterSlot(CUGUIInventorySlotBase pSlot);

	void OnPickItem(Vector3 v3CursorPos, int iPickSlotID);
	void OnDragItem(Vector3 v3CursorPos, int iPickSlotID);
	void OnDropItem(GameObject pGameObject, int iPickSlotID, int iHashCode);
	void OnSelectSlot(CUGUIInventorySlotBase pSlot);

	void OnReceiveData(int iReceiveSlotID, CInventorySlotDataBase IReceiveData);
}

public class CUGUIInventoryBase<CLASS_DATA> : CObjectBase, IInventoryListener
	where CLASS_DATA : CInventorySlotDataBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EInventoryType
	{
		None,
	}

	#region Field

	/* public - Field declaration            */

	[SerializeField] private CUGUIInventoryCursor _pInventoryCursor = null;

	[SerializeField] private bool _bEnableSwapItemInSameInventory = false;

	[SerializeField] private bool _bEnableSelectSlot = false;
	[SerializeField] private bool _bEnableDragItem = false;

	[SerializeField] private bool _bRemoveItem = false;

	/* protected - Field declaration         */

	protected int _iMaxSlot;

	/* private - Field declaration           */

	private Dictionary<int, CLASS_DATA> _mapInventoryData = new Dictionary<int, CLASS_DATA>(); 
	private Dictionary<int, CUGUIInventorySlotBase> _mapInventorySlot = new Dictionary<int, CUGUIInventorySlotBase>();


	private CUGUIInventorySlotBase _pSelectedSlot;

	private bool _bIsSlotContainItem;

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoInitInventory(Dictionary<int, CLASS_DATA> mapItemData)
	{
		EventUnSelectSlot();
		_mapInventoryData.Clear();

		int iCount = _mapInventorySlot.Count;
		for (int i = 0; i < iCount; i++)
			_mapInventorySlot[i].DoSetEmptySlot();

		int iSlotID = 0;
		var pIter = mapItemData.GetEnumerator();
		while (pIter.MoveNext())
		{
			var pCurrent = pIter.Current;

			int iRealID = pCurrent.Key;
			CLASS_DATA pData = pCurrent.Value;

			pData.DoSetSlotID(iSlotID);
			pData.DoSetRealID(iRealID);

			EventSetSlotData(iSlotID, pData);
			iSlotID++;
		}
	}

	public CLASS_DATA GetInventoryData(int iRealID)
	{
		return _mapInventoryData[iRealID];
	}

	public CUGUIInventorySlotBase GetInventorySlot(int iSlotID)
	{
		return _mapInventorySlot[iSlotID];
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	public void OnRegisterSlot(CUGUIInventorySlotBase pSlot)
	{
		int iSlotID = pSlot.p_iSlotID;
		if (_mapInventorySlot.ContainsKey(iSlotID))
		{
			Debug.Log( "이미 슬롯 키 값이 들어가있습니다." );
			return;
		}

		_mapInventorySlot.Add(iSlotID, pSlot);
	}

	public void OnPickItem(Vector3 v3CursorPos, int iPickSlotID)
	{
		if (_bEnableDragItem == false) return;

		_bIsSlotContainItem = _mapInventoryData.ContainsKey(iPickSlotID);
	}

	public void OnDragItem(Vector3 v3CursorPos, int iPickSlotID)
	{
		if (_bEnableDragItem == false) return;

		if (_bIsSlotContainItem == false) return;

		CUGUIInventorySlotBase pSlot = _mapInventorySlot[iPickSlotID];

		_pInventoryCursor.DoSetImage(pSlot.p_pImage.sprite);
		_pInventoryCursor.DoUpdatePosition(v3CursorPos);
	}

	public void OnDropItem(GameObject pGameObject, int iOwnerSlotID, int iOwnerSlotHashCode)
	{
		if (_bEnableDragItem == false) return;

		if (_bIsSlotContainItem == false) return;

		_pInventoryCursor.SetActive(false);

		if (pGameObject == null) return;

		// 일단 슬롯이 있는지 체크
		CUGUIInventorySlotBase pTargetSlot = pGameObject.GetComponent<CUGUIInventorySlotBase>();
		if (pTargetSlot == null) return;

		CLASS_DATA pOwnerData_Backup = _mapInventoryData[iOwnerSlotID];

		IInventoryEquipSlot<CLASS_DATA> ITargetEquipSlot = pTargetSlot.GetComponent<IInventoryEquipSlot<CLASS_DATA>>();
		if (ITargetEquipSlot != null)
		{
			CUGUIInventorySlotBase pOwnerSlot = GetInventorySlot(iOwnerSlotID);
			if (ITargetEquipSlot.CheckSlotType(pOwnerData_Backup) == false) return;
		}

		if (iOwnerSlotHashCode == pTargetSlot.p_iInvenHashCode)
		{
			// 같은 슬롯일 경우 데이터만 스왑해준다.
			if (_bEnableSwapItemInSameInventory)
				EventSwapData(iOwnerSlotID, pTargetSlot.p_iSlotID);
		}
		else
		{
			// 다른 슬롯일 경우 데이터 발신
			if (_bRemoveItem)
				EventRemoveData(iOwnerSlotID);

			pTargetSlot.OnReceiveData(pOwnerData_Backup);
		}
	}

	public void OnSelectSlot(CUGUIInventorySlotBase pSlot)
	{
		if (_bEnableSelectSlot == false) return;

		EventSelectSlot(pSlot);
	}

	protected void EventSelectSlot(CUGUIInventorySlotBase pSlot)
	{
		int iSlotID = pSlot.p_iSlotID;
		if (_mapInventoryData.ContainsKey(iSlotID) == false) return;

		if (_pSelectedSlot != null)
		{
			_pSelectedSlot.DoEnableImageSelected(false);

			if (_pSelectedSlot.p_iSlotID == pSlot.p_iSlotID)
			{
				EventUnSelectSlot();
				OnUnSelectedSlot();
				return;
			}
		}

		pSlot.DoEnableImageSelected(true);
		OnSelectedSlot(pSlot, _mapInventoryData[pSlot.p_iSlotID]);

		_pSelectedSlot = pSlot;
	}

	protected void EventUnSelectSlot()
	{
		if (_pSelectedSlot != null)
		{
			_pSelectedSlot.DoEnableImageSelected(false);
			_pSelectedSlot = null;
		}
	}

	public void OnReceiveData(int iReceiveSlotID, CInventorySlotDataBase IReceiveData)
	{
		CLASS_DATA pData = (CLASS_DATA)IReceiveData;

		if (_mapInventoryData.ContainsKey(iReceiveSlotID))
		{
			// 일단 옮기기 전에 들어있던 데이터 백업
			CLASS_DATA pDataDroppedPos_Backup = _mapInventoryData[iReceiveSlotID];
			IInventoryListener pInventoryListner_Backup = pData.p_pInventoryOwner;

			int iSlot_Backup = pData.p_iSlotID;

			// 현재 드래그된 아이템을 현재 인벤토리에 일단 옮긴다..
			EventSetSlotData(iReceiveSlotID, pData);
			pInventoryListner_Backup.OnReceiveData(iSlot_Backup, pDataDroppedPos_Backup);
		}
		else
		{
			EventSetSlotData(iReceiveSlotID, pData);
		}
	}

	#endregion Public

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	protected virtual void OnSelectedSlot(CUGUIInventorySlotBase pSlot, CLASS_DATA pData) { }

	protected virtual void OnUnSelectedSlot() { }

	protected virtual void OnSetItem(CUGUIInventorySlotBase pSlot, CLASS_DATA pData) { }

	protected virtual void OnRemoveItem(int iRealID) { }

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	private void EventSwapData(int iOwnerSlotID, int iTargetSlotID)
	{
		if (iOwnerSlotID == iTargetSlotID) return;

		CLASS_DATA pOwnerDataTemp = _mapInventoryData[iOwnerSlotID];

		if (_mapInventoryData.ContainsKey(iTargetSlotID))
		{
			EventSetSlotData(iOwnerSlotID, _mapInventoryData[iTargetSlotID]);
			EventSetSlotData(iTargetSlotID, pOwnerDataTemp);
		}
		else
		{
			EventRemoveData(iOwnerSlotID);
			EventSetSlotData(iTargetSlotID, pOwnerDataTemp);
		}
	}

	private void EventSetSlotData(int iSlotID, CLASS_DATA pData)
	{
		pData.DoInitOwnerListener(this);

		CUGUIInventorySlotBase pSlot = _mapInventorySlot[iSlotID];
		pSlot.EventSetData(pData);

		_mapInventoryData[iSlotID] = pData;

		OnSetItem(pSlot, pData);
	}

	private void EventRemoveData(int iSlotID)
	{
		if (_mapInventoryData.ContainsKey(iSlotID) == false) return;

		int iRealID_Backup = _mapInventoryData[iSlotID].p_iRealID;

		_mapInventorySlot[iSlotID].DoEnableImage(false);
		_mapInventoryData.Remove(iSlotID);

		OnRemoveItem(iRealID_Backup);
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		if (_bEnableDragItem && _pInventoryCursor == null)
		{
			Debug.Log("InventoryCursor를 인스펙터에 등록해주세요.");
			return;
		}

		List<CUGUIInventorySlotBase> listInventorySlot = new List<CUGUIInventorySlotBase>();
		GetComponentsInChildren(listInventorySlot);

		int iCount = listInventorySlot.Count;
		for (int i = 0; i < iCount; i++)
		{
			CUGUIInventorySlotBase pSlot = listInventorySlot[i];
			pSlot.EventOnAwake();
			pSlot.DoSetInventoryListener(this);
		}

		_iMaxSlot = iCount;
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
