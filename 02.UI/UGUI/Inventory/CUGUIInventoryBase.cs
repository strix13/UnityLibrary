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

public class CInventoryDataBase
{
	private IInventoryListener _pInventoryOwner; public IInventoryListener p_pInventoryOwner { get { return _pInventoryOwner; } }

	private Sprite _pSprite; public Sprite p_pSprite { get { return _pSprite; } }

	private int _iSlotID; public int p_iSlotID { get { return _iSlotID; } }

	public void DoInit(int iSlotID, string strImageName, IInventoryListener pInventoryListener)
	{
		string strPathImage = string.Format("Image/{0}", strImageName);

		DoInitSprite(Resources.Load<Sprite>(strPathImage));
		DoInitOwnerListener(pInventoryListener);

		_iSlotID = iSlotID;
	}

	public void DoInitOwnerListener(IInventoryListener pInventoryOwner ) { _pInventoryOwner = pInventoryOwner; }

	public void DoInitSprite(Sprite pSprite) { this._pSprite = pSprite; }

	public void DoInitSlotID(int iSlotID) { this._iSlotID = iSlotID; }
}

public interface IInventoryListener
{
	void OnRegisterSlot(CUGUIInventorySlot pSlot);

	void OnPickItem(Vector3 v3CursorPos, int iPickSlotID);
	void OnDragItem(Vector3 v3CursorPos);
	void OnDropItem(GameObject pGameObject, int iPickSlotID, int iHashCode);

	void OnReceiveData(int iReceiveSlotID, CInventoryDataBase IReceiveData);
}

public class CUGUIInventoryBase<CLASS_DATA> : CObjectBase, IInventoryListener
	where CLASS_DATA : CInventoryDataBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EInventoryType
	{
		None,
		MoveItem_Drag,
		MoveItem_Instant
	}


	#region Field

	/* public - Field declaration            */

	[SerializeField] private CUGUIInventoryCursor _pInventoryCursor;
	[SerializeField] private EInventoryType _eType = EInventoryType.None;

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private Dictionary<int, CLASS_DATA> _mapInventoryData = new Dictionary<int, CLASS_DATA>();
	private Dictionary<int, CUGUIInventorySlot> _mapInventorySlot = new Dictionary<int, CUGUIInventorySlot>();

	private CUGUIInventorySlot _pTempOwnerSlot;
	private CLASS_DATA _pTempOwnerItem;

	private bool _bIsSlotContainItem;

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoInitInventory(List<CLASS_DATA> listData)
	{
		int iCount = listData.Count;
		for (int i = 0; i < iCount; i++)
		{
			CLASS_DATA pCurrentData = listData[i];
			int iCurSlotID = pCurrentData.p_iSlotID;

			EventSetData(iCurSlotID, pCurrentData);
		}
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/
	
	public void OnRegisterSlot(CUGUIInventorySlot pSlot)
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
		_bIsSlotContainItem = _mapInventoryData.ContainsKey(iPickSlotID);

		if (_bIsSlotContainItem)
			_pInventoryCursor.DoInit(_mapInventoryData[iPickSlotID].p_pSprite, v3CursorPos);
		else
			print("해당 슬롯에 아이템이 없습니다.");
	}

	public void OnDragItem(Vector3 v3CursorPos)
	{
		if (_eType == EInventoryType.None) return;

		_pInventoryCursor.DoUpdatePosition(v3CursorPos);
	}

	public void OnDropItem(GameObject pGameObject, int iOwnerSlotID, int iOwnerSlotHashCode)
	{
		if (_eType == EInventoryType.None) return;
		if (_bIsSlotContainItem == false) return;

		_pInventoryCursor.SetActive(false);
		if (pGameObject == null) return;

		CUGUIInventorySlot pTargetSlot = pGameObject.GetComponent<CUGUIInventorySlot>();
		if (pTargetSlot == null) return;

		if (iOwnerSlotHashCode != pTargetSlot.p_iInvenHashCode)
		{
			CLASS_DATA pOwnerDataTemp = _mapInventoryData[iOwnerSlotID];

			// 보내기전에 기존건 삭제한다 이유는 어짜피 보낸쪽에서 값 유무 판단해서 기존 데이터가 또 들어오기때문
			EventRemoveData(iOwnerSlotID);
			pTargetSlot.OnReceiveData(pOwnerDataTemp);

			return;
		}

		EventSwapData(iOwnerSlotID, pTargetSlot.p_iSlotID);
	}

	public void OnReceiveData(int iReceiveSlotID, CInventoryDataBase IReceiveData)
	{
		Debug.Log(name + " " + iReceiveSlotID + "  " + IReceiveData.p_pSprite, IReceiveData.p_pSprite);

		CLASS_DATA pData = (CLASS_DATA)IReceiveData;

		if (_mapInventoryData.ContainsKey(iReceiveSlotID))
		{
			// 일단 옮기기 전에 들어있던 데이터 백업
			CLASS_DATA pDataDroppedPos = _mapInventoryData[iReceiveSlotID];
			IInventoryListener pInventoryListner = pData.p_pInventoryOwner;
			int iSlotBackup = pData.p_iSlotID;

			// 현재 드래그된 아이템을 현재 인벤토리에 일단 옮긴다..
			EventSetData(iReceiveSlotID, pData);

			// 들어있던 데이터를 내게 준 스크롤뷰에게 넘기기
			pInventoryListner.OnReceiveData( iSlotBackup, pDataDroppedPos);
		}
		else
			EventSetData(iReceiveSlotID, pData);
	}

	#endregion Public

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	private void EventSwapData(int iOwnerSlotID, int iTargetSlotID)
	{
		if (iOwnerSlotID == iTargetSlotID) return;

		CLASS_DATA pOwnerDataTemp = _mapInventoryData[iOwnerSlotID];

		if (_mapInventoryData.ContainsKey(iTargetSlotID))
		{
			EventSetData(iOwnerSlotID, _mapInventoryData[iTargetSlotID]);
			EventSetData(iTargetSlotID, pOwnerDataTemp);
		}
		else
		{
			EventRemoveData(iOwnerSlotID);
			EventSetData(iTargetSlotID, pOwnerDataTemp);
		}
	}

	private void EventSetData(int iSlotID, CLASS_DATA pData)
	{
		pData.DoInitSlotID(iSlotID);
		pData.DoInitOwnerListener(this);

		_mapInventoryData[iSlotID] = pData;
		_mapInventorySlot[iSlotID].EventSetData(pData);
	}

	private void EventRemoveData(int iSlotID)
	{
		if (_mapInventoryData.ContainsKey(iSlotID) == false) return;

		_mapInventoryData.Remove(iSlotID);
		_mapInventorySlot[iSlotID].EventRemoveData();
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		if (_pInventoryCursor == null)
		{
			Debug.Log( "InventoryCursor 를 인스펙터에 등록해주세요." );
			return;
		}

		List<CUGUIInventorySlot> listInventorySlot = new List<CUGUIInventorySlot>();
		GetComponentsInChildren(listInventorySlot);

		int iCount = listInventorySlot.Count;
		for (int i = 0; i < iCount; i++)
		{
			CUGUIInventorySlot pSlot = listInventorySlot[i];
			pSlot.EventOnAwake();
			pSlot.DoSetInventoryListener(this);
		}
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
