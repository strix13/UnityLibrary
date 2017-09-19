using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-03 오후 11:48:05
   Description : 
   Edit Log    : 
   ============================================ */

public interface IRandomItem
{
    int IRandomItem_GetPercent();
}

public class CManagerRandomTable<CLASS_Resource> : CSingletonBase_Not_UnityComponent<CManagerRandomTable<CLASS_Resource>>
    where CLASS_Resource : class, IRandomItem
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum ERandomGetMode
    {
        Peek,
        Delete,
    }
	
	/* public - Field declaration            */
	
	/* private - Field declaration           */

	private List<CLASS_Resource> _listRandomTable = new List<CLASS_Resource>();
	private List<CLASS_Resource> _listRandomTable_Delete = new List<CLASS_Resource>();
	private List<CLASS_Resource> _listRandomTable_Temp = new List<CLASS_Resource>();

	private HashSet<CLASS_Resource> _setWinTable_OnDelete = new HashSet<CLASS_Resource>();

	private ERandomGetMode _eRandomGetMode = ERandomGetMode.Peek;

	private int _iTotalValue = 0;
	private int _iTotalValue_Decrease_OnDelete = 0;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoClearRandomItemTable()
	{
		_listRandomTable.Clear();
	}

	public void DoAddRandomItem(CLASS_Resource pRandomItem)
    {
		int iPercent = pRandomItem.IRandomItem_GetPercent();
		if(iPercent <= 0)
		{
			Debug.LogWarning("Percent is less or equal to zero" + pRandomItem.ToString());
			return;
		}

		//Debug.Log(pRandomItem.ToString() + iPercent);

		_iTotalValue += iPercent;

        _listRandomTable.Add(pRandomItem);
        _listRandomTable.Sort(ComparerRandomItem);

		if (_eRandomGetMode == ERandomGetMode.Delete)
		{
			_listRandomTable_Delete.Add(pRandomItem);
			_listRandomTable_Delete.Sort(ComparerRandomItem);
		}
	}

	public void DoAddRandomItem_Range(List<CLASS_Resource> listRandomItem)
	{
		for(int i = 0; i < listRandomItem.Count; i++)
			DoAddRandomItem(listRandomItem[i]);
	}

	public CLASS_Resource GetRandomItem()
    {
		if (_eRandomGetMode == ERandomGetMode.Peek)
			return ProcGetRandomItem_Peek(_iTotalValue);
		else
			return ProcGetRandomItem_Delete(_iTotalValue - _iTotalValue_Decrease_OnDelete);
	}
	
	public CLASS_Resource GetRandomItem(int iMaxValue)
	{
		CLASS_Resource pRandomItem = null;
		if(_eRandomGetMode == ERandomGetMode.Peek)
			ProcGetRandomItemList(iMaxValue, _listRandomTable);
		else
			ProcGetRandomItemList(iMaxValue, _listRandomTable_Delete);

		if (_listRandomTable_Temp.Count != 0)
		{
			int iRandomIndex = Random.Range(0, _listRandomTable_Temp.Count);
			if (_eRandomGetMode == ERandomGetMode.Delete)
			{
				do
				{
					if (_setWinTable_OnDelete.Contains(_listRandomTable_Temp[iRandomIndex]))
					{
						_listRandomTable_Temp.RemoveAt(iRandomIndex);
						iRandomIndex = Random.Range(0, _listRandomTable_Temp.Count);
						continue;
					}
					else
						break;

				} while (_setWinTable_OnDelete.Contains(_listRandomTable_Temp[iRandomIndex]) && _listRandomTable_Temp.Count != 0);

				pRandomItem = _listRandomTable_Temp[iRandomIndex];
				ProcAddDeleteItem(pRandomItem);
			}
			else
				pRandomItem = _listRandomTable_Temp[iRandomIndex];
		}

		return pRandomItem;
	}

	public void DoSetRandomMode(ERandomGetMode eRandomMode)
	{
		_eRandomGetMode = eRandomMode;
	}

	public void DoReset_OnDeleteMode()
	{
		_setWinTable_OnDelete.Clear();
		_iTotalValue_Decrease_OnDelete = 0;
		_listRandomTable_Delete.Clear();
		_listRandomTable_Delete.AddRange(_listRandomTable);
	}

	public float DoCalculatePercent(CLASS_Resource pItem, int iMaxValue = 0)
	{
		if (iMaxValue == 0)
			iMaxValue = _iTotalValue;

		int iItemPercentValue = pItem.IRandomItem_GetPercent();
		float fPercent = (float)iItemPercentValue / (float)_iTotalValue;

		return fPercent * 100;
	}

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    private CLASS_Resource ProcGetRandomItem_Peek(int iMaxValue)
    {
		if (iMaxValue > _iTotalValue)
			iMaxValue = _iTotalValue;

		return ProcGetRandomItem(iMaxValue, _listRandomTable);
	}

	private CLASS_Resource ProcGetRandomItem_Delete(int iMaxValue)
	{
		int iTotalValue = iMaxValue - _iTotalValue_Decrease_OnDelete;
		if (iMaxValue > iTotalValue)
			iMaxValue = iTotalValue;

		CLASS_Resource pRandomItem = ProcGetRandomItem(iMaxValue, _listRandomTable_Delete);
		ProcAddDeleteItem(pRandomItem);

		return pRandomItem;
	}

	private CLASS_Resource ProcGetRandomItem(int iMaxValue, List<CLASS_Resource> listTable)
	{
		CLASS_Resource pRandomItem = null;
		int iRandomValue = Random.Range(0, iMaxValue);
		int iCheckValue = 0;
		for (int i = 0; i < listTable.Count; i++)
		{
			CLASS_Resource pRandomItemCurrent = listTable[i];
			iCheckValue += pRandomItemCurrent.IRandomItem_GetPercent();
			if (iRandomValue < iCheckValue)
			{
				pRandomItem = pRandomItemCurrent;
				break;
			}
		}

		return pRandomItem;
	}

	public void ProcGetRandomItemList(int iMaxValue, List<CLASS_Resource> listTable)
	{
		_listRandomTable_Temp.Clear();
		for (int i = 0; i < listTable.Count; i++)
		{
			CLASS_Resource pRandomItemCurrent = listTable[i];
			if (pRandomItemCurrent.IRandomItem_GetPercent() <= iMaxValue)
				_listRandomTable_Temp.Add(pRandomItemCurrent);
			else
				break;
		}
	}

		/* private - Other[Find, Calculate] Function 
		   찾기, 계산 등의 비교적 단순 로직         */

	private int ComparerRandomItem(CLASS_Resource pResourceX, CLASS_Resource pResourceY)
    {
        int iValueX = pResourceX.IRandomItem_GetPercent();
        int iValueY = pResourceY.IRandomItem_GetPercent();

        if (iValueX < iValueY)
            return -1;
        else if (iValueX > iValueY)
            return 1;
        else
            return 0;
    }

	private void ProcAddDeleteItem(CLASS_Resource pItem)
	{
		if (pItem == null) return;

		_listRandomTable_Delete.Remove(pItem);
		_iTotalValue_Decrease_OnDelete += pItem.IRandomItem_GetPercent();
		_setWinTable_OnDelete.Add(pItem);

	}
}
